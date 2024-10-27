using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class RacemodeManager : MonoBehaviour
{
    private Stopwatch stopwatch = new Stopwatch();
    public SortedDictionary<float, PlayerData> positions = new SortedDictionary<float, PlayerData>();

    public event Action<SortedDictionary<float, PlayerData>> onCompleteFinish;
    public event Action onCountdownStart;
    public event Action onRaceStart;

    private bool foundCinematicController = false;
    private bool foundRaceUIManager = false;

    public static RacemodeManager Instance { get; private set; }
    public bool initialized { get; private set; }


    public bool lookingForCheckpoint { get; private set; } = false;
    public bool lookingForPlayer { get; private set; } = false;
    public bool lookingForSpectator { get; private set; } = false;

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

    // Game Controller
    private GameController gameController;

    // Primary Systems - Needed for the Game to Start
    private CheckpointManager checkpointManager;
    private PlayerManager playerManager;
    private SpectatorManager speatorManager;

    // Secondary Systems - Not needed for the Game to Start, However will perform their duty if present.
    private RacemodeUIManager racemodeUIManager;
    private CinematicController cinematicController;
    private ScoreboardManager scoreboardManager;

    private async Task setup()
    {
        // Wait for the Game Controller To Initialize, The Race cannot start without a Game Controller.
        while (GameController.Instance == null || GameController.Instance.enabled == false || GameController.Instance.initialized == false)
        {
            await Task.Delay(1);
        }
        gameController = GameController.Instance;
        Debug.Log("Found Game Controller...         [Racemode Manager]");


        initialized = true;     // Initialized Means that systems can now be Started.
                                // Initialized is a validartor variable that is set to true once the system
                                // is fully operational.


        // ==== Primary Systems ===============================
        // Await Player Manager to Initialize
        lookingForPlayer = true;
        while (PlayerManager.Instance == null || PlayerManager.Instance.enabled == false || PlayerManager.Instance.initialized == false)
        {
            await Task.Delay(2);
        }
        lookingForPlayer = false;
        playerManager = PlayerManager.Instance;
        Debug.Log("Found Player Manager...         [Racemode Manager]");

        // Await Checkpoint Manager to Initialize
        lookingForCheckpoint = true;
        while (CheckpointManager.Instance == null || CheckpointManager.Instance.enabled == false || CheckpointManager.Instance.initialized == false)
        {
            await Task.Delay(2);
        }
        lookingForCheckpoint = false;
        checkpointManager = CheckpointManager.Instance;
        Debug.Log("Found Checkpoint Manager...         [Racemode Manager]");

        // Await for Spectator Manager to Initialize.
        lookingForSpectator = true;
        while (SpectatorManager.Instance == null || SpectatorManager.Instance.enabled == false || SpectatorManager.Instance.initialized == false)
        {
            await Task.Delay(2);
        }
        lookingForSpectator = false;
        speatorManager = SpectatorManager.Instance;
        Debug.Log("Found Spectator Manager...         [Racemode Manager]");

        Debug.Log("Finished Initializing Core Systems");

        // ==== Secondary Systems ===============================
        // Racemode UI Manager
        if(RacemodeUIManager.Instance != null && RacemodeUIManager.Instance.enabled == true)
        {
            racemodeUIManager = RacemodeUIManager.Instance;
            Debug.Log("Found Racemode UI Manager...         [Racemode Manager]");
        }
        else
        {
            Debug.LogWarning("Racemode UI Manager Not found, Skipping...    [Racemode Manager]");
        }

        // Cinematic Controller
        if (CinematicController.Instance != null && CinematicController.Instance.enabled == true)
        {
            cinematicController = CinematicController.Instance;
            Debug.Log("Found Cinematic Controller...         [Racemode Manager]");
        }
        else
        {
            Debug.LogWarning("Cinematic Controller Not found, Skipping...    [Racemode Manager]");
        }


        // Scoreboard Manager
        if (ScoreboardManager.Instance != null && ScoreboardManager.Instance.enabled == true)
        {
            scoreboardManager = ScoreboardManager.Instance;
            Debug.Log("Found Scoreboard Manager...         [Racemode Manager]");
        }
        else
        {
            Debug.LogWarning("Scoreboard Manager not Found, Skipping...    [Racemode Manager]");
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
/*
    private async Task MainRaceController()
    {
        // Base Delay
        Debug.Log("InTask");
        LockPlayersMovement(true);
        await Task.Delay(5);

        if (cinematicController != null)
        {
            Debug.Log("Initializing Cinematic");
            cinematicController.StartTimeline();
            await Task.Delay(cinematicController.GetTimelineLenght.ConvertTo<int>() * 1000);

            // On Cinematic End
        }
        onCountdownStart?.Invoke();

        if (racemodeUIManager != null)
        {
            Debug.Log("Initializing Race Countdown");
            //yield return new WaitForSeconds(5.0f);
            racemodeUIManager.StartRace();
            await Task.Delay(5 * 1000);
        }

        StartRace();
    }*/

    private IEnumerator MainRaceController()
    {
        // Base Delay
        Debug.Log("InTask");
        yield return new WaitForSeconds(.1f);

        if (cinematicController != null)
        {
            Debug.Log("Initializing Cinematic");
            cinematicController.StartTimeline();
            yield return new WaitForSeconds(cinematicController.GetTimelineLenght.ConvertTo<int>());
            //await Task.Delay(cinematicController.GetTimelineLenght.ConvertTo<int>() * 1000);

            // On Cinematic End
        }
        onCountdownStart?.Invoke();

        if (racemodeUIManager != null)
        {
            Debug.Log("Initializing Race Countdown");
            //yield return new WaitForSeconds(5.0f);
            racemodeUIManager.StartRace();
            yield return new WaitForSeconds(5f);
            //await Task.Delay(5 * 1000);
        }

        StartRace();
    }

    /*    public IEnumerator StartCinematic()
        {
            yield return new WaitForEndOfFrame();
            if (foundCinematicController)
            {
                CinematicController.Instance.StartTimeline();
                yield return new WaitForSeconds(CinematicController.Instance.GetTimelineLenght);

                // On Cinematic End
            }
            yield return new WaitForSeconds(0);
            StartCoroutine(ComenzeRacing());
        }

        public IEnumerator ComenzeRacing()
        {
            Debug.Log("Racing Comenzed");
            onCountdownStart?.Invoke();

            if (foundRaceUIManager)
            {
                yield return new WaitForSeconds(5.0f);
            }

            StartRace();
        }*/

    public void StartRace()
    {
        // Invoke Event
        onRaceStart?.Invoke();

        // Start the Timer
        stopwatch.Start();
        Debug.Log(GetElapsedTime());
        UnityEngine.Debug.LogWarning("Race has been initialized");

        // Unlock all Player Movement
        LockPlayersMovement(false);
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
/*        if (PlayerDataHolder.Instance != null)
        {
            Debug.Log("Locking in " + PlayerDataHolder.Instance.GetPlayers().Count);
            if (value)
            {
                foreach (PlayerData data in PlayerDataHolder.Instance.GetPlayers())
                {
                    data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().lockMovement = true;
                }
            }
            else
            {
                foreach (PlayerData data in PlayerDataHolder.Instance.GetPlayers())
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
        }*/
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
        if (positions.Count == PlayerDataHolder.Instance.GetPlayers().Count)
        {
            // End
            // Display the Scores
            Debug.Log("Race Values: " + positions.Count);
            scoreboardManager.UpdatePositionsFromTime(positions);
            StartCoroutine(EndGameDelay());
            onCompleteFinish?.Invoke(positions);
        }

        // Add a Listener to when the player wants to start spectating
        player.GetPlayerInScene().GetComponent<PlayerSelectAddon>().OnSelectInput.AddListener(startSpectating);
    }

    private PlayerUIComponent uiComponent;

    private IEnumerator EndGameDelay()
    {
        yield return new WaitForEndOfFrame();
        scoreboardManager.DisplayScoreboard();
/*        UpdateScoresToAllPlayers(0);
        DisplayScoresToAllPlayers();*/
        yield return new WaitForSeconds(4f);
        //uiComponent.UpdateScore(12, 3);
        AddPoints();
        scoreboardManager.UpdateUIPoints();
        yield return new WaitForSeconds(10f);

        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.StartTransition(true);
        }
        if (GameMusicController.Instance != null)
        {
            GameMusicController.Instance.EndMusic(1.2f);
        }

        // Start the brough Overs to the next scene
/*        GameObject ranking = new GameObject("Ranking");
        //ranking.AddComponent(typeof(PodiumRanking));
        PodiumRanking rank = ranking.AddComponent<PodiumRanking>();
        rank.UpdatePositions(this.positions);

        DontDestroyOnLoad(ranking);*/
        
        if(ModularController.Instance != null)
        {
            ModularController.Instance.AdvanceLevels();
        }

        yield return new WaitForSeconds(1.5f);
        //SceneManager.LoadScene("Podium");
    }
    /*    private void DisplayScoresToAllPlayers() { 
            foreach(PlayerData player in PlayerDataHolder.Instance.GetPlayers())
            {
                player.playerInScene.GetComponentInChildren<PlayerUIComponent>().DisplayScores();
            }
        }
        private void UpdateScoresToAllPlayers(float duration)
        {
            foreach (PlayerData player in PlayerDataHolder.Instance.GetPlayers())
            {
                player.playerInScene.GetComponentInChildren<PlayerUIComponent>().UpdateScore(player.points, duration);
            }
        }*/
    /*    public List<UICameraView> GetCharacterImages()
        {
            List<UICameraView> uiCamViews = new List<UICameraView>();

            foreach (PlayerData player in PlayerDataHolder.Instance.GetPlayers())
            {
                uiCamViews.Add(player.playerInScene.GetComponentInChildren<UICameraView>());
            }

            return uiCamViews;
        }
    */
    /*    private void SetPlayerScores()
        {
            int index = 0;
            foreach(PlayerData data in positions.Values)
            {
                switch (index)
                {
                    case 0:

                        break;
                }
            }
        }*/

    private void AddPoints()
    {
        /*        foreach (KeyValuePair<float, PlayerData> data in positions)
                {
                    int key = data.Key;       // Get the key
                    string value = data.Value; // Get the associated value



                    Console.WriteLine($"Key: {key}, Value: {value}");
                }*/
        int index = 1;
        foreach (PlayerData player in positions.Values) {
            scoreboardManager.SetPoints(player.id, index, 0);
            index++;
        }
    }
    private void startSpectating(Vector2 value, PlayerInput input)
    {
        if (value.x != 0)
        {
            UnityEngine.Debug.Log("Player wants to start spectating");
            PlayerData data = PlayerDataHolder.Instance.GetPlayerData(input);
            if (data == null)
            {
                UnityEngine.Debug.LogError("The Device is not finding a player attached");
            }

            data.GetPlayerInScene().GetComponent<PlayerSelectAddon>().OnSelectInput.RemoveListener(startSpectating);
            SpectatorManager.Instance.AddToSpectator(data);
        }
    }
}
