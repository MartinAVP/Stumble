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
        stopwatch.Start();
    }

    private void Update()
    {
        // Optional: Update logic for your timer if needed

        //UnityEngine.Debug.Log(GetElapsedTimeString());
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

        if(positions.Count == PlayerDataManager.Instance.GetPlayers().Count)
        {
            onCompleteFinish.Invoke(positions);
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

}
