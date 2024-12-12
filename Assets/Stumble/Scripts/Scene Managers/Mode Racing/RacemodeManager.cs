using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
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
    private GameMusicController gameMusicController;

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

        // Music Manager
        if (GameMusicController.Instance != null && GameMusicController.Instance.enabled == true)
        {
            gameMusicController = GameMusicController.Instance;
            Debug.Log("Found Game Music Controller...         [Racemode Manager]");
        }
        else
        {
            Debug.LogWarning("Game Music Controller not Found, Skipping...    [Racemode Manager]");
        }

        InitializeManager();
        return;
    }

    private void InitializeManager()
    {
        // Lock all players in place
        Debug.Log("PrePreTask");
        LockPlayersMovement(true);
        LockPlayersView(true);

        Debug.Log("PreTask");
        StartCoroutine(MainRaceController());
    }

    private IEnumerator MainRaceController()
    {
        // Base Delay
        Debug.Log("InTask");
        yield return new WaitForSeconds(.1f);



        if (cinematicController != null)
        {
            Debug.Log("Initializing Cinematic");
            //GameMusicController.Instance?.InitializeCinematicMusic();
            if(gameMusicController != null)
            {
                gameMusicController?.InitializeCinematicMusic();
            }
            yield return new WaitForSeconds(.2f);
            cinematicController.StartTimeline();
            yield return new WaitForSeconds(.1f);
            if (LoadingScreenManager.Instance != null)
            {
                LoadingScreenManager.Instance.StartTransition(false);
            }
            yield return new WaitForSeconds(cinematicController.GetTimelineLenght.ConvertTo<int>());
            //await Task.Delay(cinematicController.GetTimelineLenght.ConvertTo<int>() * 1000);

            // On Cinematic End
        }
        else
        {
            if (LoadingScreenManager.Instance != null)
            {
                LoadingScreenManager.Instance.StartTransition(false);
            }
        }

        onCountdownStart?.Invoke();
        LockPlayersView(false);

        if (racemodeUIManager != null)
        {
            if(gameMusicController != null)
            {
                gameMusicController.InitializeCountdown();
            }
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

    private IEnumerator LoadingScreenManagerDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    public void StartRace()
    {
        // Invoke Event
        onRaceStart?.Invoke();

        if (gameMusicController != null) {
            gameMusicController.InitializeGameMusic();
        }

        // Start the Timer
        stopwatch.Start();
        //Debug.Log(GetElapsedTime());
        //UnityEngine.Debug.LogWarning("Race has been initialized");

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

    private void LockPlayersView(bool value)
    {
        ThirdPersonMovement[] players = FindObjectsOfType<ThirdPersonMovement>();

        if (value)
        {
            foreach (ThirdPersonMovement player in players)
            {
                player.camInputHandler.lockView = true;
            }
        }
        else
        {
            foreach (ThirdPersonMovement player in players)
            {
                player.camInputHandler.lockView = false;
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
        if (!positions.ContainsValue(player))
        {
            positions.TryAdd(GetElapsedTime(), player);
        }
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
            //Debug.Log("Race Values: " + positions.Count);
            //scoreboardManager.UpdatePositionsFromTime(positions);
            scoreboardManager.UpdatePositionsFromRace(positions);

            StartCoroutine(EndGameDelay());
            onCompleteFinish?.Invoke(positions);

            return;
        }

        // Add a Listener to when the player wants to start spectating
        player.GetPlayerInScene().GetComponent<PlayerSelectAddon>().OnSelectInput.AddListener(startSpectating);
    }

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

        yield return new WaitForSeconds(1.5f);

        // Check if the Modular Controller Exists
        if (ModularController.Instance != null)
        {
            ModularController.Instance.AdvanceLevels();
        }
    }

    private void AddPoints()
    {
        /*        foreach (KeyValuePair<float, PlayerData> data in positions)
                {
                    int key = data.Key;       // Get the key
                    string value = data.Value; // Get the associated value



                    Console.WriteLine($"Key: {key}, Value: {value}");
                }*/
/*        int index = 1;
        foreach (PlayerData player in positions.Values) {
            scoreboardManager.SetPoints(player.id, index, 0);
            index++;
        }*/

        ScoreboardManager.Instance.AddMatchPoints();
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

    private GUIStyle panelStyle;
    private Rect panelRect = new Rect(10, 10, 300, 550);

    private void Start()
    {
        panelStyle = new GUIStyle();
        panelStyle.normal.background = MakeTexture(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.75f));
        panelStyle.padding = new RectOffset(10, 10, 10, 10); 
        panelStyle.normal.textColor = Color.white; 
        panelStyle.fontSize = 38; 
    }

    void OnGUI()
    {
        if (PlayerDataHolder.Instance != null) { 
        
            GUI.BeginGroup(panelRect, panelStyle);
            foreach (var player in PlayerDataHolder.Instance.GetPlayers()) { 
                GUILayout.Label("Player #" + player.id, GUILayout.Height(30));
                GUILayout.Label("Score: " +player.points);
                GUILayout.Label("");
            }
            GUI.EndGroup();
        }

    }

    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }
}
