using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInputManager))]
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
    private int targetPlayers = 1;

    [Header("Controller Connection Screen")]
    [SerializeField] private Transform _playerCardsPanel;
    [SerializeField] private Transform _playerAssignPanel;
    [SerializeField] private float playersViewRangeInWorld;
    [SerializeField] private UnityEngine.UI.Button _changePlayerCount;
    [SerializeField] private Transform playerBottomCardView;

    [SerializeField] private List<GameObject> _players = new List<GameObject>();
    [SerializeField] private List<GameObject> bottomCards = new List<GameObject>();
    [SerializeField] private List<Vector3> spawnPositions = new List<Vector3>();

    //[Header("Misc")]
    private int playersInLobby;
    private PlayerInputManager _playerInputManager;

    private void Start()
    {
        // Initialize Canvas
        _playerQuantitySelectionPanel.gameObject.SetActive(true);
        _playerCardsPanel.gameObject.SetActive(false);
        _playerAssignPanel.gameObject.SetActive(false);
    }

    private void Awake()
    {
        // Get Player Manager
        _playerInputManager = this.GetComponent<PlayerInputManager>();

        // Player Quantity Selection Buttons
        _increasePlayerQuantity.onClick.AddListener(AddToPlayerQuantity);
        _decreasePlayerQuantity.onClick.AddListener(SubtractToPlayerQuantity);
        _startControllerConnection.onClick.AddListener(StartControllerConnection);


        // Subscribe to Input System
        _playerInputManager.onPlayerJoined += (player) => joinNewPlayer(player);
        _playerInputManager.onPlayerLeft += (player => removeExistingPlayer(player));

        // Player Lobby
        _changePlayerCount.onClick.AddListener(ChangePlayerCount);

        // Subscribe to Controller Disconnection Event
        InputSystem.onDeviceChange += OnDeviceChange;

        // Initialize Input Manager, Disable Player joining
        _playerInputManager.DisableJoining();
    }

    private void OnDisable()
    {
        // Player Quantity Selection Buttons
        _increasePlayerQuantity.onClick.RemoveListener(AddToPlayerQuantity);
        _decreasePlayerQuantity.onClick.RemoveListener(SubtractToPlayerQuantity);
        _startControllerConnection.onClick.RemoveListener(StartControllerConnection);

        _playerInputManager.onPlayerJoined -= (player) => joinNewPlayer(player);
        _playerInputManager.onPlayerLeft -= (player => removeExistingPlayer(player));

        // Player Lobby
        _changePlayerCount.onClick.RemoveListener(ChangePlayerCount);

        // Subscribe to Controller Disconnection Event
        InputSystem.onDeviceChange -= OnDeviceChange;
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

        Debug.Log(width);
        for (int i = 0; i < targetPlayers; i++)
        {
            float UIPos = start + (i * step);
            float targetPos = (UIPos / scaleFactor) - offset;
            Vector3 targetVector = new Vector3(targetPos, 0, 0);
            spawnPositions.Add(targetVector);
        }
    }

    private void joinNewPlayer(PlayerInput player)
    {
        int id = _playerInputManager.playerCount - 1;
        // Set the position to its defined spawn Pos based on ID
        player.gameObject.transform.position = spawnPositions[id];
        // Add the new player to the List
        _players.Add(player.gameObject);
        // Set the Bottom Card Text of the Joined Player.
        //bottomCards[id].GetComponentInChildren<TextMeshProUGUI>().text = "Player #" + (id + 1);
        sortCardsAndPlayer();
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
        for (int i = 0; i < _playerInputManager.playerCount; i++)
        {
            _players[i].transform.position = spawnPositions[i];
        }
    }

    private void removeExistingPlayer(PlayerInput player)
    {
        Debug.Log("Player Left");
    }

    // Device Reconnection System
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        int playerID = 0;
        switch (change)
        {
            case InputDeviceChange.Removed:
/*                Debug.Log($"Device removed: {device}");
                playerID = findPlayer(device);
                Debug.Log("Device Disconnected belonged to player #" + playerID);*/
                this.GetComponent<PlayerDataManager>().RemovePlayer(device);
                sortCardsAndPlayer();
                break;
            case InputDeviceChange.Reconnected:
                Debug.Log("Device Reconnected attached to player #" + playerID);
                break;
        }
    }
}
