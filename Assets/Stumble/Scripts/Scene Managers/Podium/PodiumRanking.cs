using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumRanking : MonoBehaviour
{
    [HideInInspector]
    public SortedDictionary<float, PlayerData> positions = new SortedDictionary<float, PlayerData>();
    public int count = 0;

    public static PodiumRanking Instance { get; private set; }

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

    public void UpdatePositions(SortedDictionary<float, PlayerData> positions)
    {
        this.positions.Clear();
        this.positions = positions;
        
        count = this.positions.Count;
    }
}
