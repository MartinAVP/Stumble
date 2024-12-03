using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public bool initialized { get; private set; }

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

        Task Setup = setup();
    }

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (RacemodeManager.Instance == null || RacemodeManager.Instance.enabled == false || RacemodeManager.Instance.initialized == false)
        {
            // Await 5 ms and try finding it again.
            // It is made 5 seconds because it is
            // a core gameplay mechanic.
            await Task.Delay(1);
        }

        // Once it finds it initialize the sceneW
        Debug.Log("Initializing Racemode UI Manager...         [Racemode UI Manager]");
        //GameController.Instance.startSystems += LateStart;

        InitializeManager();
        initialized = true;
/*        RacemodeManager.Instance.onCountdownStart += StartRace;*/
        return;
    }

    private void OnEnable()
    {
        //RaceManager.Instance.onCompleteFinish += DisplayEndScores;
    }

    private void OnDisable()
    {
        //RaceManager.Instance.onCompleteFinish -= DisplayEndScores;
/*        if (initialized)
        {
            RacemodeManager.Instance.onCountdownStart -= StartRace;
        }*/
    }

    private void InitializeManager()
    {
        //EndScoreScreen?.SetActive(false);
        if(countdownPanel != null)
        {
            countdownPanel?.SetActive(false);
        }

    }


    public void StartRace()
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

    public bool HasAllCountDownValues()
    {
        if (countdownPanel != null && countdownTime != null &&
            countdownThree != null && countdownTwo != null && countdownOne != null && countdownGo != null)
        {
            return true;
        }
        return false;
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
