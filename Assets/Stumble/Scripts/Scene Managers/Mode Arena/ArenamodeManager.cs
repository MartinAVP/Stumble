using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class ArenamodeManager : MonoBehaviour
{
    public event Action<SortedDictionary<float, PlayerData>> onCompleteFinish;
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
        /*        LockPlayersMovement(false);*/
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
/*            Debug.Log(data.GetID());
            Debug.Log(data.GetPlayerInScene().name);*/
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
        /*        playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().m_XAxis.Value = spawn.rotation.y;*/
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

/*        int aliveCount = 0;

        foreach (var lives in playerLives.Values)
        {
            if (lives > 0)
            {
                aliveCount++;
            }
        }

        return aliveCount == 1; // Return true if only one player is alive*/
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
    }
}
