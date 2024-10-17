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

    private List<GameObject> players = new List<GameObject>();

    [HideInInspector] public bool LockKicker = false;

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

    public void startJoiningPlayers()
    {
        PlayerInputManager playerInputManager = FindAnyObjectByType<PlayerInputManager>();
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
        PlayerDataManager.Instance.AddPlayer(input);
        players.Add(input.transform.parent.gameObject);
    }

    private void PlayerDataManagerInitialize()
    {
        if (PlayerDataManager.Instance == null)
        {
            //List<GameObject> players = new List<GameObject>();

            GameObject controller = new GameObject("Player Data Manager Backup Kicker");
            PlayerDataManager data = controller.AddComponent<PlayerDataManager>();
            GameObject playerPrefab = FindAnyObjectByType<PlayerInputManager>().playerPrefab;
        }
    }

    private void GameControllerInitialize()
    {
        foreach (var player in players)
        {
            Destroy(player);
        }

        if (GameController.Instance == null)
        {
            GameObject controller = new GameObject("Game Controller Backup Kicker");
            controller.AddComponent<GameController>();
        }

        LockKicker = true;
        Debug.LogWarning("Backup Kicked In Systems Starting...");
    }

    public void start()
    {
        Debug.Log("Howdy Neighbour");
        if(playerFaker == 1)
        {
            StartCoroutine(initializeCoroutine());
        }
        else
        {
            PlayerDataManagerInitialize();
            startJoiningPlayers();
        }
        //LogAllDevices();
        //initialize();
    }

    public void startBig()
    {
        GameControllerInitialize();
    }

    string[] controlSchemes = { "Keyboard", "Controller", "Controller", "Controller" };

    private void LogAllDevices()
    {
        // Get all devices connected
        var devices = InputSystem.devices;

        // Log each device's name and type
        foreach (var device in devices)
        {
            Debug.Log($"Device: {device.displayName}, Type: {device.GetType().Name}");
        }
    }

    private IEnumerator initializeCoroutine()
    {
        yield return new WaitForSeconds(.1f);
        if (PlayerDataManager.Instance == null)
        {
            //List<GameObject> players = new List<GameObject>();

            GameObject controller = new GameObject("Player Data Manager Backup Kicker");
            PlayerDataManager data = controller.AddComponent<PlayerDataManager>();
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
            Debug.Log("Out 0");
            for (int i = 0; i < 1; i++)
            {
                GameObject fakePlayer = Instantiate(playerPrefab);
                PlayerInput fakeInput = fakePlayer.GetComponentInChildren<PlayerInput>();
                //fakeInput.playerIndex = i;
                //fakeInput.SwitchCurrentActionMap(controlSchemesInt[i]);
                fakeInput.SwitchCurrentControlScheme(controlSchemes[i], Keyboard.current, Mouse.current);

                //fakeInput.devi = InputSystem.devices[i];
                //data.AddPlayer(fakeInput);
                //Destroy(fakeInput.gameObject);
                PlayerDataManager.Instance.AddPlayer(fakeInput);
                Destroy(fakePlayer);
                Debug.Log($"Player {i} instantiated with PlayerInput index: {fakeInput.playerIndex} and is using {fakeInput.currentControlScheme}");
                //players.Add(fakePlayer);
            }

            Debug.Log("Out 1");

            /*            while(players.Count != 0)
                        {
                            data.AddPlayer(players[0].GetComponentInChildren<PlayerInput>());
                            Destroy(player);
                        }

                        int tempI = players.Count;
                        for (int i = 0;i < players.Count; i++)
                        {

                        }*/

            /*            foreach (GameObject player in players)
                        {
                            data.AddPlayer(player.GetComponentInChildren<PlayerInput>());
                            Destroy(player);
                        }*/


            Debug.Log("Out 2");
            Debug.LogError("Pause");
            /*
                        foreach(PlayerData playerData in data.GetPlayers())
                        {
                            Destroy(playerData.GetPlayerInScene());
                        }*/

            /*            GameObject fakePlayer = Instantiate(playerPrefab);
                        PlayerInput fakeInput = fakePlayer.GetComponentInChildren<PlayerInput>();
                        data.AddPlayer(fakeInput);*/

            //Destroy(fakePlayer);
        }
        //Debug.LogError("Hello");

        if (GameController.Instance == null)
        {
            GameObject controller = new GameObject("Game Controller Backup Kicker");
            controller.AddComponent<GameController>();
        }

        LockKicker = true;
        Debug.LogWarning("Backup Kicked In Systems Starting...");
    }

    private async Task initialize()
    {
        if(PlayerDataManager.Instance == null)
        {
            //List<GameObject> players = new List<GameObject>();

            GameObject controller = new GameObject("Player Data Manager Backup Kicker");
            PlayerDataManager data = controller.AddComponent<PlayerDataManager>();
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

            await Task.Delay(3);
            Debug.Log("Out 0");
            for (int i = 0; i < 2; i++)
            {
                GameObject fakePlayer = Instantiate(playerPrefab);
                PlayerInput fakeInput = fakePlayer.GetComponentInChildren<PlayerInput>();
                //fakeInput.playerIndex = i;
                //fakeInput.SwitchCurrentActionMap(controlSchemesInt[i]);
                fakeInput.SwitchCurrentControlScheme(controlSchemes[i]);
                //data.AddPlayer(fakeInput);
                //Destroy(fakeInput.gameObject);
                PlayerDataManager.Instance.AddPlayer(fakeInput);
                Destroy(fakePlayer);
                Debug.Log($"Player {i} instantiated with PlayerInput index: {fakeInput.playerIndex} and is using {fakeInput.currentControlScheme}");
                //players.Add(fakePlayer);
            }
            await Task.Delay(1);
            Debug.Log("Out 1");

/*            while(players.Count != 0)
            {
                data.AddPlayer(players[0].GetComponentInChildren<PlayerInput>());
                Destroy(player);
            }

            int tempI = players.Count;
            for (int i = 0;i < players.Count; i++)
            {

            }*/

/*            foreach (GameObject player in players)
            {
                data.AddPlayer(player.GetComponentInChildren<PlayerInput>());
                Destroy(player);
            }*/


            Debug.Log("Out 2");
            Debug.LogError("Pause");
/*
            foreach(PlayerData playerData in data.GetPlayers())
            {
                Destroy(playerData.GetPlayerInScene());
            }*/

/*            GameObject fakePlayer = Instantiate(playerPrefab);
            PlayerInput fakeInput = fakePlayer.GetComponentInChildren<PlayerInput>();
            data.AddPlayer(fakeInput);*/

            //Destroy(fakePlayer);
        }
        Debug.LogError("Hello");

        if(GameController.Instance == null)
        {
            GameObject controller = new GameObject("Game Controller Backup Kicker");
            controller.AddComponent<GameController>();
        }

        Debug.LogWarning("Backup Kicked In Systems Starting...");
    }
}

[CustomEditor(typeof(BackupKicker))]
class BackupKickerEditor : Editor
{
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
        
        if(target.GetComponent<BackupKicker>().LockKicker == true)
        {
            if(target.GetComponent<BackupKicker>().playerFaker > 1)
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
        }

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


    [TextArea(10, 1000)]
    public string Comment = "Information Here.";
}
