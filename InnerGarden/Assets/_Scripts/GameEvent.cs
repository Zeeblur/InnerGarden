using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


// Just putting this here for now, will move to correspond with the JSON reader.
public enum StoryletType
{
    GENERIC,
    SOVEREIGN,
    CHAMPION,
    LOVER,
    MAGICIAN
}

// A GameEvent is the gameplay code for the story let 
// If the GameManager creates and holds these events, we might need to pass it data. 
public class GameEvent : MonoBehaviour
{

    // Lets keep the seed around for now
    const int seed = 2151;
    private const int k_typeCount = 5;

    public EventStates currentState;
    public Transform cardPrefab;
    public Transform storyCanvas;

    public static List<int> availableStorylets;  // lets hope the GameManger can update this for us, so the "event" doesn't need to know anything about the player. 


    // Hardcoding colour for archetypes here. TODO: Update with json filling instead
    public Color[] cardColours = new Color[5];

    // Initial fade to background
    // Transition to two cards with a choice
    // When choice is made, display the story + other options. 
    // Some sort of end.

    private Transform cardA, cardB;
    private Button btnA, btnB;
    private GameObject[] cardGOs = new GameObject[2];

    private GameObject chosenStory;
    // vars for UI animation
    private Transform startPosition;
    public float speed = 1.0F;
    private float startTime;
    private float journeyLength;

    // this should tie into the screen/resolution ideally.
    private Vector3 offset = new Vector3(400.0f, 0.0f, 0.0f);

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

        // Will need to instantiate card prefabs here
        cardA = Instantiate(cardPrefab, storyCanvas);
        cardA.SetParent(storyCanvas);
        cardGOs[0] = cardA.gameObject;

        cardB = Instantiate(cardPrefab, storyCanvas);
        cardB.SetParent(storyCanvas);
        cardB.position += offset;
        cardGOs[1] = cardB.gameObject;

        // hidey
        cardGOs[0].SetActive(false);
        cardGOs[1].SetActive(false);

        // initialise colours
        cardColours[0] = new Color(0.95f, 0.6f, 0.1f, 1.0f);
        cardColours[1] = new Color(0.67f, 0.1f, 0.8f, 1.0f);
        cardColours[2] = new Color(1.0f, 0.0f, 0.1f, 1.0f);
        cardColours[3] = new Color(1.0f, 0.6f, 0.9f, 1.0f);
        cardColours[4] = new Color(0.0f, 0.8f, 0.95f, 1.0f);

        RandomizeCards(); // this will create the cards but not display them until EventState has switched

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

    public void RandomizeCards()
    {
        // we could weight this if we want, for now I've just made sure it doesn't pick the same type twice.
        // probably want to return a tuple of IDs (depending on which Storylet picked)
        // to do this we'll also probably need a list of available choices to us! and one's we've picked before. 

        // unless we wanted the player's archetypes & past to determine the next choices.. 

        // Ask the reader for a storylet
        // 
        int first = Random.Range(0, k_typeCount);
        int second = first;
        while (second == first)
        {
            second = Random.Range(0, k_typeCount);
        }

        btnA = createEventCard(cardA, first);
        btnB = createEventCard(cardB, second);
    }

    public void ShowCards()
    {
        foreach (GameObject go in cardGOs)
        {
            go.SetActive(true);
        }
    }

    // this creates a button handle, assigns the text colour & callback. 
    public Button createEventCard(Transform card, int inputChoice)
    {
        if (card.GetComponentInChildren<Text>()) // nullcheck
        {
            card.GetComponentInChildren<Text>().text = ((StoryletType)inputChoice).ToString();
            card.GetComponentInChildren<Image>().color = cardColours[inputChoice];
        }

        Button btn = card.GetComponentInChildren<Button>();
        btn.onClick.AddListener(delegate { TaskOnClick(btn); });
        return btn;
    }

    public void DisplayNarrativeChoices()
    {
        // lerp movement of card to middle position

        float distCovered = (Time.time - startTime) * speed;

        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        chosenStory.transform.position = Vector3.Lerp(startPosition.position, storyCanvas.position, fractionOfJourney);

        chosenStory.GetComponentInChildren<Text>().text = "chosens story timeeee \n a\n b\n c\n";


    }

    void TaskOnClick(Button btn)
    {
        currentState = EventStates.NARRATIVECHOICE;
        Debug.Log("You have clicked the button!");
        print(btn);
        chosenStory = btn.transform.parent.gameObject;
        startPosition = btn.transform;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition.position, storyCanvas.position);

        // destroy the other cards.
        foreach (GameObject go in cardGOs)
        {
            print(go);
            if (go != chosenStory)
                Destroy(go);
        }

        
        
    }
}
