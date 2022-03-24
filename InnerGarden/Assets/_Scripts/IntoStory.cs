using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntoStory : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("Story");
    }
}
