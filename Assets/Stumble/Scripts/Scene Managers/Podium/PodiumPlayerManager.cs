using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PodiumPlayerManager : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();
    //[SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<LayerMask> playerLayers;

    [Header("Cosmetic")]

    public PlayerInputManager playerInputManager;
    private PlayerDataManager playerDataManager;

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerDataManager = PlayerDataManager.Instance;

        playerInputManager.onPlayerJoined += AddPlayer;
        //playerDataManager.onPlayerInputDeviceDisconnect += OnPlayerInputDisconnected;
        playerDataManager.onPlayerInputDeviceReconnect += OnPlayerInputReconnected;
    }

    private void OnEnable()
    {
        Debug.Log(playerDataManager.GetPlayers().Count);
        for (int i = 0; i < playerDataManager.GetPlayers().Count; i++)
        {
            //int playersInGame = playerInputManager.playerCount;
            int playersInGame = playerDataManager.GetPlayersWithInGameCharacter();
            Debug.Log(playersInGame);
            //Debug.Log("The current player count is " + playersInGame);
            string playerControlScheme = playerDataManager.GetPlayerData(playersInGame).input.currentControlScheme;
            InputDevice playerDevice = playerDataManager.GetPlayerData(playersInGame).device;
            playerInputManager.JoinPlayer(playersInGame - i, 0 - i, playerControlScheme, playerDevice);
            //Debug.Log("Split Screen Index for " + i + " is " + playersInGame);
        }
        Debug.Log(playerDataManager.GetPlayers().Count);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;

        //playerDataManager.onPlayerInputDeviceDisconnect -= OnPlayerInputDisconnected;
        playerDataManager.onPlayerInputDeviceReconnect -= OnPlayerInputReconnected;
    }

    private void Start()
    {
        /*        for (int i = 0; i < playerDataManager.GetPlayers().Count; i++)
                {
                    int playersInGame = playerInputManager.playerCount;
                    //Debug.Log("The current player count is " + playersInGame);
                    string playerControlScheme = playerDataManager.GetPlayerData(playersInGame).input.currentControlScheme;
                    InputDevice playerDevice = playerDataManager.GetPlayerData(playersInGame).device;
                    playerInputManager.JoinPlayer(playersInGame, playersInGame - playerDataManager.GetPlayers().Count, playerControlScheme, playerDevice);
                    //Debug.Log("Split Screen Index for " + i + " is " + playersInGame);
                }*/

        //CheckpointManager.Instance.initializeCheckpoints();
    }

    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);

        // Player Data Manager
        //playerDataManager.GetPlayerData(player).SetPlayerInScene(player.gameObject);
        //int playerID = playerDataManager.GetPlayers().Count;
        int playerID = playerDataManager.GetPlayersWithInGameCharacter();
        //Debug.Log("Current Player Count: " +  playerID);
        //Debug.Log("Current Player on Input Count: " +  playerInputManager.playerCount);
        //Debug.Log("Current Player on New Count: " +  playerDataManager.GetPlayersWithInGameCharacter());

        playerDataManager.GetPlayerData(playerID).SetPlayerInput(player);
        playerDataManager.GetPlayerData(playerID).SetPlayerInScene(player.transform.gameObject);

        // Need to use the paren due to the structure of the prefab
        Transform playerParent = player.transform.parent;
        //playerParent.position = spawnPoints[playerInputManager.playerCount - 1].position;
        playerParent.name = "Player #" + (playerID);
        //Debug.Log("Setting player #" + (playerInputManager.playerCount - 1) + " to " + spawnPoints[playerInputManager.playerCount - 1].position);

        // Convert layer mask (bit) to an integer
        //int layerToAdd = (int)Mathf.Log(playerLayers[playerID].value, 2);

        //set the layer
        //playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
        // add the layer
        //playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;

        //player.camera = Camera.main;
        //player.transform.GetComponent<StaticPlayerMovement>().cam = Camera.main.transform;
        // set the action in the custom cinemachine Input handler
        //playerParent.GetComponentInChildren<InputHandler>().horizontal = player.actions.FindAction("Look");

        // Add Cosmetic [Prototype]
        player.gameObject.GetComponentInChildren<MeshRenderer>().material = playerDataManager.GetPlayerData(player).cosmeticData.colorPicked;

        //Check for player Count
        //Debug.Log(players.Count);

    }

    // Game Pause Disconnection Management
    private List<int> playerDisconnectIDs = new List<int>();
    private void OnPlayerInputDisconnected(PlayerData data)
    {
        playerDisconnectIDs.Add(data.id);

        if (playerDisconnectIDs.Count > 0)
        {
            // Freeze Time
            Time.timeScale = 0;
        }
    }

    private void OnPlayerInputReconnected(PlayerData data)
    {
        playerDisconnectIDs.Remove(data.GetID());

        if (playerDisconnectIDs.Count == 0)
        {
            Time.timeScale += 1;
        }
    }
}
