using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public List<GameObject> otherObjects;

    public Dropdown resDropdown;

    Resolution[] resolutions;

    public CanvasScaler canvasScaler;
    public Slider scaleSlider;
    
    public Vector2 initialRefResolution;
    public Vector2 currentRefRes;

    // SavedPlayerSettings
    public float UIScale = 1.0f;
    public Vector2 referenceRes;
    public bool fsToggle;
    public Vector2 currentRes;

    void OnEnable()
    {
        UIScale = PlayerPrefs.GetFloat("UIScale");
        referenceRes = new Vector2(PlayerPrefs.GetFloat("RefRes_X"), PlayerPrefs.GetFloat("RefRes_Y"));
        fsToggle = PlayerPrefs.GetInt("IsFullscreen") == 1 ? true : false;
        currentRes = new Vector2(PlayerPrefs.GetFloat("CurRes_X"), PlayerPrefs.GetFloat("CurRes_Y"));
    }

    void OnDisable()
    {
        print("Options says OnDisable");
        PlayerPrefs.SetFloat("UIScale", UIScale);
        PlayerPrefs.SetFloat("RefRes_X", referenceRes.x);
        PlayerPrefs.SetFloat("RefRes_Y", referenceRes.y);
        PlayerPrefs.SetInt("IsFullscreen", fsToggle ? 1 : 0);
        PlayerPrefs.SetFloat("CurRes_X", currentRes.x);
        PlayerPrefs.SetFloat("CurRes_Y", currentRes.y);
    }

    void Start()
    {
        resolutions = Screen.resolutions;
        currentRes = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        resDropdown.ClearOptions();

        List<string> dropData = new List<string>();
        int currentResIdx = 0;
        for (int i = 0; i < resolutions.Length; ++i)
        {
            print(resolutions[i].ToString());
            dropData.Add(resolutions[i].ToString());

            if (resolutions[i].width == currentRes.x && resolutions[i].height == currentRes.y)
            {
                currentResIdx = i;
            }
                
        }
        resDropdown.AddOptions(dropData);
        resDropdown.value = currentResIdx;

        initialRefResolution = canvasScaler.referenceResolution;
    }

    public void OptionsOpen()
    {
        optionsPanel.SetActive(true);

        // toggle buttons
        ShowOthers(false);
    }

    public void OptionsClose()
    {
        optionsPanel.SetActive(false);
        ShowOthers(true);
        GameManager.Resume();
    }

    public void ShowOthers(bool toHide)
    {
        foreach (GameObject go in otherObjects)
        {
            if (go)
               go.SetActive(toHide);
        }
    }

    public void SetVolume(float vol)
    {

    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        fsToggle = isFullscreen; // save to prefs
    }

    public void SetResolution(int resIdx)
    {
        Resolution res = resolutions[resIdx];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetUIScale(float input)
    {
        UIScale = input;
        print(input);
        currentRefRes = initialRefResolution / UIScale;
        canvasScaler.referenceResolution = currentRefRes;
        referenceRes = currentRefRes;
    }
}