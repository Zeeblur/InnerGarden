using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Data = Narrative;



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

    // We don't have a player atm so just leaving "scores" here as a set of ints.
    private int[] m_scores = new int[4] { 0, 0, 0, 0 };

    // read only counters for if available to get into garden
    public static int gardenCounter { get; private set; }
    public static int gardenVisits { get; private set; }

    private const int k_totalStorylets = 20;

    private Data.StoryLet[] allStories = new Data.StoryLet[k_totalStorylets];

    private List<int> usedStoryIndex = new List<int>();

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
        
    }

    // TODO: changed from string to real type from JSON
    public static Data.StoryLet GetStory()
    {
        Data.StoryLet hardcodedStory = new Data.StoryLet();
        hardcodedStory.body = "You’re spending a few days at a friend’s, and they want to show you this new game they are in love with. Problem, it’s a platformer, and requires pressing buttons in quick sequences. You’re not too sure if it’s a result of your executive function deficit or an issue with your motor skills, but games like that have always been a pain to play for you.";

        hardcodedStory.answers = new string[3];
        hardcodedStory.answers[0] = "Your friend isn’t asking you to help them progress in the game, though, they just want to share something that makes them happy.You take the controller and listen to their excited explanations.";
        hardcodedStory.answers[1] = "You decline playing yourself, but tell them you’ll gladly watch them play. You sneak in a couple sketches of their lively demonstration, earning yourself a few indulgent glances.";
        hardcodedStory.answers[2] = "You explain how platformers tend to be a struggle for you, and ask if you can keep the play session quick. Maybe you could play some strategy coop game afterwards, since we both enjoy them?";

        hardcodedStory.answerKey = new Data.Archetype[3];
        hardcodedStory.answerKey[0] = Data.Archetype.LOVER;
        hardcodedStory.answerKey[1] = Data.Archetype.MAGICIAN;
        hardcodedStory.answerKey[2] = Data.Archetype.SOVEREIGN;

        return hardcodedStory;
    }

    public static void LoadJson()
    {
        // populate all stories array
        for(int i = 0; i < k_totalStorylets; i++)
        {
            Instance.allStories[i] = new Data.StoryLet(); // random data atm TODO switch with data
            Instance.allStories[i].body = "fgfggf";
            Instance.allStories[i].cardType = (Narrative.CardType)Random.Range(0, 3);
        }
    }

    public static Data.StoryLet[] FetchRandomStories(int count)
    {
        // if no stories load them :D
        if (Instance.allStories[0] == null)
        {
            LoadJson();
        }

        // return array of count stories
        Data.StoryLet[] chosenStories = new Data.StoryLet[count];

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

            // test vars
            chosenStories[i].body = i.ToString();

            Instance.usedStoryIndex.Add(rn);
        }

        return chosenStories;
    }

    public static void UnlockCondition(string itemName)
    {
        // This should be called when a player has a special unlock. e.g Cat.
    }

    public static void IncreaseScore(Data.Archetype inA)
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
            totals += ((Data.Archetype)i).ToString() + ": " + Instance.m_scores[i] + "\n";
        }
      
        Debug.Log("You have the scores: " + totals);
    }
}
