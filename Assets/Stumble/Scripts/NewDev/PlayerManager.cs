using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    // == The Master Player Holder ==
    // This class holds every player that is being managed in the game.
    // This class should not be wiped across scenes, it is what carries
    // the players over.
    [SerializeField] private List<LayerMask> playerLayers;

    // Core Controller
    private GameController gameController;              // Core Game Controller
    private PlayerInputManager playerInputManager;      // Play Input Manager Reference.

    private PlayerDataHolder playerDataHolder;          // Reference to where the Data is Being Saved

    [Header("Settings")]
    [SerializeField] private bool kickPlayerOnDisconntect = false;      // Kick players when they leave from the game
    [SerializeField] public SceneCameraType sceneCameraType;   
    [Space]

    // UI
    public InputSystemUIInputModule UIEventSystem;
    private Dictionary<InputDevice, PlayerInput> deviceSaver = new Dictionary<InputDevice, PlayerInput>(); //    Dictionary that holds player devices in case of kicking enabled
    private bool bringingPlayersOver = false;

    [HideInInspector] public bool initialized = false;

    public static PlayerManager Instance { get; private set; }

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

    private void Start()
    {
        Task Setup = setup();
    }

    // Note: Setup is the function used to wait until it finds a GameController
    private async Task setup()
    {
        // Wait for these values GameController needs to be 
        while (GameController.Instance == null || GameController.Instance.enabled == false || GameController.Instance.initialized == false)
        {
            await Task.Delay(1);
        }
        // Found Game Controller assing to local variable
        gameController = GameController.Instance;

        Debug.Log("Game Controller Found, Starting Scene Initialization           [Player Manager]");

        // Check the Scene Type.
        switch (gameController.gameState)
        {
            case GameState.Race:
                while (RacemodeManager.Instance == null || RacemodeManager.Instance.enabled == false || RacemodeManager.Instance.lookingForPlayer == false)
                {
                    await Task.Delay(1);
                }
                Debug.Log("Racemode Manager Found           [Player Manager]");
                break;
            case GameState.Arena:
                // Logic For Arena
                break;
            case GameState.StartScreen:
                break;
            case GameState.Lobby:
                while (LobbyManager.Instance == null || LobbyManager.Instance.enabled == false || LobbyManager.Instance.initialized == false)
                {
                    await Task.Delay(1);
                }
                Debug.Log("Lobby Manager Found           [Player Manager]");
                break;
            case GameState.Podium:
                while (PodiumManager.Instance == null || PodiumManager.Instance.enabled == false || PodiumManager.Instance.initialized == false ||
                    PodiumManager.Instance.lookingForPlayer == false)
                {
                    Debug.Log(PodiumManager.Instance.gameObject.name + " " + PodiumManager.Instance.enabled + " " + PodiumManager.Instance.initialized + " " + PodiumManager.Instance.lookingForPlayer);
                    await Task.Delay(1);
                }
                Debug.Log("Podium Manager Found           [Player Manager]");
                break;
            default:
                Debug.LogError("Player Manager has found an invalid game state          [Player Manager]");
                break;
        }

        Debug.Log("Game Controller Found, Initializing event subscription... ");

        gameController = GameController.Instance;
        playerDataHolder = PlayerDataHolder.Instance;
        playerInputManager = GetComponent<PlayerInputManager>();

        //gameController.startSystems += LateStart;
        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerLeft += RemovePlayer;

        LateStart();

    }

    //Fuck michael - Angel 10/09/2024
    private void OnDisable()
    {
        if (gameController != null)
        {
            playerInputManager.onPlayerJoined -= AddPlayer;
            playerInputManager.onPlayerLeft -= RemovePlayer;
        }
    }

    private void LateStart()
    {
        Debug.Log("Initializing Player Manager...         [Player Manager]");
        InitializePlayers();
    }

    private void InitializePlayers()
    {
        if (GameController.Instance != null)
        {
            //Debug.Log(GameController.Instance.gameState);
            if (gameController.gameState == GameState.Lobby || gameController.gameState == GameState.StartScreen)
            {
                playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
            }
        }

        // If Player Data Manager Exists
        if (playerDataHolder != null)
        {
            // Enable Bringing Players Over
            bringingPlayersOver = true;

            // Spawn Players Already In the Player Data Holder
            int players = playerDataHolder.players.Count;

            for (int i = 0; i < players; i++)
            {
                int playersInGame = i;
                string playerControlScheme = playerDataHolder.GetData(i).input.currentControlScheme;
                InputDevice playerDevice = playerDataHolder.GetData(i).device;
                playerInputManager.JoinPlayer(i, i - playerDataHolder.GetPlayers().Count, playerControlScheme, playerDevice);
                Debug.Log("Player #" + i + " has been spawned           [Player Manager]");
            }

            // Disable Bringing Players Over
            bringingPlayersOver = false;
        }
        else // Not Player Data Manager
        {
            //Debug.LogWarning("No player data manager was found, using automatic player join");
            playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenJoinActionIsTriggered;
        }

        Debug.Log("Player Manager correctly initialized...             [Player Manager]");
        initialized = true;
    }

    public void AddPlayer(PlayerInput player)
    {
        //players.Add(player);
        GameObject playerObject = player.gameObject;
        int playerID = player.playerIndex;

        // If there is a player Data Manager set the variables
        if (playerDataHolder != null)
        {
            // Not spawning players from another Scene
            if (!bringingPlayersOver)
            {
                // Add to Data Holder
                playerDataHolder.AddPlayer(player);
            }

            playerDataHolder.GetData(playerID).SetPlayerInput(player);
            playerDataHolder.GetData(playerID).SetPlayerInScene(player.transform.gameObject);

            // Check if playerPrefab has a MeshRenderer - Meaning its a player with model
            ApplyCosmeticsToPlayer(player, playerID);
        }

        // Need to use the paren due to the structure of the prefab
        Transform playerParent = player.transform.parent;

        playerParent.name = "Player #" + (playerID);
        playerObject.name = "Player #" + (playerID) + " controller";

        int layerToAdd = (int)Mathf.Log(playerLayers[playerID].value, 2);


        //SetLayerForEachChild(layerToAdd, player.transform);
        //playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
        if(player.gameObject.GetComponent<UICameraView>() != null)
        {
            player.gameObject.GetComponent<UICameraView>().characterCam.cullingMask |= 1 << layerToAdd;
        }

        switch (sceneCameraType)
        {
            case SceneCameraType.ThirdPersonControl:
                playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
                playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
/*
                // Add the Input Handler for camera controll
                // NOTE: This line was moved inside the ThirdPersonMovemen script
                //playerParent.GetComponentInChildren<InputHandler>().horizontal = player.actions.FindAction("Look");
*/
                break;
            case SceneCameraType.StaticCamera:
                //player.camera = Camera.main;
                player.GetComponent<ThirdPersonMovement>().cam = Camera.main.transform;
                break;
            case SceneCameraType.None:
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

/*    private void SetLayerForEachChild(int layerID, Transform parent)
    {
        parent.gameObject.layer = layerID;

        foreach (Transform child in parent)
        {
            SetLayerForEachChild(layerID, child);
        }
    }*/

    private void RemovePlayer(PlayerInput player)
    {
        // 4 - 1
        if (playerInputManager.maxPlayerCount > playerInputManager.playerCount)
        {
            playerInputManager.EnableJoining();
            //Debug.LogWarning("Enabled Joining");
        }

        if (kickPlayerOnDisconntect)
        {
            // Remove from Player Data
            playerDataHolder.RemovePlayer(player);

            // Destroy the Object
            Destroy(player.gameObject);
        }
    }
    private void ApplyCosmeticsToPlayer(PlayerInput player, int playerID)
    {
        if (player.gameObject.GetComponent<PlayerCosmetics>() != null)
        {
            //Debug.Log("Player has a PlayerCosmetics Item");
            // Add Cosmetic [Prototype]
            //Debug.Log(GameController.Instance.gameState.ToString());
            if (GameController.Instance.gameState == GameState.Lobby || BackupKicker.Instance != null)
            {
                // Set the default cosmetic if the scene is Lobby
                CosmeticManager.Instance.setDefaultCosmetic(playerDataHolder.GetPlayerData(player));
                Debug.Log("Set the default player for Player #" + player.playerIndex);
            }
            else
            {
                // Set the chosen cosmetic
                player.GetComponent<PlayerCosmetics>().body.GetComponent<SkinnedMeshRenderer>().material = playerDataHolder.GetData(playerID).GetCosmeticData().colorPicked;
                player.GetComponent<PlayerCosmetics>().eyes.GetComponent<SkinnedMeshRenderer>().material = playerDataHolder.GetData(playerID).GetCosmeticData().colorPicked;
                player.GetComponent<PlayerCosmetics>().fins.GetComponent<SkinnedMeshRenderer>().material = playerDataHolder.GetData(playerID).GetCosmeticData().colorPicked;

                // Spawn Hat
                if (playerDataHolder.GetData(playerID).GetCosmeticData().hatPrefab != null)
                {
                    Instantiate(playerDataHolder.GetData(playerID).GetCosmeticData().hatPrefab, player.GetComponent<PlayerCosmetics>().hatPos);
                }

                // == Spawn Boots ==
                // Right Boot
                if (playerDataHolder.GetData(playerID).GetCosmeticData().rightBootPrefab != null)
                {
                    Instantiate(playerDataHolder.GetData(playerID).GetCosmeticData().rightBootPrefab, player.GetComponent<PlayerCosmetics>().rightFoot);
                }
                // Left Boot
                if (playerDataHolder.GetData(playerID).GetCosmeticData().leftBootPrefab != null)
                {
                    Instantiate(playerDataHolder.GetData(playerID).GetCosmeticData().leftBootPrefab, player.GetComponent<PlayerCosmetics>().leftFoot);
                }
            }
        }
    }

    private void SetPlayerLayersToEachChild(Transform parent, int layer)
    {
        transform.gameObject.layer = layer;
        foreach (Transform child in transform)
        {
            SetPlayerLayersToEachChild(child, layer);
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (!kickPlayerOnDisconntect) { return; }
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
