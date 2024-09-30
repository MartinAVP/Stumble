using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CheckpointData
{
    public CheckpointData(int id, GameObject mainCheckpoint, List<Transform> checkpointSpawns, List<int> playerIDsOnCheckpoint)
    {
        this.id = id;
        this.mainCheckpoint = mainCheckpoint;
        this.checkpointSpawns = checkpointSpawns;
        this.playerIDsOnCheckpoint = playerIDsOnCheckpoint;
    }

    public int id;
    public GameObject mainCheckpoint;          // Holds the Trigger to grab the checkpoint
    public List<Transform> checkpointSpawns;  // Holds the possible spawn positions
    public List<int> playerIDsOnCheckpoint;   //

    public List<Transform> GetCheckpointSpawns()
    {
        return checkpointSpawns;
    }
    public int GetID()
    {
        return id;
    }
    public List<int> GetPlayersInCheckpoint()
    {
        return playerIDsOnCheckpoint;
    }
    public GameObject GetCheckpoint()
    {
        return mainCheckpoint;
    }

    public void addPlayer(int id)
    {
        playerIDsOnCheckpoint.Add(id);
    }
    public void removePlayer(int id)
    {
        playerIDsOnCheckpoint.Remove(id);
    }
}
