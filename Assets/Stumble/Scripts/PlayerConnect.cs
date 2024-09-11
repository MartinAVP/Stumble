using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerConnect : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerLobbyPrefab;
    [SerializeField] private GameObject playerCardPrefab;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI m_playerQuantityText;
    [SerializeField] private UnityEngine.UI.Button m_addOnePlayer;
    [SerializeField] private UnityEngine.UI.Button m_removeOnePlayer;

    [SerializeField] private float playerSpawnViewRange;
    [SerializeField] private List<GameObject> players = new List<GameObject>();
    [SerializeField] private List<GameObject> playerCards = new List<GameObject>();
    [SerializeField] private Transform scrollView;

    private int numPlayers = 0;

    private void Awake()
    {
        m_addOnePlayer.onClick.AddListener(addOnePlayer);
        m_removeOnePlayer.onClick.AddListener(removeOnePlayer);
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
        ScrollRect view = scrollView.transform.GetComponent<ScrollRect>();
        float width = scrollView.transform.GetComponent<RectTransform>().rect.width;
        float scaleFactor = width / playerSpawnViewRange;
        float offset = playerSpawnViewRange / 2;

        //Debug.Log(width);
        for (int i = 0; i < playerCards.Count; i++)
        {
            float targetPos = ((view.content.GetChild(i).GetComponent<RectTransform>().localPosition.x) / scaleFactor) - offset;
            //Debug.Log("Card" + i + ": " + targetPos);
            Vector3 targetVector = new Vector3(targetPos, 0 ,0);
            players[i].transform.position = targetVector;
        }
    }

    public void addOnePlayer()
    {
        if (numPlayers < 4) {
            numPlayers++;
            GameObject tempCard = Instantiate(playerCardPrefab, scrollView.transform.GetComponent<ScrollRect>().content.transform);
            tempCard.transform.name = "Player Card #" + playerCards.Count.ToString();
            playerCards.Add(tempCard);

            GameObject tempPlayer = Instantiate(playerLobbyPrefab, this.transform.position, Quaternion.identity);
            tempPlayer.transform.Rotate(0, 180, 0);
            tempPlayer.transform.name = "Player #" + players.Count.ToString();
            players.Add(tempPlayer);

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

            GameObject tempPlayer = players[players.Count - 1];
            //Debug.Log("Removing" + (players.Count - 1) + ": " + tempPlayer);
            players.RemoveAt(players.Count - 1);
            Destroy(tempPlayer);

            allingPlayers();

            m_playerQuantityText.text = numPlayers.ToString();
        }
    }
}
