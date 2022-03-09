using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntoGarden : MonoBehaviour
{
    public void EnterGarden()
    {
        SceneManager.LoadScene("Garden");
    }
}
