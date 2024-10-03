using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

//[RequireComponent(typeof(PlayerInputManager))]
public class UIManagerLobby : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private GameObject playerLobbyPrefab;

    [Header("Player Selection Screen")]
    [SerializeField] private Transform _playerQuantitySelectionPanel;

    [SerializeField] private UnityEngine.UI.Button _decreasePlayerQuantity;
    [SerializeField] private UnityEngine.UI.Button _increasePlayerQuantity;
    [SerializeField] private UnityEngine.UI.Button _startControllerConnection;

    [SerializeField] private TextMeshProUGUI _targetPlayerCount;
    public int targetPlayers { get; private set; } = 1;

    [Header("Controller Connection Screen")]
    [SerializeField] private Transform _playerCardsPanel;
    [SerializeField] private Transform _playerAssignPanel;
    [SerializeField] private float playersViewRangeInWorld;
    [SerializeField] private UnityEngine.UI.Button _changePlayerCount;
    [SerializeField] private Transform playerBottomCardView;

    [SerializeField] private List<GameObject> _players = new List<GameObject>();
    [SerializeField] private List<GameObject> bottomCards = new List<GameObject>();
    [SerializeField] private List<Vector3> spawnPositions = new List<Vector3>();

    [Header("Player Holder")]
/*    public GameObject playerHolder;*/

    //[Header("Misc")]
    public PlayerInputManager _playerInputManager;
    private PlayerDataManager _playerDataManager;

    private void Start()
    {
        // Initialize Canvas
        _playerQuantitySelectionPanel.gameObject.SetActive(true);
        _playerCardsPanel.gameObject.SetActive(false);
        _playerAssignPanel.gameObject.SetActive(false);

        //=-==================================================
        _playerDataManager = PlayerDataManager.Instance;
        //_playerInputManager = FindObjectOfType<PlayerInputManager>();
        _playerInputManager = FindAnyObjectByType<PlayerInputManager>();
        // Initialize Input Manager, Disable Player joining
        _playerInputManager.DisableJoining();


        _playerDataManager.onPlayerConnect += (player) => joinNewPlayer(player);
        //_playerInputManager.onPlayerLeft += (player) => removeExistingPlayer(player);
        //_playerDataManager.onPlayerDisconnect += (player) => removeExistingPlayer(player);
        _playerDataManager.onPlayerInputDeviceDisconnect += (player) => removeExistingPlayer(player);
    }

    private void Awake()
    {
        // Get Player Manager
        /*        _playerInputManager = this.GetComponent<PlayerInputManager>();
                _playerDataManager = this.GetComponent<PlayerDataManager>();*/

        // Player Quantity Selection Buttons
        _increasePlayerQuantity.onClick.AddListener(AddToPlayerQuantity);
        _decreasePlayerQuantity.onClick.AddListener(SubtractToPlayerQuantity);
        _startControllerConnection.onClick.AddListener(StartControllerConnection);


        // Subscribe to Input System
        //_playerInputManager.onPlayerJoined += (player) => joinNewPlayer(player);
        //_playerInputManager.onPlayerLeft += (player => removeExistingPlayer(player));

        // Player Lobby
        _changePlayerCount.onClick.AddListener(ChangePlayerCount);

        // Subscribe to Controller Disconnection Event
        //InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        // UI - Player Quantity Selection Buttons
        _increasePlayerQuantity.onClick.RemoveListener(AddToPlayerQuantity);
        _decreasePlayerQuantity.onClick.RemoveListener(SubtractToPlayerQuantity);
        _startControllerConnection.onClick.RemoveListener(StartControllerConnection);

        //_playerInputManager.onPlayerJoined -= (player) => joinNewPlayer(player);
        //_playerInputManager.onPlayerLeft -= (player => removeExistingPlayer(player));

        // Subscribe to PlayerDataManager
        _playerDataManager.onPlayerConnect -= (player) => joinNewPlayer(player);
        //_playerDataManager.onPlayerDisconnect += (player) => removeExistingPlayer(player);
        _playerDataManager.onPlayerInputDeviceDisconnect += (player) => removeExistingPlayer(player);

        // Player Lobby
        _changePlayerCount.onClick.RemoveListener(ChangePlayerCount);

        // Subscribe to Controller Disconnection Event
        //InputSystem.onDeviceChange -= OnDeviceChange;
    }

    // Buttons
    private void AddToPlayerQuantity()
    {
        // Set Target Player Count and Prevent it going over 4
        if(targetPlayers >= 4) { return; }
        targetPlayers++;

        // Update the UI Counter
        _targetPlayerCount.text = targetPlayers.ToString();
    }
    private void SubtractToPlayerQuantity()
    {
        if(targetPlayers <= 1) { return; }
        targetPlayers--;

        // Update the UI Counter
        _targetPlayerCount.text = targetPlayers.ToString();
    }

    private void ChangePlayerCount()
    {
        _playerInputManager.DisableJoining();

        // Clear all players in Scene
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            Destroy(p);
        }

        // Clear all the Cards in the UI
        foreach(GameObject c in bottomCards)
        {
            Destroy(c);
        }

        // Clear the Spawn Positions
        spawnPositions.Clear();

        // Clear players in Data
        _playerDataManager.ClearPlayers();

        // Update the UI
        _playerQuantitySelectionPanel.gameObject.SetActive(true);
        _playerCardsPanel.gameObject.SetActive(false);
        _playerAssignPanel.gameObject.SetActive(false);
    }

    private void StartControllerConnection()
    {
        // Enabling Joining to the Game
        _playerInputManager.EnableJoining();

        // Update the UI
        _playerQuantitySelectionPanel.gameObject.SetActive(false);
        _playerCardsPanel.gameObject.SetActive(true);
        _playerAssignPanel.gameObject.SetActive(true);

        // Add Bottom Cards to UI
        for (int i = 0; i < targetPlayers; i++)
        {
            GameObject temp = Instantiate(playerCardPrefab, playerBottomCardView.GetComponent<ScrollRect>().content);
            temp.name = "Player Card #" + i;
            bottomCards.Add(temp);
        }

        // Initialize Spawn Positions in Relation to the target Players
        initializePlayerSpawnPositions();
    }

    private void initializePlayerSpawnPositions()
    {
        ScrollRect view = playerBottomCardView.transform.GetComponent<ScrollRect>();

        float width = playerBottomCardView.transform.GetComponent<RectTransform>().rect.width;
        float scaleFactor = width / playersViewRangeInWorld;
        float offset = playersViewRangeInWorld / 2;

        float start = width / (targetPlayers * 2); // Starting value
        float step = start * 2; // Step size between values

        //Debug.Log(width);
        for (int i = 0; i < targetPlayers; i++)
        {
            float UIPos = start + (i * step);
            float targetPos = (UIPos / scaleFactor) - offset;
            Vector3 targetVector = new Vector3(targetPos, 0, 0);
            spawnPositions.Add(targetVector);
        }
    }

    private void joinNewPlayer(PlayerData player)
    {
        // Check if the player is not in the lobby
        if (!PlayerDataManager.Instance.isLobby) { return; }
        // Set the Player Position to one of the SpawnPoints
        player.GetPlayerInScene().gameObject.transform.position = spawnPositions[player.GetID()];

        // Prevent Player Spawning If already At Max
        if(_playerInputManager.playerCount >= targetPlayers)
        {
            _playerInputManager.DisableJoining();
        }

        // Set Default Cosmetics
        //player.GetPlayerInScene().GetComponentInChildren<MeshRenderer>().material = CosmeticManager.Instance.colors[0].color;
        CosmeticManager.Instance.setDefaultCosmetic(player);
    }

    private void sortCardsAndPlayer() {
        // Re Allign Cards
        // Set All Cards to Await for Players
        for (int i = 0; i < targetPlayers; i++)
        {
            bottomCards[i].GetComponentInChildren<TextMeshProUGUI>().text = "Press A or Space to Join";
        }

        for (int i = 0;i < _playerInputManager.playerCount; i++)
        {
            bottomCards[i].GetComponentInChildren<TextMeshProUGUI>().text = "Player #" + (i + 1);
        }
        // Re Allign Players
        // Create a temp List
        List<PlayerData> tempData = _playerDataManager.players;
        for (int i = 0; i < _playerInputManager.playerCount; i++)
        {
            tempData[i].GetPlayerInScene().transform.position = spawnPositions[i];
        }
    }

    private void removeExistingPlayer(PlayerData player)
    {
        // Check if the player is not in the lobby
        if (!PlayerDataManager.Instance.isLobby) { return; }
        // Destroy the Player in the Scene
        Destroy(player.GetPlayerInScene());
        // Re Sort the Players in the screen.
        sortCardsAndPlayer();

        if (_playerInputManager.playerCount <= targetPlayers)
        {
            _playerInputManager.EnableJoining();
        }
    }

    // Device Reconnection System
/*    public int GetTargetPlayers()
    {
        return targetPlayers;
    }*/
}
