using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Data
{
    public class StoryletReader
    {
        public static RootObject ReadData()
        {
            string filePath = Application.dataPath + "/Storylets/storylets.json";
            string jsonString = File.ReadAllText(filePath);

            RootObject theStories = JsonUtility.FromJson<RootObject>("{\"stories\":" + jsonString + "}");
            return theStories;
        }

        [System.Serializable]
        public class StoryletOption
        {
            public string optionText;
            public string optionArchetype;
        }

        [System.Serializable]
        public class Storylet
        {
            public int storyletID;
            public string storyletPic;
            public string storyletText;
            public StoryletOption[] storyletOptions;
        }

        [System.Serializable]
        public class RootObject
        {
            public Storylet[] stories;
        }
    }
}

