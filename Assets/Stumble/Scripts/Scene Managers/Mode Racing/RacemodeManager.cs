using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class RacemodeManager : MonoBehaviour
{
    private Stopwatch stopwatch;
    public SortedDictionary<float, PlayerData> positions = new SortedDictionary<float, PlayerData>();

    public event Action<SortedDictionary<float, PlayerData>> onCompleteFinish;
    public event Action onCountdownStart;
    public event Action onRaceStart;

    public static RacemodeManager Instance { get; private set; }
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

        setup();
    }

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (ExperimentalPlayerManager.Instance == null || ExperimentalPlayerManager.Instance.enabled == false || ExperimentalPlayerManager.Instance.finishedSystemInitializing == false)
        {
            // Await 5 ms and try finding it again.
            // It is made 5 seconds because it is
            // a core gameplay mechanic.
            await Task.Delay(2);
        }

        // Once it finds it initialize the scene
        Debug.Log("Initializing Racemode Manager...         [Racemode Manager]");
        //GameController.Instance.startSystems += LateStart;

        InitializeManager();
        initialized = true;
        return;
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
/*        if (initialized)
        {
            //GameController.Instance.startSystems -= LateStart;
        }*/
        
    }

    private void InitializeManager()
    {
        //UnityEngine.Debug.Log("Late Start Called on Race manager");
        stopwatch = new Stopwatch();

        // Note: In order to have a cinematic or countdown at the start,
        // Both the RacemodeUIManager and the CinematicController have to be in
        // in the scene, if one of them is missing the race will start without these.
        if (CinematicController.Instance != null && RacemodeUIManager.Instance != null)
        {
            // Note: If The Countdown values are not assigned in the UI Manager, then it will
            // skip the cinematic and the countdown.
            if (RacemodeUIManager.Instance.HasAllCountDownValues())
            {
                StartCoroutine(StartCinematic());
            }
            else
            {
                UnityEngine.Debug.LogWarning("RaceUIManager does not have all the countdown values, skipping countdown and cinematic.");
            }
        }
        // No Cinematic Controller in Scene
        else
        {
            // Start the Race Directly
            StartRace();
        }

        // Lock all players in place
        LockPlayersMovement(true);
    }

    public IEnumerator StartCinematic()
    {
        CinematicController.Instance.StartTimeline();
        yield return new WaitForSeconds(CinematicController.Instance.GetTimelineLenght);

        // On Cinematic End
        StartCoroutine(ComenzeRacing());
    }

    public IEnumerator ComenzeRacing()
    {
        onCountdownStart?.Invoke();
        yield return new WaitForSeconds(5.0f);
        StartRace();
    }

    public void StartRace()
    {
        // Invoke Event
        onRaceStart?.Invoke();

        // Start the Timer
        stopwatch.Start();
        UnityEngine.Debug.LogWarning("Race has been initialized");

        // Unlock all Player Movement
        LockPlayersMovement(false);
    }

    public void ReachFinishLine(PlayerData player)
    {

        // Check if the player has already finished
        foreach (PlayerData data in positions.Values)
        {
            // Player has already reached the finish line
            if (data == player) return;
        }

        // Add player to the finish
        positions.Add(GetElapsedTime(), player);
        UnityEngine.Debug.Log("Player #" + player.GetID() + " has reached the finish line in " + GetElapsedTimeString());

        // Freeze player position
        // Unprone
        if (player.GetPlayerInScene().GetComponent<ThirdPersonMovement>().isProne)
        {
            player.GetPlayerInScene().GetComponent<ThirdPersonMovement>().toggleProne(false);
        }
        // Lock Movement
        player.GetPlayerInScene().GetComponent<ThirdPersonMovement>().lockMovement = true;

        // Check if the last player reached the checkpoint
        if (positions.Count == PlayerDataManager.Instance.GetPlayers().Count)
        {
            StartCoroutine(EndGameDelay());
            onCompleteFinish?.Invoke(positions);
        }

        // Add a Listener to when the player wants to start spectating
        player.GetPlayerInScene().GetComponent<PlayerSelectAddon>().OnSelectInput.AddListener(startSpectating);
    }

    private IEnumerator EndGameDelay()
    {
        yield return new WaitForSeconds(4f);
        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.StartTransition(true);
        }
        // Start the brough Overs to the next scene
        GameObject ranking = new GameObject("Ranking");
        //ranking.AddComponent(typeof(PodiumRanking));
        PodiumRanking rank = ranking.AddComponent<PodiumRanking>();
        rank.UpdatePositions(this.positions);

        DontDestroyOnLoad(ranking);

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Podium");
    }

    private void startSpectating(Vector2 value, PlayerInput input)
    {
        if(value.x != 0)
        {
            UnityEngine.Debug.Log("Player wants to start spectating");
            PlayerData data = PlayerDataManager.Instance.GetPlayerData(input);
            if (data == null)
            {
                UnityEngine.Debug.LogError("The Device is not finding a player attached");
            }

            data.GetPlayerInScene().GetComponent<PlayerSelectAddon>().OnSelectInput.RemoveListener(startSpectating);
            SpectatorManager.Instance.AddToSpectator(data);
        }
    }

    public float GetElapsedTime()
    {
        return (float)stopwatch.Elapsed.TotalSeconds;
    }
    public string GetElapsedTimeString()
    {
        return $"{stopwatch.Elapsed.Hours:D2}:{stopwatch.Elapsed.Minutes:D2}:{stopwatch.Elapsed.Seconds:D2}.{stopwatch.Elapsed.Milliseconds:D3}";
    }

    private void LockPlayersMovement(bool value)
    {
        if (PlayerDataManager.Instance != null)
        {
            Debug.Log("Locking in " + PlayerDataManager.Instance.GetPlayers().Count);
            if (value)
            {
                foreach (PlayerData data in PlayerDataManager.Instance.GetPlayers())
                {
                    data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().lockMovement = true;
                }
            }
            else
            {
                foreach (PlayerData data in PlayerDataManager.Instance.GetPlayers())
                {
                    data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().lockMovement = false;
                }
            }
        }
        else // No Player Data Manager
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

    }
}
