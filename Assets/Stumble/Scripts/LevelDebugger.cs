using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDebugger : MonoBehaviour
{
    private GUIStyle buttonStyle;
    private bool hasCursor = false;
    private bool isRaceMode = false;

    private bool hasGravity = true;
    [SerializeField] private bool isDebugging = true;

    private void Start()
    {
        if(CheckpointManager.Instance != null)
        {
            isRaceMode = true;
        }
    }

    private void Update()
    {
        if(!isDebugging) { return; }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (hasCursor)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            hasCursor = !hasCursor;
        }
    }

    void OnGUI()
    {
        if (!isDebugging) { return; }
        // Initialize the GUIStyle on the first frame OnGUI is called
        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 25,
                normal = { textColor = Color.white }
            };
        }

        // Define button dimensions
        float buttonWidth = 200;
        float buttonHeight = 50;

        // Button 1
        if (GUI.Button(new Rect(10, 10, buttonWidth, buttonHeight), "Next Checkpoint", buttonStyle))
        {
            GoToNextCheckpoint();
        }
        if (GUI.Button(new Rect(10, 70, buttonWidth, buttonHeight), "Toggle Gravity", buttonStyle))
        {
            ToggleGravity();
        }
    }

    void GoToNextCheckpoint()
    {
        CheckpointManager.Instance.ForceReachNextCheckpoint(PlayerDataManager.Instance.GetPlayerData(0));
    }

    void ToggleGravity()
    {
        hasGravity = !hasGravity;
        foreach(PlayerData data in PlayerDataManager.Instance.GetPlayers())
        {
            data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().lockVeritcalMovement = !hasGravity;
        }
    }
}
