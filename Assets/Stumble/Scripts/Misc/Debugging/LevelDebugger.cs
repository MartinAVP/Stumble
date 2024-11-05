using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class LevelDebugger : MonoBehaviour
{
    private GUIStyle buttonStyle;
    private bool hasCursor = false;
    //private bool isRaceMode = false;

    private bool hasGravity = true;
    //[SerializeField] private bool isDebugging = true;

/*    private void Update()
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
    }*/
/*    void OnGUI()
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
    }*/

    public void GoToNextCheckpoint()
    {
        CheckpointManager.Instance.ForceReachNextCheckpoint(PlayerDataManager.Instance.GetPlayerData(0));
    }

    public void ToggleGravity()
    {
        hasGravity = !hasGravity;
        foreach(PlayerData data in PlayerDataManager.Instance.GetPlayers())
        {
            data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().lockVeritcalMovement = !hasGravity;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(LevelDebugger))]
    [CustomPropertyDrawer(typeof(LevelDebugger))]
    class LevelDebuggerEditor : Editor
    {
        private bool displayDocumentation = false;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            /*        if (GUILayout.Button("Start Player Joining System"))
                    {
                        if (Application.isPlaying)
                        {
                            target.GetComponent<BackupKicker>().start();
                        }
                        //Debug.Log("It's alive: " + target);

                    }*/
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Toggle Gravity"))
            {
                if (Application.isPlaying)
                {
                    target.GetComponent<LevelDebugger>().ToggleGravity();
                }
                //Debug.Log("It's alive: " + target);

            }
            if (GUILayout.Button("Skip Checkpoint"))
            {
                if (Application.isPlaying)
                {
                    target.GetComponent<LevelDebugger>().GoToNextCheckpoint();
                }
                //Debug.Log("It's alive: " + target);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("");
            if (!displayDocumentation)
            {
                if (GUILayout.Button("Show Docs"))
                {
                    //Debug.Log("It's alive: " + target);
                    displayDocumentation = !displayDocumentation;
                }
            }
            else
            {
                if (GUILayout.Button("Hide Docs"))
                {
                    //Debug.Log("It's alive: " + target);
                    displayDocumentation = !displayDocumentation;
                }
            }

            if (displayDocumentation)
            {
                DisplayDocs();
            }

            /*            if (target.GetComponent<BackupKicker>().LockKicker == true)
                        {
                            if (target.GetComponent<BackupKicker>().playerFaker > 1)
                            {
                                if (GUILayout.Button("Start Player Joining"))
                                {
                                    if (Application.isPlaying)
                                    {
                                        target.GetComponent<BackupKicker>().start();
                                    }
                                    //Debug.Log("It's alive: " + target);

                                }

                                if (GUILayout.Button("Start Game Scene"))
                                {
                                    if (Application.isPlaying)
                                    {
                                        target.GetComponent<BackupKicker>().startBig();
                                    }
                                    //Debug.Log("It's alive: " + target);

                                }
                                int players = target.GetComponent<BackupKicker>().playersActive;
                                GUILayout.Label("Debug Information");
                                GUILayout.Label("Current Have: " + players + " players joined");

                            }
                            else
                            {
                                if (GUILayout.Button("Start Single Player"))
                                {
                                    if (Application.isPlaying)
                                    {
                                        target.GetComponent<BackupKicker>().start();
                                    }
                                    //Debug.Log("It's alive: " + target);

                                }
                            }
                        }*/




        }

        private void DisplayDocs()
        {
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;

            GUIStyle header1Style = new GUIStyle(GUI.skin.label);
            header1Style.fontSize = 14;

            GUIStyle normalStyle = new GUIStyle(GUI.skin.label);
            normalStyle.fontSize = 10;

            // Use the styles in your labels

            GUILayout.Label("", normalStyle);
            GUILayout.Label("How to use the Level Debugger", titleStyle);
            GUILayout.Label("", normalStyle);
            GUILayout.Label("What is the Level Debugger?", header1Style);
            GUILayout.Label("The Level Debugger is a tool that will allow you to have some", normalStyle);
            GUILayout.Label("special abilities to test the current level. The Level Debugger", normalStyle);
            GUILayout.Label("is meant to work on race and arena scenes.", normalStyle);
            GUILayout.Label("", normalStyle);

            GUILayout.Label("Toggle Gravity:", header1Style);
            GUILayout.Label("It will toggle gravity to all players, players will not be", normalStyle);
            GUILayout.Label("able to go up or down, just horizontally with no gravity.", normalStyle);
            GUILayout.Label("", normalStyle);
            GUILayout.Label("Skip Checkpoint:", header1Style);
            GUILayout.Label("This will skip to the next checkpoint only for player #1, once ", normalStyle);
            GUILayout.Label("player #1 reaches the finish line, then it will no longer work.", normalStyle);
            GUILayout.Label("", normalStyle);
        }

        [TextArea(10, 1000)]
        public string Comment = "Information Here.";
    }
#endif
}
