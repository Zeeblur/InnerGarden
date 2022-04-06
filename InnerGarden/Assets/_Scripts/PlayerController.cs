using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hold Right mouse button to look up/down, use left/right to spin around centre. After set seconds press space to continue
public class PlayerController : MonoBehaviour
{
    public Transform playerCamera = null;
    public float horizontalSensitivity = 3.5f;
    public float verticalSensitivity = 1.5f;

    public float secondsUntilPrompt = 10.0f;
    private Vector3 dragOrigin;

    public CanvasGroup storyPrompt;
    private bool showStoryPrompt = false;

    public GameObject target;
    private Vector3 startUp;

    // Start is called before the first frame update
    void Start()
    {
        startUp = Vector3.up;
        // awake in the garden?
        StartCoroutine(CountdownSeconds());
    }

    private IEnumerator CountdownSeconds()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsUntilPrompt); //wait 10 seconds
            showStoryPrompt = true;
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseLook();

        if (showStoryPrompt)
        {
            storyPrompt.alpha = 1;
            
            // any key except mouse
            if (Input.anyKey && !(Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)))
            {
                GameManager.LeaveGarden();
                Debug.Log("Leaving Garden");
            }
        }
    }

    void UpdateMouseLook()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
        }

        // horizontal rotation around centre
        if (!Input.GetMouseButton(1))
        {
            transform.RotateAround(target.transform.position, Vector3.up, mouseDelta.x * horizontalSensitivity);
            return;
        }

        // up and down 
        Vector3 pos = playerCamera.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        playerCamera.eulerAngles += verticalSensitivity * new Vector3(-pos.y, 0, 0);
    }

}

