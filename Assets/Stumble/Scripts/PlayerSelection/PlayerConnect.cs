using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class PlayerConnect : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerLobbyPrefab;
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private GameObject playerCardTopPrefab;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI m_playerQuantityText;
    [SerializeField] private UnityEngine.UI.Button m_addOnePlayer;
    [SerializeField] private UnityEngine.UI.Button m_removeOnePlayer;
    [SerializeField] private UnityEngine.UI.Button m_startSelection;
    [Space]
    [SerializeField] private Transform playerSelectionPanel;
    [SerializeField] private Transform playerCardsPanel;
    [SerializeField] private Transform playerAssignPanel;
    [Space]

    [SerializeField] private float playerSpawnViewRange;
    [SerializeField] private List<GameObject> playerSpawns = new List<GameObject>();
    [SerializeField] private List<GameObject> playerCards = new List<GameObject>();
    [SerializeField] private List<GameObject> playerCardsTop = new List<GameObject>();
    [SerializeField] private Transform scrollBottomView;
    [SerializeField] private Transform scrollTopView;

    private int numPlayers = 0;

    private void Awake()
    {
        m_addOnePlayer.onClick.AddListener(addOnePlayer);
        m_removeOnePlayer.onClick.AddListener(removeOnePlayer);
        m_startSelection.onClick.AddListener(StartGame);
    }

    private void Start()
    {
        addOnePlayer();
    }

    private void LateUpdate()
    {
        allingPlayers();
    }

    private void allingPlayers()
    {
        ScrollRect view = scrollBottomView.transform.GetComponent<ScrollRect>();
        float width = scrollBottomView.transform.GetComponent<RectTransform>().rect.width;
        float scaleFactor = width / playerSpawnViewRange;
        float offset = playerSpawnViewRange / 2;

        //Debug.Log(width);
        for (int i = 0; i < playerCards.Count; i++)
        {
            float targetPos = ((view.content.GetChild(i).GetComponent<RectTransform>().localPosition.x) / scaleFactor) - offset;
            //Debug.Log("Card" + i + ": " + targetPos);
            Vector3 targetVector = new Vector3(targetPos, 0 ,0);
            playerSpawns[i].transform.position = targetVector;
        }
    }

    public void StartGame()
    {
        playerSelectionPanel.gameObject.SetActive(false);
        playerAssignPanel.gameObject.SetActive(true);
        foreach(var player in playerSpawns)
        {
            player.gameObject.SetActive(true);
        }
    }

    public void addOnePlayer()
    {
        if (numPlayers < 4) {
            numPlayers++;
            GameObject tempCard = Instantiate(playerCardPrefab, scrollBottomView.transform.GetComponent<ScrollRect>().content.transform);
            tempCard.transform.name = "Player Card #" + playerCards.Count.ToString();
            playerCards.Add(tempCard);

            GameObject tempCardTop = Instantiate(playerCardTopPrefab, scrollTopView.transform.GetComponent<ScrollRect>().content.transform);
            tempCardTop.transform.name = "Player Join Card #" + playerCards.Count.ToString();
            playerCardsTop.Add(tempCardTop);

            //Debug.Log("Reach");

            GameObject tempPlayer = Instantiate(playerLobbyPrefab, this.transform.position, Quaternion.identity);
            tempPlayer.transform.Rotate(0, 180, 0);
            tempPlayer.transform.name = "Player Spawn #" + playerSpawns.Count.ToString();
            tempPlayer.gameObject.SetActive(false);
            //tempPlayer.GetComponentInChildren<PlayerInput>().camera = Camera.main;
            playerSpawns.Add(tempPlayer);

            allingPlayers();
            m_playerQuantityText.text = numPlayers.ToString();
        }
    }
    public void removeOnePlayer() 
    {
        if (numPlayers > 1)
        {
            numPlayers--;
            GameObject tempCard = playerCards[playerCards.Count - 1];
            playerCards.RemoveAt(playerCards.Count - 1);
            Destroy(tempCard);

            GameObject tempCardTop = playerCardsTop[playerCards.Count - 1];
            playerCardsTop.RemoveAt(playerCardsTop.Count - 1);
            Destroy(tempCardTop);

            GameObject tempPlayer = playerSpawns[playerSpawns.Count - 1];
            //Debug.Log("Removing" + (players.Count - 1) + ": " + tempPlayer);
            playerSpawns.RemoveAt(playerSpawns.Count - 1);
            Destroy(tempPlayer);

            allingPlayers();

            m_playerQuantityText.text = numPlayers.ToString();
        }
    }

    public Vector3 getSpawnPos(int id)
    {
        if(id <= playerSpawns.Count)
        {
            return playerSpawns[id].transform.position;
        }
        return Vector3.zero;
    }

    public void playerJoined(int id)
    {
        playerCards[id].GetComponentInChildren<TextMeshProUGUI>().text = "Player Joined Succesfully! Joined As Player #" + id;
        playerCardsTop[id].transform.GetChild(0).gameObject.SetActive(false);
    }

    public void playerLostConnect(int id)
    {
        playerCards[id].GetComponentInChildren<TextMeshProUGUI>().text = "Player Lost Connection! Please Reconnect Player #" + id;
        playerCardsTop[id].transform.GetChild(0).gameObject.SetActive(true);
    }

    public void playerReConnect(int id)
    {
        playerCards[id].GetComponentInChildren<TextMeshProUGUI>().text = "Player Reconnected Succesfully! Joined As Player #" + id;
        playerCardsTop[id].transform.GetChild(0).gameObject.SetActive(false);
    }
}
