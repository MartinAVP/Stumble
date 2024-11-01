using Cinemachine;
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

public class ArenamodeManager : MonoBehaviour
{
    private Stopwatch stopwatch = new Stopwatch();
    public SortedDictionary<int, PlayerData> positions = new SortedDictionary<int, PlayerData>();

    public event Action<SortedDictionary<float, PlayerData>> onCompleteFinish;
    public event Action onLastManStanding;
    public event Action onCountdownStart;
    public event Action onArenaStart;

    private bool gameEnding = false;

    [Range(1, 8)]
    [SerializeField] private int Lives;
    private IDictionary<PlayerData, int> playerLives = new Dictionary<PlayerData, int>();

    public static ArenamodeManager Instance { get; private set; }
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
    private ArenaSpawnManager checkpointManager;
    private PlayerManager playerManager;
    private SpectatorManager speatorManager;

    // Secondary Systems - Not needed for the Game to Start, However will perform their duty if present.
    private ArenamodeUIManager arenamodeUIManager;
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
        Debug.Log("Found Game Controller...         [Arenamode Manager]");


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
        Debug.Log("Found Player Manager...         [Arenamode Manager]");

        // Await Checkpoint Manager to Initialize
        lookingForCheckpoint = true;
        while (ArenaSpawnManager.Instance == null || ArenaSpawnManager.Instance.enabled == false || ArenaSpawnManager.Instance.initialized == false)
        {
            await Task.Delay(2);
        }
        lookingForCheckpoint = false;
        checkpointManager = ArenaSpawnManager.Instance;
        Debug.Log("Found Arena Spawn Manager...         [Arenamode Manager]");

        // Await for Spectator Manager to Initialize.
        Debug.Log("Looking For Spectator Manager...         [Arenamode Manager]");
        lookingForSpectator = true;
        while (SpectatorManager.Instance == null || SpectatorManager.Instance.enabled == false || SpectatorManager.Instance.initialized == false)
        {
            await Task.Delay(1);
        }
        lookingForSpectator = false;
        speatorManager = SpectatorManager.Instance;
        Debug.Log("Found Spectator Manager...         [Arenamode Manager]");

        Debug.Log("Finished Initializing Core Systems");

        // ==== Secondary Systems ===============================
        // Racemode UI Manager
        if (ArenamodeUIManager.Instance != null && ArenamodeUIManager.Instance.enabled == true)
        {
            arenamodeUIManager = ArenamodeUIManager.Instance;
            Debug.Log("Found Arenamode UI Manager...         [Arenamode Manager]");
        }
        else
        {
            Debug.LogWarning("Racemode UI Manager Not found, Skipping...    [Arenamdoe Manager]");
        }

        // Cinematic Controller
        if (CinematicController.Instance != null && CinematicController.Instance.enabled == true)
        {
            cinematicController = CinematicController.Instance;
            Debug.Log("Found Cinematic Controller...         [Arenamode Manager]");
        }
        else
        {
            Debug.LogWarning("Cinematic Controller Not found, Skipping...    [Arenamode Manager]");
        }


        // Scoreboard Manager
        if (ScoreboardManager.Instance != null && ScoreboardManager.Instance.enabled == true)
        {
            scoreboardManager = ScoreboardManager.Instance;
            Debug.Log("Found Scoreboard Manager...         [Arenamode Manager]");
        }
        else
        {
            Debug.LogWarning("Scoreboard Manager not Found, Skipping...    [Arenamode Manager]");
        }

        // Music Manager
        if (GameMusicController.Instance != null && GameMusicController.Instance.enabled == true)
        {
            gameMusicController = GameMusicController.Instance;
            //gameMusicController.setup();
            Debug.Log("Found Game Music Controller...         [Arenamode Manager]");
        }
        else
        {
            Debug.LogWarning("Game Music Controller not Found, Skipping...    [Arenamode Manager]");
        }

        Debug.Log("End Task #1");
        AddPlayersToDictionary();
        Debug.Log("End Task #2");
        InitializeManager();
        Debug.Log("End Task #3");
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

        if (arenamodeUIManager != null)
        {
            Debug.Log("Initializing Race Countdown");
            //yield return new WaitForSeconds(5.0f);
            arenamodeUIManager.StartRace();
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
        onArenaStart?.Invoke();

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

    private void AddPlayersToDictionary()
    {
        // Add Player Lives
        foreach (PlayerData player in PlayerDataHolder.Instance.GetPlayers())
        {
            playerLives.Add(player, Lives + 1);
        }
        Debug.Log(playerLives.Count + " in the dictionary ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
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
        int index = 1;
        foreach (PlayerData player in positions.Values)
        {
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

    public void PlayerOnKillZone(GameObject playerObj)
    {

        // Remove a Life;
        if (PlayerDataHolder.Instance == null) { UnityEngine.Debug.LogError("Arena completely relies on player data holder"); return; }
        Debug.Log(playerLives.Count + "Dictionary Size #2");

        PlayerInput input = playerObj.GetComponent<PlayerInput>();
        PlayerData data = PlayerDataHolder.Instance.GetPlayerData(input);
        int id = input.playerIndex;
        //Debug.Log(id);
        // Remove a Life

        if (playerLives.ContainsKey(data))
        {
            Debug.LogWarning("The key is in the dictionary");
        }
        else
        {
            Debug.LogWarning("The key is not in the dictionary");

            foreach (var item in playerLives.Keys)
            {
                Debug.Log("LOGGER: " + item.id + " is in the dictionary");
            }
        }

        playerLives[data] = playerLives[data] - 1;
        int currentLives = playerLives[data];

/*        for (int i = 0; i < playerLives.Count; i++) {
            Debug.Log("LOGGER: Player #" + i + " now has " + playerLives[i] + " lives");
        }*/

        // Update UI
        //ArenamodeUIManager.Instance.UpdatePlayersAlive(GetPlayersAlive().ToString());
        Debug.Log("Player #" + id + " now has " + playerLives[data] + " lives");

        // End Game Check
        if (GetPlayersAlive() == 1)
        {
            Debug.Log("There is 1 player left");
            if (!gameEnding)
            {
                gameEnding = true;
                positions.Add(0, GetLastPlayer());
                scoreboardManager.UpdatePositions(positions);
                StartCoroutine(EndGameDelay());
                Respawn(playerObj);
/*                if(playerObj.GetComponent<ThirdPersonMovement>() != null)
                {
                    playerObj.GetComponent<ThirdPersonMovement>().lockVeritcalMovement = true;
                }*/
                onLastManStanding?.Invoke();
            }
        }

        if (IsLastPlayerStanding())
        {
            Debug.Log("This player is the last one standing");
            Respawn(playerObj);
            return;
        }

        // Player's Last Life.
        if (playerLives[data] <= 0)
        {
            Debug.Log("Making Player #" + id + "a spectator");
            //PlayerData data = PlayerDataHolder.Instance.GetPlayerData(playerObj.GetComponent<PlayerInput>());

            Debug.Log(data.GetPlayerInScene().name);
            positions.Add(GetPlayersAlive(), data);
            if (SpectatorManager.Instance != null)
            {
                SpectatorManager.Instance.AddToSpectator(data);
            }
            else
            {
                Debug.LogError("No Spectator Manager");
            }
        }
        else
        {
            Respawn(playerObj);
        }
    }

/*    public void ReachFinishLine(PlayerData player)
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
            //Debug.Log("Race Values: " + positions.Count);
            scoreboardManager.UpdatePositions(positions);
            StartCoroutine(EndGameDelay());
            onCompleteFinish?.Invoke(positions);
        }

        // Add a Listener to when the player wants to start spectating
        player.GetPlayerInScene().GetComponent<PlayerSelectAddon>().OnSelectInput.AddListener(startSpectating);
    }*/

    public void Respawn(GameObject playerObject)
    {
        int playerID = PlayerDataHolder.Instance.GetPlayerData(playerObject.GetComponent<PlayerInput>()).GetID();
        //int currentCheckpoint = findCheckpointPlayerIsin(playerID);

        //List<Transform> spawns = Checkpoints[currentCheckpoint].GetCheckpointSpawns();
        Transform spawn = ArenaSpawnManager.Instance.GetNonBlockedSpawn();
        //GetNonBlockedSpawn(spawns);

        //
        // BIG NOTE: Unity has a bug with the character controller, while enabled the position of the
        //           player cannot change if the character controller is enabled. A small disable
        //           fixes the problem.
        // 
        // Fix Link: https://discussions.unity.com/t/transform-position-does-not-work/802628/14
        //

        playerObject.GetComponent<CharacterController>().enabled = false;
        playerObject.transform.position = spawn.position;
        playerObject.GetComponent<CharacterController>().enabled = true;

        // Loose all momentum on Respawn & Unprone
        if (playerObject.GetComponent<ThirdPersonMovement>().isProne)
        {
            playerObject.GetComponent<ThirdPersonMovement>().toggleProne(false);
        }
        playerObject.GetComponent<ThirdPersonMovement>().horizontalVelocity = 0;
        playerObject.GetComponent<ThirdPersonMovement>().verticalVelocity = 0;

        // Adjust the rotation to the spawn rotation
        playerObject.transform.rotation = spawn.rotation;
        playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().m_YAxis.Value = .5f;
        //*        playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().m_XAxis.Value = spawn.rotation.y;
        playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().ForceCameraPosition(spawn.position, spawn.rotation);

        Vector3 offset = spawn.rotation * new Vector3(0, 3, -10); // 10m behind the player
        playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().ForceCameraPosition(spawn.position + offset, spawn.rotation); //
    }

    private bool IsLastPlayerStanding()
    {
        bool lastPlayer = true;
        foreach (int lives in playerLives.Values)
        {
            if (lives > 0)
            {
                lastPlayer = false;
                break;
            }
        }
        return lastPlayer;

        //*        int aliveCount = 0;
/*
            foreach (var lives in playerLives.Values)
        {
            if (lives > 0)
            {
                aliveCount++;
            }
        }

        return aliveCount == 1; */
        // Return true if only one player is alive*//*
    }

/*    private IEnumerator EndGameDelay()
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
    }*/

    private int GetPlayersAlive()
    {
        int alive = 0;
        foreach (int lives in playerLives.Values)
        {
            if (lives > 0)
            {
                alive++;
            }

        }
        return alive;
    }

    private PlayerData GetLastPlayer()
    {
        foreach (PlayerData playerID in playerLives.Keys)
        {
            if (playerLives[playerID] > 0)
            {
                return playerID;
            }
        }

        return null;
    }

    /*    public event Action<SortedDictionary<float, PlayerData>> onCompleteFinish;
        public event Action onCountdownStart;
        public event Action onRaceStart;
        public event Action onLastManStanding;

        [Range (1,8)]
        [SerializeField] private int Lives;
        private IDictionary<int, int> playerLives = new Dictionary<int, int>();

        public SortedDictionary<float, PlayerData> positions = new SortedDictionary<float, PlayerData>();

        private bool ending = false;

        public static ArenamodeManager Instance { get; private set; }
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

        // Start is called before the first frame update
        private void OnEnable()
        {
            GameController.Instance.startSystems += LateStart;
            GameController.Instance.startSecondarySystems += LateSecondaryStart;
        }

        private void OnDisable()
        {
            GameController.Instance.startSystems -= LateStart;
            GameController.Instance.startSecondarySystems -= LateSecondaryStart;

        }

        private void LateStart()
        {
            // Note: In order to have a cinematic or countdown at the start,
            // Both the RacemodeUIManager and the CinematicController have to be in
            // in the scene, if one of them is missing the race will start without these.
            if (CinematicController.Instance != null && ArenamodeUIManager.Instance != null)
            {
                // Note: If The Countdown values are not assigned in the UI Manager, then it will
                // skip the cinematic and the countdown.
                if (ArenamodeUIManager.Instance.HasAllCountDownValues())
                {
                    StartCoroutine(StartCinematic());
                }
                else
                {
                    //UnityEngine.Debug.LogWarning("RaceUIManager does not have all the countdown values, skipping countdown and cinematic.");
                }
            }
            // No Cinematic Controller in Scene
            else
            {
                // Start the Race Directly
                StartRace();
            }

        }

        private void LateSecondaryStart()
        {
            InitializePlayerData();

            // Update UI
            ArenamodeUIManager.Instance.UpdatePlayersAlive(GetPlayersAlive().ToString());
        }

        private void InitializePlayerData()
        {
            if (PlayerDataManager.Instance != null)
            {

                List<PlayerData> data = PlayerDataManager.Instance.GetPlayers();

                foreach (PlayerData playerData in data)
                {
                    playerLives.Add(playerData.GetPlayerInScene().GetComponent<PlayerInput>().playerIndex, Lives);
                }
            }
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
            //stopwatch.Start();
            UnityEngine.Debug.LogWarning("Race has been initialized");

            // Unlock all Player Movement
            */
    /*        LockPlayersMovement(false);*//*
        }

        // Update is called once per frame


        public void PlayerOnKillZone(GameObject playerObj)
        {

            // Remove a Life;
            if (PlayerDataManager.Instance == null) { UnityEngine.Debug.LogError("Arena completely relies on player data manager"); return; }


            int id = playerObj.GetComponent<PlayerInput>().playerIndex;
            playerLives[id] = playerLives[id] - 1;
            UnityEngine.Debug.Log("Player #" + id + " now has " + playerLives[id]);
            int currentLives = playerLives[playerObj.GetComponent<PlayerInput>().playerIndex];

            // Update UI
            ArenamodeUIManager.Instance.UpdatePlayersAlive(GetPlayersAlive().ToString());

            // End Game Check
            if (GetPlayersAlive() == 1)
            {
                positions.Add(0, PlayerDataManager.Instance.GetPlayerData(GetLastPlayer()));
                StartCoroutine(EndGameDelay());
                onLastManStanding?.Invoke();
            }

            if (IsLastPlayerStanding())
            {
                Respawn(playerObj);
                return;
            }

            if(currentLives <= 0)
            {
                //Debug.Log("Making Player #" + id + "a spectator");
                PlayerData data = PlayerDataManager.Instance.GetPlayerData(playerObj.GetComponent<PlayerInput>());
    */
    /*            Debug.Log(data.GetID());
                Debug.Log(data.GetPlayerInScene().name);*/
    /*
                positions.Add(GetPlayersAlive(), data);
                if (SpectatorManager.Instance != null)
                {
                    SpectatorManager.Instance.AddToSpectator(data);
                }
                else
                {
                    Debug.LogError("No Spectator Manager");
                }
            }
            else
            {
                Respawn(playerObj);
            }
        }

        public void Respawn(GameObject playerObject)
        {
            int playerID = PlayerDataManager.Instance.GetPlayerData(playerObject.GetComponent<PlayerInput>()).GetID();
            //int currentCheckpoint = findCheckpointPlayerIsin(playerID);

            //List<Transform> spawns = Checkpoints[currentCheckpoint].GetCheckpointSpawns();
            Transform spawn = ArenaSpawnManager.Instance.GetNonBlockedSpawn();
                              //GetNonBlockedSpawn(spawns);

            //
            // BIG NOTE: Unity has a bug with the character controller, while enabled the position of the
            //           player cannot change if the character controller is enabled. A small disable
            //           fixes the problem.
            // 
            // Fix Link: https://discussions.unity.com/t/transform-position-does-not-work/802628/14
            //

            playerObject.GetComponent<CharacterController>().enabled = false;
            playerObject.transform.position = spawn.position;
            playerObject.GetComponent<CharacterController>().enabled = true;

            // Loose all momentum on Respawn & Unprone
            if (playerObject.GetComponent<ThirdPersonMovement>().isProne)
            {
                playerObject.GetComponent<ThirdPersonMovement>().toggleProne(false);
            }
            playerObject.GetComponent<ThirdPersonMovement>().horizontalVelocity = 0;
            playerObject.GetComponent<ThirdPersonMovement>().verticalVelocity = 0;

            // Adjust the rotation to the spawn rotation
            playerObject.transform.rotation = spawn.rotation;
            playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().m_YAxis.Value = .5f;
            */
    /*        playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().m_XAxis.Value = spawn.rotation.y;*//*
            playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().ForceCameraPosition(spawn.position, spawn.rotation);

            Vector3 offset = spawn.rotation * new Vector3(0, 3, -10); // 10m behind the player
            playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().ForceCameraPosition(spawn.position + offset, spawn.rotation); //
        }

        private bool IsLastPlayerStanding()
        {
            bool lastPlayer = true;
            foreach(int lives in playerLives.Values)
            {
                if(lives > 0)
                {
                    lastPlayer = false;
                    break;
                }
            }
            return lastPlayer;

    *//*        int aliveCount = 0;

            foreach (var lives in playerLives.Values)
            {
                if (lives > 0)
                {
                    aliveCount++;
                }
            }

            return aliveCount == 1; // Return true if only one player is alive*//*
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

        private int GetPlayersAlive()
        {
            int alive = 0;
            foreach (int lives in playerLives.Values)
            {
                if (lives > 0)
                {
                    alive++;
                }

            }
            return alive;
        }

        private int GetLastPlayer()
        {
            foreach (int playerID in playerLives.Keys)
            {
                if (playerLives[playerID] > 0)
                {
                    return playerID;
                }
            }
            return -1;
        }*/
}
