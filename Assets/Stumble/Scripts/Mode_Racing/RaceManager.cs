using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    private Stopwatch stopwatch;
    public SortedDictionary<float, PlayerData> positions = new SortedDictionary<float, PlayerData>();

    public event Action<SortedDictionary<float, PlayerData>> onCompleteFinish;
    public event Action onCountdownStart;
    public event Action onRaceStart;

    public static RaceManager Instance { get; private set; }

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

        // Lock all players in place
        LockPlayersMovement(true);

        // Check if Cinematic Controler Exists
        if (CinematicController.Instance != null)
        {
            StartCoroutine(StartCinematic());
        }
        // No Cinematic Controller in Scene
        else
        {
            // Start the Race Directly
            StartRace();
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
        stopwatch.Start();
        UnityEngine.Debug.LogWarning("Finalized Timer");

        // Unlock all Player Movement
        LockPlayersMovement(false);
    }

    public void ReachFinishLine(PlayerData player)
    {
        
        // Check if the player has already finished
        foreach(PlayerData data in positions.Values)
        {
            // Player has already reached the finish line
            if(data == player) return;
        }

        // Add player to the finish
        positions.Add(GetElapsedTime(), player);
        UnityEngine.Debug.Log("Player #" + player.GetID() + " has reached the finish line in " + GetElapsedTimeString());

        // Check if the last player reached the checkpoint
        if(positions.Count == PlayerDataManager.Instance.GetPlayers().Count)
        {
            onCompleteFinish?.Invoke(positions);
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
