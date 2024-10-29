using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PodiumManager : MonoBehaviour
{
    //private Stopwatch stopwatch;
    //public SortedDictionary<float, PlayerData> positions = new SortedDictionary<float, PlayerData>();

    //public event Action<SortedDictionary<float, PlayerData>> onCompleteFinish;
    //public event Action onCountdownStart;
    public event Action onPodiumStarted;

    //PlayerDataManager dataManager;

    public static PodiumManager Instance { get; private set; }
    public bool initialized { get; private set; }

    // Singleton
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
        while (ExperimentalPlayerManager.Instance == null || ExperimentalPlayerManager.Instance.enabled == false || ExperimentalPlayerManager.Instance.finishedSystemInitializing == false)
        {
            // Await 2 ms and try finding it again.
            // It is made 2 seconds because it is
            // a core gameplay mechanic.
            await Task.Delay(1);
        }

        // Once it finds it initialize the scene
        UnityEngine.Debug.Log("Initializing Podium Manager...         [Podium Manager]");
        //GameController.Instance.startSystems += LateStart;

        InitializeManager();
        initialized = true;
        return;
    }

    private void InitializeManager()
    {
        onPodiumStarted?.Invoke();
        //if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(false); }
        StartCountdown();
    }

    public virtual void StartCountdown()
    {
        if (CinematicController.Instance != null)
        {
            CinematicController.Instance.StartTimeline();
        }

        StartCoroutine(returnToMenuCooldown());
    }

    private IEnumerator returnToMenuCooldown()
    {
        yield return new WaitForSeconds(20f);
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GamemodeSelect");
    }

}
