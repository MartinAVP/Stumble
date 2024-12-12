using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
/*using static UnityEditor.Experimental.GraphView.GraphView;*/
using Debug = UnityEngine.Debug;

public class PodiumManager : MonoBehaviour
{
    //private Stopwatch stopwatch;
    //public SortedDictionary<float, PlayerData> positions = new SortedDictionary<float, PlayerData>();

    //public event Action<SortedDictionary<float, PlayerData>> onCompleteFinish;
    //public event Action onCountdownStart;
    public event Action onPodiumStarted;

    public SortedDictionary<PlayerData, int> positions = new SortedDictionary<PlayerData, int>();
    public List<PlayerData> newPositions = new List<PlayerData>();

    //PlayerDataManager dataManager;

    public bool initialized { get; private set; }

    public bool lookingForPlayer { get; private set; } = false;
    public bool lookingForSpawnManager { get; private set; } = false;
    public bool lookingForUIManager { get; private set; } = false;

    // Game Controller
    private GameController gameController;

    // Primary Systems - Needed for the Game to Start
    private PlayerManager playerManager;
    private PodiumSpawnManager podiumSpawnManager;

    // Secondary Systems - Not needed for the Game to Start, However will perform their duty if present.
    private PodiumUIManager podiumUIManager;
    private CinematicController cinematicController;
    private PodiumMusicController podiumMusicController;
    public static PodiumManager Instance { get; private set; }          

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

    private void Start()
    {
        Task Setup = setup();
    }

    private async Task setup()
    {
        // Wait for the Game Controller To Initialize, The Race cannot start without a Game Controller.
        while (GameController.Instance == null || GameController.Instance.enabled == false || GameController.Instance.initialized == false)
        {
            await Task.Delay(1);
        }
        gameController = GameController.Instance;
        Debug.Log("Found Game Controller...         [Podium Manager]");


        initialized = true;     // Initialized Means that systems can now be Started.
                                // Initialized is a validartor variable that is set to true once the system
                                // is fully operational.


        // ==== Primary Systems ===============================
        // Await Player Manager to Initialize
        lookingForPlayer = true;
        while (PlayerManager.Instance == null || PlayerManager.Instance.enabled == false || PlayerManager.Instance.initialized == false)
        {
            await Task.Delay(1);
        }
        lookingForPlayer = false;
        playerManager = PlayerManager.Instance;
        Debug.Log("Found Player Manager...         [Podium Manager]");

        lookingForSpawnManager = true;
        while (PodiumSpawnManager.Instance == null || PodiumSpawnManager.Instance.enabled == false || PodiumSpawnManager.Instance.initialized == false)
        {
            await Task.Delay(2);
        }
        lookingForSpawnManager = false;
        podiumSpawnManager = PodiumSpawnManager.Instance;
        Debug.Log("Found Podium Spawn Manager...         [Podium Manager]");

        Debug.Log("Finished Initializing Core Systems");

        // ==== Secondary Systems ===============================
        // Racemode UI Manager
        if (PodiumUIManager.Instance != null && PodiumUIManager.Instance.enabled == true)
        {
            podiumUIManager = PodiumUIManager.Instance;
            Debug.Log("Found Podium UI Manager...         [Podium Manager]");
        }
        else
        {
            Debug.LogWarning("Podium UI Manager Not found, Skipping...    [Podium Manager]");
        }

        // Cinematic Controller
        if (CinematicController.Instance != null && CinematicController.Instance.enabled == true)
        {
            cinematicController = CinematicController.Instance;
            Debug.Log("Found Cinematic Controller...         [Podium Manager]");
        }
        else
        {
            Debug.LogWarning("Cinematic Controller Not found, Skipping...    [Podium Manager]");
        }

        // Music Manager
        if (PodiumMusicController.Instance != null && PodiumMusicController.Instance.enabled == true)
        {
            podiumMusicController = PodiumMusicController.Instance;
            Debug.Log("Found Podium Music Controller...         [Podium Manager]");
        }
        else
        {
            Debug.LogWarning("Podium Music Controller not Found, Skipping...    [Podium Manager]");
        }

        InitializeManager();
        return;
    }

    private void InitializeManager()
    {
        // Lock all players in place
        Debug.Log("PrePreTask");
        LockPlayersMovement(true);

        Debug.Log("PreTask");
        StartCoroutine(MainRaceController());
    }

    private IEnumerator MainRaceController()
    {

        // INTERMEDIARY ACTION.
        CalculateFinalPositionPlacing();
        SetPlayerSpawns();

        // Base Delay
        Debug.Log("InTask");
        yield return new WaitForSeconds(.1f);
        if (podiumUIManager != null) { 
            SetUIMessage();
        }

        LockPlayersMovement(true);

        yield return new WaitForSeconds(.5f);
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(false); }
        TriggerAnimations();
        if (cinematicController != null)
        {
            Debug.Log("Initializing Cinematic");
            cinematicController.StartTimeline();
            yield return new WaitForSeconds(cinematicController.GetTimelineLenght.ConvertTo<int>() + .2f);
            LockPlayersMovement(false);
        }

        StartEndingSequence();
    }

    public virtual void StartEndingSequence()
    {
        StartCoroutine(returnToMenuCooldown());
    }

    private IEnumerator returnToMenuCooldown()
    {
        yield return new WaitForSeconds(20f);
        MenuMusicController.Instance.StartMusic();
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        yield return new WaitForSeconds(2f);

        if (ModularController.Instance != null)
        {
            // Reset all player Points
            foreach (PlayerData data in PlayerDataHolder.Instance.GetPlayers())
            {
                data.points = 0;
            }
        }

        // Destroy the Object
        Destroy(ModularController.Instance.gameObject);
        SceneManager.LoadScene("GamemodeSelect");
    }

    private void LockPlayersMovement(bool value)
    {
        ThirdPersonMovement[] players = FindObjectsOfType<ThirdPersonMovement>();

        if (value)
        {
            foreach (ThirdPersonMovement player in players)
            {
                player.lockMovement = true;
            }
        }
        else
        {
            foreach (ThirdPersonMovement player in players)
            {
                player.lockMovement = false;
            }
        }
    }


    private void CalculateFinalPositionPlacing()
    {
        // Calculate
        //positions.Clear();
        newPositions.Clear();
        foreach (PlayerData player in PlayerDataHolder.Instance.GetPlayers())
        {
            newPositions.Add(player);
        }

        // Sort players based on points in descending order
        newPositions.Sort((a, b) => b.points.CompareTo(a.points)); // For ascending order, swap a and b
    }

    public void TriggerAnimations()
    {
        //Debug.Log("!!! Animation Triggered");
        // Case 1 Player for Debugging
        if (newPositions.Count == 1)
        {
            newPositions[0].input.GetComponentInChildren<AnimationStateController>().TriggerPodiumAnimation(true);
        }
        // Case More than 1 Player (Normal)
        else
        {
            newPositions[0].input.GetComponentInChildren<AnimationStateController>().TriggerPodiumAnimation(true);
            // Start from one loser
/*            for (int i = 1; i < newPositions.Count; i++)
            {
                newPositions[i].input.GetComponentInChildren<AnimationStateController>().TriggerPodiumAnimation(false);
            }*/
            newPositions[newPositions.Count - 1].input.GetComponentInChildren<AnimationStateController>().TriggerPodiumAnimation(false);
        }
    }
    private void SetUIMessage()
    {
        int index = 0;
        foreach (var player in newPositions)
        {
            podiumUIManager.TogglePlace(index, true);
            podiumUIManager.ChangePoints(index, player.points);
            index++;
        }
    }

    private void SetPlayerSpawns()
    {
        int index = 0;
        foreach (var player in newPositions)
        {
            podiumSpawnManager.SetPlayerSpawns(player, index);
            index++;
        }
    }
}
