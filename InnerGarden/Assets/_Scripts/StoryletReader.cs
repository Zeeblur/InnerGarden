using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Data
{
    public class StoryletReader : MonoBehaviour
    {
        public static string jsonStringRead;
        public static string filePath = Application.streamingAssetsPath + "/storylets.json";

        // lets run it in start and hope it's back in time?
        void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log("Wooooo i can't read files");
            GameManager.Instance.StartCoroutine(GetRequest(filePath, (value =>
            {
                jsonStringRead = value;
                Debug.Log("Printing: "+ value);
            })));
            
#endif
        }

        // have to use a coroutine for async web request to read the json file
        static IEnumerator GetRequest(string uri, System.Action<string> callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                        // add to value here
                        jsonStringRead = webRequest.downloadHandler.text;
                        break;
                }
            }
        }

        public static RootObject ReadData()
        {
            string jsonString = "";

#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log(jsonStringRead);
            jsonString = jsonStringRead;
#else
            jsonString = File.ReadAllText(filePath);
#endif
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

