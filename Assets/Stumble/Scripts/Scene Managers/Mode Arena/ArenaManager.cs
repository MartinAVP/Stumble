using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    private Stopwatch stopwatch;
    public SortedDictionary<float, PlayerData> positions = new SortedDictionary<float, PlayerData>();

    //public event Action<SortedDictionary<float, PlayerData>> onBumpFinish;
    //[SerializeField] public event Action onArenaStart;
    public event Action onStartArena;

    public static ArenaManager Instance { get; private set; }

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
    }

    private void Start()
    {
        stopwatch = new Stopwatch();
        //StartRace();
        StartCoroutine(StartCinematic());
    }

    public IEnumerator StartCinematic()
    {
        LockPlayersMovement(true);

        CinematicController.Instance.StartTimeline();
        yield return new WaitForSeconds(CinematicController.Instance.GetTimelineLenght);
        StartRace();
    }

    public void StartRace()
    {
        // Lock all Player Movement
        foreach (PlayerData data in PlayerDataManager.Instance.GetPlayers())
        {
            data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().lockMovement = true;
        }
        onStartArena?.Invoke();
        StartCoroutine(ComenzeRacing());
    }

    public IEnumerator ComenzeRacing()
    {
        yield return new WaitForSeconds(5.0f);
        // Start the Timer
        stopwatch.Start();
        UnityEngine.Debug.LogWarning("Finalized Timer");

        // Unlock all Player Movement
        LockPlayersMovement(false);

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

        // Check if the last player reached the checkpoint
        if (positions.Count == PlayerDataManager.Instance.GetPlayers().Count)
        {
            onCompleteFinish.Invoke(positions);
        }
    }*/

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
}
