using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

/*    [Header("Players Alive")]
    [SerializeField] private TextMeshProUGUI playersAlive;*/

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
        while (ArenamodeManager.Instance == null || ArenamodeManager.Instance.enabled == false || ArenamodeManager.Instance.initialized == false)
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

    private void InitializeManager()
    {
        
        if (LoadingScreenManager.Instance != null)
        {
            Debug.LogWarning("No Loading Screen Manager Found");
            LoadingScreenManager.Instance.StartTransition(false);
        }
        else
        {
            Debug.LogWarning("No Loading Screen Manager Found");
        }

        //EndScoreScreen?.SetActive(false);
        if (countdownPanel != null)
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
}
