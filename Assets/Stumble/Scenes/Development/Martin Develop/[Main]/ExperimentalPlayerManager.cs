using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class ExperimentalPlayerManager : MonoBehaviour
{
    //private List<PlayerInput> players = new List<PlayerInput>();
    //[SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<LayerMask> playerLayers;

    // Core Controller
    private GameController gameController;

    private PlayerInputManager playerInputManager;
    private PlayerDataManager playerDataManager;

/*    [HideInInspector]*/
    //public UnityEvent OnAllPlayersBroughtInSpawned;


    [Header("Settings")]
    //[SerializeField] public SceneCameraType camType;
    //[SerializeField] private SceneCameraType cameraType;
    [SerializeField] private bool cursorEnabled = false;                                            // Enable Cursor (Recommend to leave disabled)
    [SerializeField] private bool kickPlayerOnDisconntect = false;                                  // Kick players when they leave from the game
    [SerializeField] private SceneCameraType sceneCameraType;

    // UI
    public InputSystemUIInputModule UIEventSystem;

    private Dictionary<InputDevice, PlayerInput> deviceSaver = new Dictionary<InputDevice, PlayerInput>(); // Dictionary that holds player devices in case of kicking enabled

    private bool bringingPlayersOver = false;

    private void Awake()
    {
        // Get the Player Input Manager Unique tu each scene
        playerInputManager = GetComponent<PlayerInputManager>();
        Debug.Log("================= " + SceneManager.GetActiveScene().name + " =================");
    }

    private void Start()
    {
        // [!] Note: Anything using the gameController has to be
        // in start. If added to OnEnbale will give error
        gameController.startSystems += LateStart;
    }

    private void OnEnable()
    {
        // Events
        gameController = GameController.Instance;
        playerInputManager.onPlayerLeft += RemovePlayer;

        playerDataManager = FindAnyObjectByType<PlayerDataManager>();
        // If the player Data Manager Exists, Rely on that
        // Before spawning the players
/*        if (playerDataManager != null)
        {
            // Player Data Manager
            playerDataManager.onPlayerAdded += AddPlayer;
            Debug.LogWarning("Subscribed to Data Manager");
        }
        else
        {
            // Player Input Manager
            Debug.LogWarning("Using player Input Manager");
        }*/
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        gameController.startSystems -= LateStart;
        //fuck michael - Angel 10/09/2024
/*        if (playerDataManager != null)
        {
            playerDataManager.onPlayerAdded -= AddPlayer;
        }
        else
        {
            playerInputManager.onPlayerJoined -= AddPlayer;
        }*/

        playerInputManager.onPlayerJoined -= AddPlayer;
        playerInputManager.onPlayerLeft -= RemovePlayer;
    }

    private void LateStart()
    {
        InitializePlayers();
    }

    private void InitializePlayers()
    {
        if(GameController.Instance != null) {
            //Debug.Log(GameController.Instance.gameState);
            if (gameController.gameState == GameState.Lobby || gameController.gameState == GameState.StartScreen)
            {
                playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
            }
        }

        // If Player Data Manager Exists
        if(playerDataManager != null)
        {
            //Debug.LogWarning("Player Data Manager was found using manual player join.");

            bringingPlayersOver = true;

            // Spawn Players Already In the Player Data Manager
            int players = playerDataManager.players.Count;
            Debug.Log("Spawning" + players + " players in the scene");
            for (int i = 0; i < players; i++)
            {
                int playersInGame = i;
                string playerControlScheme = playerDataManager.GetPlayerData(i).input.currentControlScheme;
                InputDevice playerDevice = playerDataManager.GetPlayerData(i).device;
                //playerInputManager.JoinPlayer(i, i - playerDataManager.GetPlayers().Count, playerControlScheme, playerDevice);
                playerInputManager.JoinPlayer(i, i - playerDataManager.GetPlayers().Count, playerControlScheme, playerDevice);
                Debug.Log("Spawning a new Player " + i);
            }

            bringingPlayersOver = false;
            Debug.Log("Added all players brought");
            //OnAllPlayersBroughtInSpawned?.Invoke();
        }
        else // Not Player Data Manager
        {
            //Debug.LogWarning("No player data manager was found, using automatic player join");
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
        playerDataManager = PlayerDataManager.Instance;
        if (playerDataManager != null)
        {
            // Not spawning players from another Scene
            if (!bringingPlayersOver)
            {
                // Add to data
                playerDataManager.AddPlayer(player);
            }

            playerDataManager.GetPlayerData(playerID).SetPlayerInput(player);
            playerDataManager.GetPlayerData(playerID).SetPlayerInScene(player.transform.gameObject);

            // Check if playerPrefab has a MeshRenderer - Meaning its a player with model
            if (player.gameObject.GetComponent<PlayerCosmetics>() != null)
            {
                //Debug.Log("Player has a PlayerCosmetics Item");
                // Add Cosmetic [Prototype]
                if (GameController.Instance.gameState == GameState.Lobby) {
                    // Set the default cosmetic if the scene is Lobby
                    CosmeticManager.Instance.setDefaultCosmetic(playerDataManager.GetPlayerData(player));
                }
                else
                {
                    // Set the chosen cosmetic
                    //player.gameObject.GetComponentInChildren<MeshRenderer>().material = playerDataManager.GetPlayerData(player).cosmeticData.GetMaterialPicked();
                    //Debug.LogWarning("Apply Cosmetic to Player");
                    player.GetComponent<PlayerCosmetics>().body.GetComponent<SkinnedMeshRenderer>().material = playerDataManager.GetPlayerData(playerID).GetCosmeticData().colorPicked;
                    player.GetComponent<PlayerCosmetics>().eyes.GetComponent<SkinnedMeshRenderer>().material = playerDataManager.GetPlayerData(playerID).GetCosmeticData().colorPicked;
                    player.GetComponent<PlayerCosmetics>().fins.GetComponent<SkinnedMeshRenderer>().material = playerDataManager.GetPlayerData(playerID).GetCosmeticData().colorPicked;

                    // Spawn Hat
                    if(playerDataManager.GetPlayerData(playerID).GetCosmeticData().hatPrefab != null)
                    {
                        Instantiate(playerDataManager.GetPlayerData(playerID).GetCosmeticData().hatPrefab, player.GetComponent<PlayerCosmetics>().hatPos);
                    }

                    // Spawn Boots
                    // Right Boot
                    if(playerDataManager.GetPlayerData(playerID).GetCosmeticData().rightBootPrefab != null)
                    {
                        Instantiate(playerDataManager.GetPlayerData(playerID).GetCosmeticData().rightBootPrefab, player.GetComponent<PlayerCosmetics>().rightFoot);
                    }
                    // Left Boot
                    if(playerDataManager.GetPlayerData(playerID).GetCosmeticData().leftBootPrefab != null)
                    {
                        Instantiate(playerDataManager.GetPlayerData(playerID).GetCosmeticData().leftBootPrefab, player.GetComponent<PlayerCosmetics>().leftFoot);
                    }
                }
            }
            else
            {
                //Debug.LogWarning("No PlayerCosmetics Item Found");
            }
        }

        // Need to use the paren due to the structure of the prefab
        Transform playerParent = player.transform.parent;
        //playerParent.position = spawnPoints[playerInputManager.playerCount - 1].position;
        playerParent.name = "Player #" + (playerID);
        playerObject.name = "Player #" + (playerID) + " controller";
        //Debug.Log("Setting player #" + (playerInputManager.playerCount - 1) + " to " + spawnPoints[playerInputManager.playerCount - 1].position);
        //Debug.Log("Adding Player");

        switch (sceneCameraType)
        {
            case SceneCameraType.ThirdPersonControl:
                // Convert layer mask (bit) to an integer
                int layerToAdd = (int)Mathf.Log(playerLayers[playerID].value, 2);

                //set the layer
                //player.camera = playerParent.GetComponentInChildren<Camera>();
                playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
                // add the layer
/*                Transform CamPos = playerParent.GetComponentInChildren<Camera>().transform;
                Camera cam = CamPos.GetComponent<Camera>();
                Destroy(cam);
                Camera newCam = CamPos.AddComponent<Camera>();
                newCam = cam;*/

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
        if (playerInputManager.maxPlayerCount == playerInputManager.playerCount)
        {
            playerInputManager.DisableJoining();
            //Debug.LogWarning("Disabled Joining");
        }
    }

    private void RemovePlayer(PlayerInput player)
    {
        // 4 - 1
        if (playerInputManager.maxPlayerCount > playerInputManager.playerCount)
        {
            playerInputManager.EnableJoining();
            //Debug.LogWarning("Enabled Joining");
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


    

}

/// Enums For Customization
public enum SceneCameraType
{
    ThirdPersonControl,
    StaticCamera
}