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
    [SerializeField] private bool kickBackup;
    [Range(1,4)]
    [SerializeField] private int playerFaker = 1;

    public List<GameObject> players = new List<GameObject>();

    public bool playersJoined;

    public static BackupKicker Instance { get; private set; }
    public bool initialized { get; private set; }

    private bool joiningPlayers = false;

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

    public int playersActive = 0;

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
        if (GameController.Instance == null)
        {
            GameObject controller = new GameObject("Game Controller Backup Kicker");
            controller.AddComponent<GameController>();
        }

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
            for (int i = 0; i < 2; i++)
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
        Debug.LogError("Hello");

        if (GameController.Instance == null)
        {
            GameObject controller = new GameObject("Game Controller Backup Kicker");
            controller.AddComponent<GameController>();
        }

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

        if (GUILayout.Button("Start Player Joining System"))
        {
            if (Application.isPlaying)
            {
                target.GetComponent<BackupKicker>().start();
            }
            //Debug.Log("It's alive: " + target);

        }

        if (GUILayout.Button("Initialize Backup Generator"))
        {
            if (Application.isPlaying)
            {
                target.GetComponent<BackupKicker>().startBig();
            }
            //Debug.Log("It's alive: " + target);

        }

        int players = target.GetComponent<BackupKicker>().playersActive;
        GUILayout.Label("Current Have: " + players + "players joined");

    }


    [TextArea(10, 1000)]
    public string Comment = "Information Here.";
}