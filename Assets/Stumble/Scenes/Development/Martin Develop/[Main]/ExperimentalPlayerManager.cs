using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class ExperimentalPlayerManager : MonoBehaviour
{
    //private List<PlayerInput> players = new List<PlayerInput>();
    //[SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<LayerMask> playerLayers;

    // Core Controller
    private GameController gameController;

    private PlayerInputManager playerInputManager;
    private PlayerDataManager playerDataManager;


    [Header("Settings")]
    [SerializeField] private SceneCameraType sceneCamera = SceneCameraType.ThirdPersonControl;      // Camera Type for the Scene
    [SerializeField] private bool cursorEnabled = false;                                            // Enable Cursor (Recommend to leave disabled)
    [SerializeField] private bool kickPlayerOnDisconntect = false;                                  // Kick players when they leave from the game

    // UI
    public InputSystemUIInputModule UIEventSystem;
    //public GameObject UIFirstSelected;

    private Dictionary<InputDevice, PlayerInput> deviceSaver = new Dictionary<InputDevice, PlayerInput>(); // Dictionary that holds player devices in case of kicking enabled

    private void Awake()
    {
        // Get the Player Input Manager Unique tu each scene
        playerInputManager = GetComponent<PlayerInputManager>();
        
        playerDataManager = PlayerDataManager.Instance;
    }

    private void Start()
    {
        // [!] Note: Anything using the gameController has to be
        // in start. If added to OnEnbale will give error
        gameController = GameController.Instance;
        gameController.startSystems += initializePlayers;
    }

    private void OnEnable()
    {
        // Events
        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerLeft += RemovePlayer;
    }

    private void OnDisable()
    {
        gameController.startSystems -= initializePlayers;

        playerInputManager.onPlayerJoined -= AddPlayer;
        playerInputManager.onPlayerLeft -= RemovePlayer;
    }

    private void initializePlayers()
    {
        if(GameController.Instance != null) {
            Debug.Log(GameController.Instance.gameState);
            if (gameController.gameState == GameState.Lobby || gameController.gameState == GameState.StartScreen)
            {
                playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenJoinActionIsTriggered;
            }
        }

        // If Player Data Manager Exists
        if(playerDataManager != null)
        {
            Debug.LogWarning("Player Data Manager was found using manual player join.");

            // Spawn Players in the Player Data Manager
            for (int i = 0; i < playerDataManager.GetPlayers().Count; i++)
            {
                int playersInGame = playerInputManager.playerCount;
                string playerControlScheme = playerDataManager.GetPlayerData(playersInGame).input.currentControlScheme;
                InputDevice playerDevice = playerDataManager.GetPlayerData(playersInGame).device;
                playerInputManager.JoinPlayer(playersInGame, playersInGame - playerDataManager.GetPlayers().Count, playerControlScheme, playerDevice);
            }
        }
        else // Not Player Data Manager
        {
            Debug.LogWarning("No player data manager was found, using automatic player join");
            playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenJoinActionIsTriggered;
        }

        if (!cursorEnabled)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            cursorEnabled = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void AddPlayer(PlayerInput player)
    {
        //players.Add(player);
        GameObject playerObject = player.gameObject;
        int playerID = player.playerIndex;

        // If there is a player Data Manager set the variables
        if (playerDataManager != null)
        {
            playerDataManager.GetPlayerData(playerID).SetPlayerInput(player);
            playerDataManager.GetPlayerData(playerID).SetPlayerInScene(player.transform.gameObject);

            // Check if playerPrefab has a MeshRenderer - Meaning its a player with model
            if (player.gameObject.GetComponentInChildren<MeshRenderer>() != null)
            {
                // Add Cosmetic [Prototype]
                player.gameObject.GetComponentInChildren<MeshRenderer>().material = playerDataManager.GetPlayerData(player).cosmeticData.GetMaterialPicked();
            }
        }

        // Need to use the paren due to the structure of the prefab
        Transform playerParent = player.transform.parent;
        //playerParent.position = spawnPoints[playerInputManager.playerCount - 1].position;
        playerParent.name = "Player #" + (playerID);
        playerObject.name = "Player #" + (playerID) + " controller";
        //Debug.Log("Setting player #" + (playerInputManager.playerCount - 1) + " to " + spawnPoints[playerInputManager.playerCount - 1].position);

        switch (sceneCamera)
        {
            case SceneCameraType.ThirdPersonControl:
                // Convert layer mask (bit) to an integer
                int layerToAdd = (int)Mathf.Log(playerLayers[playerID].value, 2);

                //set the layer
                playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
                // add the layer
                playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;

                // Add the Input Handler for camera controll
                playerParent.GetComponentInChildren<InputHandler>().horizontal = player.actions.FindAction("Look");

                break;
            case SceneCameraType.StaticCamera:
                player.camera = Camera.main;
                break;
            default:
                break;
        }

        // Assign Multiplayer Event System to host
        if (playerID == 0)
        {
            if (UIEventSystem != null)
            {
                player.uiInputModule = UIEventSystem;
            }
        }

        // Add to the input saver
        deviceSaver.Add(player.devices[0], player);

        // Disable Joining
        if (playerInputManager.maxPlayerCount >= playerInputManager.playerCount)
        {
            playerInputManager.DisableJoining();
        }
    }

    private void RemovePlayer(PlayerInput player)
    {
        // 4 - 1
        if (playerInputManager.maxPlayerCount <= playerInputManager.playerCount)
        {
            playerInputManager.EnableJoining();
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if(!kickPlayerOnDisconntect) { return; }
        switch (change)
        {
            case InputDeviceChange.Removed:
                if (deviceSaver.TryGetValue(device, out PlayerInput playerInput))
                {
                    deviceSaver.Remove(device);
                    Destroy(playerInput);
                    Debug.Log($"Destroyed GameObject associated with device: {device}");
                }
                break;
        }
    }


    /// Enums For Customization
    private enum SceneCameraType
    {
        ThirdPersonControl,
        StaticCamera
    }
}