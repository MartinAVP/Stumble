using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceUIManager : MonoBehaviour
{
    [SerializeField] private GameObject EndScoreScreen;
    public GameObject playerWinCardPrefab;
    public Transform playerScores;

    private void OnEnable()
    {
        RaceManager.Instance.onCompleteFinish += DisplayEndScores;
    }

    private void OnDisable()
    {
        RaceManager.Instance.onCompleteFinish -= DisplayEndScores;
    }

    private void Start()
    {
        EndScoreScreen.SetActive(false);
    }

    public void DisplayEndScores(SortedDictionary<float, PlayerData> players)
    {

        Transform content = playerScores.GetComponent<ScrollRect>().content;

        EndScoreScreen.SetActive(true);
        int index = 1;
        foreach (var item in players)
        {
            GameObject card = Instantiate(playerWinCardPrefab, content);
            card.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = index.ToString("D2");
            card.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Player #" + item.Value.GetID();
            card.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.Key.ToString("F3");
            index++;
        }
        index = 0;
    }

}
