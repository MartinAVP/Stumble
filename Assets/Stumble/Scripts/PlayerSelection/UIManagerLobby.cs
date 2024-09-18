using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private Transform _playerCardView;

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

        //_playerInputManager.onPlayerJoined.AddListener(StartControllerConnection);

        // Player Lobby
        _changePlayerCount.onClick.AddListener(ChangePlayerCount);

        // Initialize Input Manager, Disable Player joining
        _playerInputManager.DisableJoining();
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
            GameObject temp = Instantiate(playerCardPrefab, _playerCardView.GetComponent<ScrollRect>().content);
            temp.name = "Player Card #" + i;
            bottomCards.Add(temp);
        }

        // Initialize Spawn Positions in Relation to the target Players
        initializePlayerSpawnPositions();
    }

    private void initializePlayerSpawnPositions()
    {
        ScrollRect view = _playerCardView.transform.GetComponent<ScrollRect>();
        float width = _playerCardView.transform.GetComponent<RectTransform>().rect.width;
        float scaleFactor = width / playersViewRangeInWorld;
        float offset = playersViewRangeInWorld / 2;

        Debug.Log(width + " " + scaleFactor + " " + offset);

        //Debug.Log(width);
        for (int i = 0; i < targetPlayers; i++)
        {
            Debug.Log(view.content.GetChild(i).GetComponent<RectTransform>().transform.name);
            //Debug.Log((view.content.GetChild(i).GetComponent<RectTransform>().localPosition.x));
            Debug.Log((view.content.GetChild(i).GetComponent<RectTransform>().localPosition));
            Debug.Log((view.content.GetChild(i).GetComponent<RectTransform>().rect.x));
/*            Debug.Log((view.content.GetChild(i).GetComponent<RectTransform>().anchoredPosition3D.x));
            Debug.Log((view.content.GetChild(i).GetComponent<RectTransform>().position.x));*/
            float targetPos = ((view.content.GetChild(i).GetComponent<RectTransform>().localPosition.x) / scaleFactor) - offset;
            //Debug.Log("Card" + i + ": " + targetPos);
            Vector3 targetVector = new Vector3(targetPos, 0, 0);
            spawnPositions.Add(targetVector);
        }
    }


}
