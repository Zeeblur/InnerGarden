using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject[] otherObjects;

    public Dropdown resDropdown;

    Resolution[] resolutions;

    public CanvasScaler canvasScaler;
    public Slider scaleSlider;
    
    public Vector2 initialRefResolution;
    public Vector2 currentRefRes;
    public Vector2 currentRes;

    // SavedPlayerSettings
    public float UIScale = 1.0f;

    void OnEnable()
    {
        UIScale = PlayerPrefs.GetFloat("UIScale");
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat("UIScale", UIScale);
    }

    void Start()
    {
        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();

        List<string> dropData = new List<string>();
        foreach (Resolution res in resolutions)
        {
            print(res.ToString());
            dropData.Add(res.ToString());
        }
        resDropdown.AddOptions(dropData);

        initialRefResolution = canvasScaler.referenceResolution;
        currentRes = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
    }

    public void OptionsOpen()
    {
        optionsPanel.SetActive(true);

        // toggle buttons
        ShowOthers(false);
    }

    public void Update()
    {
        //RectTransform rt = optionsPanel.GetComponent<RectTransform>();
        //print(rt.localPosition);
    }

    public void OptionsClose()
    {
        optionsPanel.SetActive(false);
        ShowOthers(true);
    }

    public void ShowOthers(bool toHide)
    {
        foreach (GameObject go in otherObjects)
        {
            go.SetActive(toHide);
        }
    }

    public void SetVolume(float vol)
    {

    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution()
    {

    }

    public void SetUIScale(float input)
    {
        UIScale = input;
        print(input);
        currentRefRes = initialRefResolution / UIScale;
        canvasScaler.referenceResolution = currentRefRes;
    }
}