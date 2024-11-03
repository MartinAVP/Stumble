using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BackupKicker : MonoBehaviour
{
    [Range(1,4)]
    [SerializeField] public int playerFaker = 1;

    public List<GameObject> players = new List<GameObject>();

    private PlayerDataHolder playerDataHolder;
    private PlayerInputManager playerInputManager;

    [HideInInspector] public bool LockKicker = false;
    [HideInInspector] public bool joiningPlayers = false;

    public static BackupKicker Instance { get; private set; }
    public bool initialized { get; private set; }



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    [HideInInspector] public int playersActive = 0;

    private void Start()
    {
        playerInputManager = FindAnyObjectByType<PlayerInputManager>();
    }

    public void StartSinglePlayer()
    {
        //Debug.Log("Howdy Neighbour");
        //PlayerDataManagerInitialize();
        startJoiningPlayers();
        StartCoroutine(InitializeSinglePlayer());
        //LogAllDevices();
        //initialize();
    }

    public void StartMultiplayer()
    {
        joiningPlayers = true;
        PlayerDataManagerInitialize();
        startJoiningPlayers();
    }

    public void startJoiningPlayers()
    {
        playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
        playerInputManager.EnableJoining();

        playerInputManager.onPlayerJoined += AddingPlayers;
    }
    public void endJoiningPlayers()
    {
        PlayerInputManager playerInputManager = FindAnyObjectByType<PlayerInputManager>();
        playerInputManager.onPlayerJoined -= AddingPlayers;
    }
    private void AddingPlayers(PlayerInput input)
    {
        playersActive++;
        PlayerDataHolder.Instance.AddPlayer(input);
        players.Add(input.transform.parent.gameObject);
    }

    private void PlayerDataManagerInitialize()
    {
        if (PlayerDataHolder.Instance == null)
        {
            //List<GameObject> players = new List<GameObject>();

            GameObject controller = new GameObject("Player Data Manager Backup Kicker");
            PlayerDataHolder data = controller.AddComponent<PlayerDataHolder>();
            GameObject playerPrefab = FindAnyObjectByType<PlayerInputManager>().playerPrefab;
        }
    }

    private void FinalizeInitialization()
    {
        joiningPlayers = false;

        endJoiningPlayers();
        foreach (var player in players)
        {
            Destroy(player);
        }

        players.Clear();

        if (GameController.Instance == null)
        {
            GameObject controller = new GameObject("Game Controller Backup Kicker");
            controller.AddComponent<GameController>();
            controller.AddComponent<AudioListener>();
        }

        LockKicker = true;
        Debug.LogWarning("Backup Kicked In Systems Starting...");
    }

    public void StartGameScene()
    {
        StartCoroutine(InitializeMultiplayer());
        FinalizeInitialization();
    }

    string[] controlSchemes = { "Keyboard", "Controller", "Controller", "Controller" };

/*    private void LogAllDevices()
    {
        // Get all devices connected
        var devices = InputSystem.devices;

        // Log each device's name and type
        foreach (var device in devices)
        {
            Debug.Log($"Device: {device.displayName}, Type: {device.GetType().Name}");
        }
    }
*/
    private IEnumerator InitializeMultiplayer()
    {
        yield return new WaitForSeconds(.1f);

        //List<GameObject> players = new List<GameObject>();

        GameObject controller = new GameObject("Player Data Holder Backup Kicker");
        PlayerDataHolder data = controller.AddComponent<PlayerDataHolder>();
        playerDataHolder = data;
        GameObject playerPrefab = FindAnyObjectByType<PlayerInputManager>().playerPrefab;

        //Debug.Log(players.Length);
        /*            for (int i = 0; i < 4; i++)
                    {
                        if(i == 0)
                        {
                            GameObject fakePlayer = Instantiate(playerPrefab);
                            PlayerInput fakeInput = fakePlayer.GetComponentInChildren<PlayerInput>();
                            data.AddPlayer(fakeInput);
                        }
                        //players.Add(fakePlayer);
                        //players[i] = fakePlayer;
                    }

                    foreach (GameObject obj in players)
                    {
                        Destroy(obj);
                    }
                    // Optionally clear the list if you no longer need it
                    players.Clear();*/

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        //Debug.Log("Out 0");
        for (int i = 0; i < players.Count; i++)
        {
            GameObject fakePlayer = Instantiate(playerPrefab);
            PlayerInput fakeInput = fakePlayer.GetComponentInChildren<PlayerInput>();
            fakeInput.SwitchCurrentControlScheme(controlSchemes[i], Keyboard.current, Mouse.current);

            //fakeInput.devi = InputSystem.devices[i];
            //data.AddPlayer(fakeInput);
            //Destroy(fakeInput.gameObject);
            //data.AddPlayer(fakeInput);
            Debug.Log("Adding a Player to Player Data Manager");
            Debug.Log(data.GetPlayers().Count);
            yield return new WaitForEndOfFrame();
            //Destroy(fakePlayer);
            Debug.Log($"Player {i} instantiated with PlayerInput index: {fakeInput.playerIndex} and is using {fakeInput.currentControlScheme}");
            //players.Add(fakePlayer);
        }

        yield return new WaitForEndOfFrame();
        FinalizeInitialization();
    }

    private IEnumerator InitializeSinglePlayer()
    {
        yield return new WaitForSeconds(.1f);

        GameObject controller = new GameObject("Player Data Holder Backup Kicker");
        PlayerDataHolder data = controller.AddComponent<PlayerDataHolder>();
        playerDataHolder = data;
        GameObject playerPrefab = FindAnyObjectByType<PlayerInputManager>().playerPrefab;

        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        GameObject fakePlayer = Instantiate(playerPrefab);
        PlayerInput fakeInput = fakePlayer.GetComponentInChildren<PlayerInput>();
        fakeInput.SwitchCurrentControlScheme(controlSchemes[0], Keyboard.current, Mouse.current);

        Debug.Log("Adding a Player to Player Data Manager");
        Debug.Log(data.GetPlayers().Count);
        Debug.Log($"Player {0} instantiated with PlayerInput index: {fakeInput.playerIndex} and is using {fakeInput.currentControlScheme}");

        yield return new WaitForEndOfFrame();

        FinalizeInitialization();
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(BackupKicker))]
class BackupKickerEditor : Editor
{
    private bool displayDocumentation = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BackupKicker kicker = target.GetComponent<BackupKicker>();
/*        if (GUILayout.Button("Start Player Joining System"))
        {
            if (Application.isPlaying)
            {
                target.GetComponent<BackupKicker>().start();
            }
            //Debug.Log("It's alive: " + target);

        }*/
        
        if(kicker.LockKicker)
        {
            if(kicker.playerFaker > 1)
            {
                if (!kicker.joiningPlayers)
                {
                    if (GUILayout.Button("Start Player Joining"))
                    {
                        if (Application.isPlaying)
                        {
                            kicker.StartMultiplayer();
                        }
                        //Debug.Log("It's alive: " + target);

                    }
                }
                else
                {
                    if (GUILayout.Button("Start Game Scene"))
                    {
                        if (Application.isPlaying)
                        {
                            kicker.StartGameScene();
                        }
                        //Debug.Log("It's alive: " + target);

                    }
                }

                int players = kicker.playersActive;
                GUILayout.Label("Debug Information");
                GUILayout.Label("Current Have: " + players + " players joined");

            }
            else
            {
                if (GUILayout.Button("Start Single Player"))
                {
                    if (Application.isPlaying)
                    {
                        kicker.StartSinglePlayer();
                    }
                    //Debug.Log("It's alive: " + target);

                }
            }
        }


        GUILayout.Label("");
        if(!displayDocumentation)
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
        GUILayout.Label("How to use the Backup Kicker", titleStyle);
        GUILayout.Label("", normalStyle);
        GUILayout.Label("For Single Player:", header1Style);
        GUILayout.Label("Put the player Fake on 1 player and hit Start Single Player", normalStyle);
        GUILayout.Label("That will Generate a Fake player for you to test the level", normalStyle);
        GUILayout.Label("Only mouse and Keyboard!, Checkpoints will work.", normalStyle);
        GUILayout.Label("", normalStyle);

        GUILayout.Label("For Multi Player:", header1Style);
        GUILayout.Label("Put the player Fake on the desired player quantity and hit ", normalStyle);
        GUILayout.Label("Start Player Joining, this will start a fake joining system.", normalStyle);
        GUILayout.Label("Press once a button on the input device you want.", normalStyle);
        GUILayout.Label("Once all the players are joined, hit Start Game Scene.", normalStyle);
    }
}

#endif