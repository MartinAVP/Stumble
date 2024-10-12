using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RacemodeUIManager : MonoBehaviour
{
    [Header("End Screen")]
/*    [SerializeField] private GameObject EndScoreScreen;
    public GameObject playerWinCardPrefab;
    public Transform playerScores;*/

    [Header("Start Counter")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private Image countdownTime;
    [SerializeField] private Sprite countdownThree;
    [SerializeField] private Sprite countdownTwo;
    [SerializeField] private Sprite countdownOne;
    [SerializeField] private Sprite countdownGo;

    public static RacemodeUIManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        //RaceManager.Instance.onCompleteFinish += DisplayEndScores;
        RacemodeManager.Instance.onCountdownStart += StartRace;
    }

    private void OnDisable()
    {
        //RaceManager.Instance.onCompleteFinish -= DisplayEndScores;
        RacemodeManager.Instance.onCountdownStart -= StartRace;
    }



    private void Start()
    {
        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.StartTransition(false);
        }

        //EndScoreScreen?.SetActive(false);
        if(countdownPanel != null)
        {
            countdownPanel?.SetActive(false);
        }

    }

    public bool HasAllCountDownValues()
    {
        if(countdownPanel != null && countdownTime != null &&
            countdownThree != null && countdownTwo != null && countdownOne != null && countdownGo != null)
        {
            return true;
        }
        return false;
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

/*    public void DisplayEndScores(SortedDictionary<float, PlayerData> players)
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
    }*/
}
