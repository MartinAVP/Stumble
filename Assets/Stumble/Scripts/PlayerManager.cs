using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerManager : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<LayerMask> playerLayers;

    [Header("Cosmetic")]
    public Transform players3FillScreen;

    public PlayerInputManager playerInputManager;
    private PlayerDataManager playerDataManager;

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerDataManager = PlayerDataManager.Instance;
        
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;

/*        int playersInGame = playerInputManager.playerCount;
        Debug.Log("The current player count is " + playersInGame);
        string playerControlScheme = playerDataManager.GetPlayerData(playersInGame).input.currentControlScheme;
        InputDevice playerDevice = playerDataManager.GetPlayerData(playersInGame).device;
        playerInputManager.JoinPlayer(playersInGame, playersInGame, playerControlScheme, playerDevice);*/
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    private void Start()
    {
        for (int i = 0; i < playerDataManager.GetPlayers().Count; i++)
        {
            int playersInGame = playerInputManager.playerCount;
            Debug.Log("The current player count is " + playersInGame);
            string playerControlScheme = playerDataManager.GetPlayerData(playersInGame).input.currentControlScheme;
            InputDevice playerDevice = playerDataManager.GetPlayerData(playersInGame).device;
            playerInputManager.JoinPlayer(playersInGame, playersInGame - playerDataManager.GetPlayers().Count, playerControlScheme, playerDevice);
            Debug.Log("Split Screen Index for " + i + " is " + playersInGame);
        }
    }

    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);

        // Player Data Manager
        //playerDataManager.GetPlayerData(player).SetPlayerInScene(player.gameObject);

        playerDataManager.GetPlayerData(playerInputManager.playerCount - 1).SetPlayerInput(player);
        playerDataManager.GetPlayerData(playerInputManager.playerCount - 1).SetPlayerInScene(player.transform.parent.gameObject);

        // Need to use the paren due to the structure of the prefab
        Transform playerParent = player.transform.parent;
        playerParent.position = spawnPoints[playerInputManager.playerCount - 1].position;
        playerParent.name = "Player #" + (playerInputManager.playerCount - 1);
        Debug.Log("Setting player #" + (playerInputManager.playerCount - 1) + " to " + spawnPoints[playerInputManager.playerCount - 1].position);

        // Convert layer mask (bit) to an integer
        int layerToAdd = (int)Mathf.Log(playerLayers[playerInputManager.playerCount - 1].value, 2);

        //set the layer
        playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
        // add the layer
        playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;

        // set the action in the custom cinemachine Input handler
        playerParent.GetComponentInChildren<InputHandler>().horizontal = player.actions.FindAction("Look");

        // Add Cosmetic [Prototype]
        player.gameObject.GetComponentInChildren<MeshRenderer>().material = playerDataManager.GetPlayerData(player).cosmeticData.GetMaterialPicked();

        //Check for player Count
        //Debug.Log(players.Count);
        Player3ScreenToggle(players.Count);

    }

    private void Player3ScreenToggle(int count)
    {
        if(count == 3)
        {
            players3FillScreen.transform.gameObject.SetActive(true);
        }
        else
        {
            players3FillScreen.transform.gameObject.SetActive(false);
        }
    }
}
