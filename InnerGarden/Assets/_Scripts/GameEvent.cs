using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Narrative;

namespace Narrative
{
    // Just putting this here for now, will move to correspond with the JSON reader.
    public enum Archetype
    {
        SOVEREIGN,
        CHAMPION,
        LOVER,
        MAGICIAN
    }

    public enum CardType
    {
        HOBBIES,
        OUTINGS,
        SOCIAL, 
        WORK
    }

    public class StoryLet
    {
        public string body;
        public string[] answers;
        public Archetype[] answerKey;
        public CardType cardType;
    }
}

// A GameEvent is the gameplay code for the story let 
// If the GameManager creates and holds these events, we might need to pass it data. 
public class GameEvent : MonoBehaviour
{

    // Lets keep the seed around for now
    const int seed = 2151;
    public int k_cardCount = 9;
    public int gridSize = 3;

    public EventStates currentState;
    public Transform cardPrefab;
    public Transform storyCanvas;
    public Transform backCardPrefab;
    public Transform gardenButton;
    public int entryReq = 5;   // garden entry requirements

    public Sprite[] cardBackgrounds = new Sprite[4];

    public string gardenBtnText = "Enter The Garden\n (";

    public static List<int> availableStorylets;  // lets hope the GameManger can update this for us, so the "event" doesn't need to know anything about the player. 

    public static GameManager gm;

    // Hardcoding colour for archetypes here. TODO: Update with json filling instead
    public Color[] cardColours = new Color[5];

    private GameObject[] cardGOs;

    private GameObject chosenStory;
    private Transform chosenStoryTrans;

    private Narrative.StoryLet currentStoryLet;
    private Narrative.StoryLet[] stories;

    // vars for UI animation
    private Transform startPosition;
    public float speed = 1.0F;
    private float startTime;
    private float journeyLength;
    public Transform cardTarget;

    // this should tie into the screen/resolution ideally. // TODO
    private Vector3 gridPosition = new Vector3(-200.0f, 200.0f, 0.0f);
    private Vector3 colOffset = new Vector3(250.0f, 0.0f, 0.0f);
    private Vector3 rowOffset = new Vector3(0.0f, -200.0f, 0.0f);

    public enum EventStates
    {
        INTRO,
        EVENTCHOICE,
        NARRATIVECHOICE,
        CONCLUSION
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = EventStates.INTRO;
        
        Random.InitState(seed);

        cardGOs = new GameObject[k_cardCount];

        stories = GameManager.FetchRandomStories(k_cardCount);

        // Will need to instantiate card prefabs here
        for (int i = 0; i < k_cardCount; i++)
        {
            CreateCard(i, stories[i].cardType);
        }

        // initialise colours TODO not used currently
        cardColours[0] = new Color(0.95f, 0.6f, 0.1f, 1.0f);
        cardColours[1] = new Color(0.67f, 0.1f, 0.8f, 1.0f);
        cardColours[2] = new Color(1.0f, 0.0f, 0.1f, 1.0f);
        cardColours[3] = new Color(1.0f, 0.6f, 0.9f, 1.0f);
        cardColours[4] = new Color(0.0f, 0.8f, 0.95f, 1.0f);

        // TODO We don't currently have Random anymore for the stories as it's hardcoded. 

        // disable the garden button until later.
        gardenButton.GetComponent<Button>().interactable = false;
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case EventStates.INTRO:
                // poll the timer then present choice
                StartCoroutine(Countdown2());
                break;
            case EventStates.EVENTCHOICE:
                // present the choice to the player and accept input
                ShowCards();
                break;
            case EventStates.NARRATIVECHOICE:
                DisplayNarrativeChoices();
                break;
            case EventStates.CONCLUSION:
                break;
        }
    }

    private void CreateCard(int index, Narrative.CardType type)
    {
        Transform card = Instantiate(cardPrefab, storyCanvas);
        card.SetParent(storyCanvas);
        card.position += gridPosition;

        int colValue = index % gridSize;

        card.position += (colOffset * colValue);

        int rowValue = (int)Mathf.Floor(index / gridSize);
        card.position += (rowOffset * rowValue);
        
        cardGOs[index] = card.gameObject;
        cardGOs[index].SetActive(false);

        Button btn = CreateEventCard(card, type);      

    }

    private IEnumerator Countdown2()
    {
        while (currentState != EventStates.EVENTCHOICE)
        {
            yield return new WaitForSeconds(2); //wait 2 seconds
            if (currentState == EventStates.INTRO)
                currentState = EventStates.EVENTCHOICE;
            break;
        }
    }

    public void ShowCards()
    {
        foreach (GameObject go in cardGOs)
        {
            if (go)
                go.SetActive(true);
        }
    }

    // this creates a button handle, assigns the text colour & callback. 
    public Button CreateEventCard(Transform card, Narrative.CardType inputChoice)
    {
        if(card.GetComponentInChildren<Image>())
        {
            card.GetComponentInChildren<Image>().sprite = cardBackgrounds[(int)inputChoice];
            card.GetComponentInChildren<Image>().color = cardColours[(int)inputChoice];
        }

        if (card.GetComponentInChildren<Text>()) // nullcheck
        {
            card.GetComponentInChildren<Text>().text = (inputChoice).ToString();
        }

        Button btn = card.GetComponentInChildren<Button>();
        btn.onClick.AddListener(delegate { TaskOnClickCard(btn); });
        return btn;
    }

    void TaskOnClickCard(Button btn)
    {
        if (currentState == EventStates.EVENTCHOICE)
        {
            currentState = EventStates.NARRATIVECHOICE;
            Debug.Log("You have clicked the button!");

            chosenStory = btn.transform.parent.gameObject;

            // bring to front. 
            chosenStory.GetComponent<RectTransform>().SetAsLastSibling();

            // deletes chosen card and turns it into a story card
            chosenStory = CreateStoryCard(btn.transform.position);
        }
        // disables the rest
    }

    void OptionChosen(Archetype arch)
    {
        Debug.Log("You scored! " + arch.ToString());
        GameManager.IncreaseScore(arch);

        // What do we do when finished? 
        Destroy(chosenStory);
        currentState = EventStates.EVENTCHOICE;

        gardenButton.GetComponentInChildren<Text>().text = gardenBtnText + "(" + GameManager.gardenCounter + "/" + entryReq + ")";

        if (GameManager.gardenCounter == entryReq)
        {
            GameManager.PrintScores();
            gardenButton.GetComponent<Button>().interactable = true;
        }


        // TODO should really delete dangling GO References
        
    }

    public GameObject CreateStoryCard(Vector3 inPos)
    {
        // flip prefab?
        Destroy(chosenStory);
        chosenStoryTrans = Instantiate(backCardPrefab.transform, storyCanvas);
        chosenStoryTrans.SetParent(storyCanvas);
        chosenStoryTrans.position = inPos;
        startPosition = chosenStoryTrans;

        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition.position, cardTarget.position);

        // Update Text
        // this should return a story object that we can use, at the moment I'm returning a string. 
        currentStoryLet = GameManager.GetStory();

        Text cardText = chosenStoryTrans.GetComponentInChildren<Text>();

        cardText.text = currentStoryLet.body;

        Button[] cardBtns = chosenStoryTrans.GetComponentsInChildren<Button>();

        for (uint i = 0; i < currentStoryLet.answers.Length; i++)
        {
            // first btn is body text
            cardBtns[i+1].GetComponentInChildren<Text>().text = currentStoryLet.answers[i];

            // answer key is the archetypes of the options. 
            var index = i; // have to keep a reference of i as using delegate
            cardBtns[i+1].onClick.AddListener(delegate { OptionChosen(currentStoryLet.answerKey[index]); });
        }

        return chosenStoryTrans.gameObject;

    }

    public void DisplayNarrativeChoices()
    {
        // lerp movement of card to middle position
        float deltaT = Time.time - startTime;
        float distCovered = deltaT * speed;
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        if (chosenStoryTrans)
            chosenStoryTrans.position = Vector3.Lerp(startPosition.position, cardTarget.position, fractionOfJourney);
    }   

    // TODO probably don't need this function anymore.
    void DestroyAllCards()
    {
        // destroy the other cards.
        foreach (GameObject go in cardGOs)
        {
            print(go);
            if (go != chosenStory)
                Destroy(go);
        }
    }
}
