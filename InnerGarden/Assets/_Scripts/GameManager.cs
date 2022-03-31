using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Data;
using Narrative;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
 
    // ensure singleton
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private uint[] m_scores = new uint[4] { 0, 0, 0, 0 };

    public uint[] scores = new uint[4];

    // read only counters for if available to get into garden
    public static int gardenCounter { get; private set; }
    public static int gardenVisits { get; private set; }

    private const int k_totalStorylets = 20;

    private Narrative.StoryLet[] allStories = new Narrative.StoryLet[k_totalStorylets];

    private List<int> usedStoryIndex = new List<int>();

    public static float UIScale;
    public static GameEvent.EventStates prevES;
    public static bool optionsOpen = false;
    public static Vector2 referenceRes;
    public static Vector2 currentRes;
    public static CanvasScaler cScaler;
    public static bool fullScreen = false;

    void OnEnable()
    {
        print("GM says OnEnable");
        UIScale = PlayerPrefs.GetFloat("UIScale");
        referenceRes = new Vector2(PlayerPrefs.GetFloat("RefRes_X"), PlayerPrefs.GetFloat("RefRes_Y"));

        // update canvas scaler with player pref
        cScaler = Instance.gameObject.GetComponent<CanvasScaler>();
        if(cScaler)
            cScaler.referenceResolution = referenceRes * UIScale;

        fullScreen = PlayerPrefs.GetInt("IsFullscreen") == 1 ? true : false;
        Screen.fullScreen = fullScreen;
        currentRes = new Vector2(PlayerPrefs.GetFloat("CurRes_X"), PlayerPrefs.GetFloat("CurRes_Y"));
        Screen.SetResolution((int)currentRes.x, (int)currentRes.y, Screen.fullScreen);
    }

    // Start is called before the first frame update
    void Start()
    {
        gardenCounter = 0;
        gardenVisits = 0;
        print(Screen.currentResolution);
        LoadJson();
    }

    // Update is called once per frame
    void Update()
    {
        // easy cheat
        Instance.m_scores = scores;

        // Check for keyboard inputs
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
        {
            if (!optionsOpen)
            {
                print("Flip State options");
                optionsOpen = true;
                prevES = GameEvent.GetEventState();
                GameEvent.ChangeEventState(GameEvent.EventStates.OPTIONS);
            }
        }
    }

    public static void Resume()
    {
        // resume last event state
        GameEvent.ChangeEventState(prevES);
        optionsOpen = false;
    }

    // TODO: changed from string to real type from JSON
    public static Narrative.StoryLet GetStory()
    {
        Narrative.StoryLet hardcodedStory = new Narrative.StoryLet();
        hardcodedStory.body = "You’re spending a few days at a friend’s, and they want to show you this new game they are in love with. Problem, it’s a platformer, and requires pressing buttons in quick sequences. You’re not too sure if it’s a result of your executive function deficit or an issue with your motor skills, but games like that have always been a pain to play for you.";

        hardcodedStory.answers = new string[3];
        hardcodedStory.answers[0] = "Your friend isn’t asking you to help them progress in the game, though, they just want to share something that makes them happy.You take the controller and listen to their excited explanations.";
        hardcodedStory.answers[1] = "You decline playing yourself, but tell them you’ll gladly watch them play. You sneak in a couple sketches of their lively demonstration, earning yourself a few indulgent glances.";
        hardcodedStory.answers[2] = "You explain how platformers tend to be a struggle for you, and ask if you can keep the play session quick. Maybe you could play some strategy coop game afterwards, since we both enjoy them?";

        hardcodedStory.answerKey = new Narrative.Archetype[3];
        hardcodedStory.answerKey[0] = Narrative.Archetype.LOVER;
        hardcodedStory.answerKey[1] = Narrative.Archetype.MAGICIAN;
        hardcodedStory.answerKey[2] = Narrative.Archetype.SOVEREIGN;

        return hardcodedStory;
    }

    public static void LoadJson()
    {
        StoryletReader.RootObject root = StoryletReader.ReadData();

        print(root.stories[0].storyletText);

        // populate all stories array
        for(int i = 0; i < k_totalStorylets; i++)
        {
            Instance.allStories[i] = new Narrative.StoryLet();
            Instance.allStories[i].body = root.stories[i].storyletText;

            int optLen = root.stories[i].storyletOptions.Length;
            Instance.allStories[i].answers = new string[optLen];
            Instance.allStories[i].answerKey = new Narrative.Archetype[optLen];
            for (int j = 0; j < optLen; j++)
            {
                Instance.allStories[i].answers[j] = root.stories[i].storyletOptions[j].optionText;

                try
                {
                    Instance.allStories[i].answerKey[j] = (Narrative.Archetype)System.Enum.Parse(typeof(Narrative.Archetype), root.stories[i].storyletOptions[j].optionArchetype);
                }
                catch
                {
                    Debug.Log("Warning: archetype not recognised for storylet: " + root.stories[i].storyletID + " Option: " + j);
                }
            }

            try
            {
                Instance.allStories[i].cardType = (Narrative.CardType)System.Enum.Parse(typeof(Narrative.CardType), root.stories[i].storyletPic);
            }
            catch
            {
                Debug.Log("Warning: storyletPic not recognised for storylet: " + root.stories[i].storyletID);
            }

        }
    }

    public static Narrative.StoryLet[] FetchRandomStories(int count)
    {
        // if no stories load them :D
        if (Instance.allStories[0] == null)
        {
            LoadJson();
        }

        // return array of count stories
        Narrative.StoryLet[] chosenStories = new Narrative.StoryLet[count];

        for (int i = 0; i < count; i++)
        {
            // get a random number
            int rn = Random.Range(0, k_totalStorylets);

            // has it been used already? get another
            while(Instance.usedStoryIndex.Contains(rn))
            {
                rn = Random.Range(0, k_totalStorylets);
            }

            chosenStories[i] = Instance.allStories[rn];

            Instance.usedStoryIndex.Add(rn);
        }

        return chosenStories;
    }

    public static void UnlockCondition(string itemName)
    {
        // This should be called when a player has a special unlock. e.g Cat.
    }

    public static void IncreaseScore(Narrative.Archetype inA)
    {
        Instance.m_scores[((int)inA)]++;
        gardenCounter++;
    }

    public static void LeaveGarden()
    {
        gardenVisits++;
        SceneManager.LoadScene("Story");
    }

    public static void PrintScores()
    {
        string totals = "";

        for (uint i = 0; i < Instance.m_scores.Length; ++i)
        {
            totals += ((Narrative.Archetype)i).ToString() + ": " + Instance.m_scores[i] + "\n";
        }
      
        Debug.Log("You have the scores: " + totals);
    }

    public static uint[] GetScores()
    {
        return Instance.m_scores;
    }


}
