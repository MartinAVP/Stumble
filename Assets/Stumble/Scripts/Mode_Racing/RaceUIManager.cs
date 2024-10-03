using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceUIManager : MonoBehaviour
{
    [Header("End Screen")]
    [SerializeField] private GameObject EndScoreScreen;
    public GameObject playerWinCardPrefab;
    public Transform playerScores;

    [Header("Start Counter")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private Image countdownTime;
    [SerializeField] private Sprite countdownThree;
    [SerializeField] private Sprite countdownTwo;

    [SerializeField] private Sprite countdownOne;
    [SerializeField] private Sprite countdownGo;

    private void OnEnable()
    {
        RaceManager.Instance.onCompleteFinish += DisplayEndScores;
        RaceManager.Instance.onCountdownStart += StartRace;
    }

    private void OnDisable()
    {
        RaceManager.Instance.onCompleteFinish -= DisplayEndScores;
        RaceManager.Instance.onCountdownStart -= StartRace;
    }

    private void Start()
    {
        EndScoreScreen?.SetActive(false);
        countdownPanel?.SetActive(false);
    }

    private void StartRace()
    {
        StartCoroutine(StartRaceCountdown());
    }

    public IEnumerator StartRaceCountdown()
    {
        float waitTime = 1.2f;

        yield return new WaitForSeconds(waitTime);
        countdownPanel?.SetActive(true);
        countdownTime.sprite = countdownThree;
        //countdownPanel.GetComponent<Animator>().SetTrigger("StartTrigger");
        countdownTime.gameObject.GetComponent<Animation>().Play("ShowNumber");
        yield return new WaitForSeconds(waitTime);
        countdownTime.sprite = countdownTwo;
        //countdownPanel.GetComponent<Animator>().SetTrigger("StartTrigger");
        countdownTime.gameObject.GetComponent<Animation>().Play("ShowNumber");
        yield return new WaitForSeconds(waitTime);
        countdownTime.sprite = countdownOne;
        //countdownPanel.GetComponent<Animator>().SetTrigger("StartTrigger");
        countdownTime.gameObject.GetComponent<Animation>().Play("ShowNumber");
        yield return new WaitForSeconds(waitTime);
        countdownTime.sprite = countdownGo;
        //countdownPanel.GetComponent<Animator>().SetTrigger("StartTrigger");
        countdownTime.gameObject.GetComponent<Animation>().Play("ShowNumber");

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
