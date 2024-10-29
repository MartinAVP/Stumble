using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArenamodeUIManager : MonoBehaviour
{
    public static ArenamodeUIManager Instance;

    [Header("Start Counter")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private Image countdownTime;
    [SerializeField] private Sprite countdownThree;
    [SerializeField] private Sprite countdownTwo;
    [SerializeField] private Sprite countdownOne;
    [SerializeField] private Sprite countdownGo;

    [Header("Players Alive")]
    [SerializeField] private TextMeshProUGUI playersAlive;

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
        ArenamodeManager.Instance.onCountdownStart += StartArena;
    }

    private void OnDisable()
    {
        //RaceManager.Instance.onCompleteFinish -= DisplayEndScores;
        ArenamodeManager.Instance.onCountdownStart -= StartArena;
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

    // Start is called before the first frame update
    void Start()
    {
        if(LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.StartTransition(false);
        }
    }

    private void StartArena()
    {
        StartCoroutine(StartArenaCountdown());
    }

    public IEnumerator StartArenaCountdown()
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

    public void UpdatePlayersAlive(string quantity)
    {
        playersAlive.text = quantity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
