using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;

public class StoryletReader : MonoBehaviour
{
    string filepath, testPath;
    public string jsonString;


    void Start()
    {
        filepath = Application.dataPath + "/Storylets/Storylets_schema.json";

        testPath = Application.dataPath + "/Storylets/storyTest.json";
        jsonString = File.ReadAllText(filepath);

        //  Root myStory = JsonUtility.FromJson<Root>(js>(jsonString);

        // create own storylet
        Storylet story = new Storylet();
        story.storyletText = "beefhouse";

        JSONNode daya = JSON.Parse(filepath);
        JSONNode dindex = daya.Value;
        print(dindex);
    }

    // Update is called once per frame
    void Update()
    {
        // read the JSON. 

        //Storylet[] mystuy = JsonHelper.FromJson<Storylet>(testPath);

        //print(mystuy[0].storyletText);

        //Root[] myStory = JsonHelper.FromJson<Root>(testPath);
        //print(myStory[0].properties.storyletText);

    }

    [System.Serializable]
    public class Storylet
    {
        public int storyletID;
        public string storyletCat;
        public string storyletPic;
        public string storyletText;
        public string storyletComment;
        public string[] storyletIf;
        public string[] storyletOptions;
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class StoryletID
    {
        public string description { get; set; }
        public string type { get; set; }
    }

    [System.Serializable]
    public class StoryletCat
    {
        public string description { get; set; }
        public string type { get; set; }
        public List<string> @enum { get; set; }

    }

    public class StoryletPic
    {
        public string description { get; set; }
        public string type { get; set; }
    }

    public class Items
    {
        public string Ref { get; set; }
    }

    public class StoryletIf
    {
        public string description { get; set; }
        public string type { get; set; }
        public Items items { get; set; }
    }

    public class StoryletText
    {
        public string description { get; set; }
        public string type { get; set; }
        public int maxLength { get; set; }
    }

    public class StoryletOptions
    {
        public string description { get; set; }
        public string type { get; set; }
        public Items items { get; set; }
    }

    public class StoryletComment
    {
        public string description { get; set; }
        public string type { get; set; }
    }

    [System.Serializable]
    public class Properties
    {
        public StoryletID storyletID { get; set; }
        public StoryletCat storyletCat { get; set; }
        public StoryletPic storyletPic { get; set; }
        public StoryletIf storyletIf { get; set; }
        public StoryletText storyletText { get; set; }
        public StoryletOptions storyletOptions { get; set; }
        public StoryletComment storyletComment { get; set; }
        public DefaultOutcomeText defaultOutcomeText { get; set; }
        public DefaultOutcomeEffect defaultOutcomeEffect { get; set; }
        public AlternativeOutcomeIf alternativeOutcomeIf { get; set; }
        public AlternativeOutcomeText alternativeOutcomeText { get; set; }
        public AlternativeOutcomeEffect alternativeOutcomeEffect { get; set; }
        public OptionText optionText { get; set; }
        public Archetypes archetypes { get; set; }
        public OptionOutcomes optionOutcomes { get; set; }
        public Variable variable { get; set; }
        public Modifyer modifyer { get; set; }
        public CompType compType { get; set; }
        public CompValue compValue { get; set; }
    }

    public class OptionText
    {
        public string description { get; set; }
        public string type { get; set; }
        public int maxLength { get; set; }
    }

    public class Archetypes
    {
        public string description { get; set; }
        public string type { get; set; }
    }

    public class DefaultOutcomeText
    {
        public string definition { get; set; }
        public string type { get; set; }
        public int maxLength { get; set; }
    }

    public class DefaultOutcomeEffect
    {
        public string description { get; set; }
        public string type { get; set; }
        public Items items { get; set; }
    }

    public class AlternativeOutcomeIf
    {
        public string description { get; set; }
        public string type { get; set; }
        public Items items { get; set; }
    }

    public class AlternativeOutcomeText
    {
        public string definition { get; set; }
        public string type { get; set; }
        public int maxLength { get; set; }
    }

    public class AlternativeOutcomeEffect
    {
        public string description { get; set; }
        public string type { get; set; }
        public Items items { get; set; }
    }

    public class OptionOutcomes
    {
        public string description { get; set; }
        public string type { get; set; }
        public Properties properties { get; set; }
        public List<string> required { get; set; }
    }

    public class Variable
    {
        public string description { get; set; }
        public string type { get; set; }
        public List<string> @enum { get; set; }
    }

    public class Modifyer
    {
        public string description { get; set; }
        public string type { get; set; }
    }

    public class Effect
    {
        public string description { get; set; }
        public string type { get; set; }
        public Properties properties { get; set; }
        public List<string> required { get; set; }
    }

    public class CompType
    {
        public string description { get; set; }
        public string type { get; set; }
        public List<string> @enum { get; set; }
    }

    public class CompValue
    {
        public string description { get; set; }
        public List<string> type { get; set; }
    }

    public class Condition
    {
        public string description { get; set; }
        public string type { get; set; }
        public Properties properties { get; set; }
        public List<string> required { get; set; }
    }

    public class Defs2
    {
        public Effect effect { get; set; }
        public Condition condition { get; set; }
        public Option option { get; set; }
    }

    public class Option
    {
        public string description { get; set; }
        public string type { get; set; }
        public Properties properties { get; set; }
        public List<string> required { get; set; }

    }

    [System.Serializable]
    public class Root
    {
        public string Schema { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public Properties properties { get; set; }
        public List<string> required { get; set; }

        public static Root CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Root>(jsonString);
        }

    }

}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}

