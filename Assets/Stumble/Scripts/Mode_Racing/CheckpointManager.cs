using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckpointManager : MonoBehaviour
{
    public List<CheckpointData> Checkpoints;

    public static CheckpointManager Instance { get; private set; }

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

    private void OnDisable()
    {
        Checkpoints.Clear();
    }

    private void Start()
    {
        initializeCheckpoints();
        InitializePlayers();
    }


    
    public void initializeCheckpoints()
    {
        Checkpoints.Clear();
        // Get all checkpoints in scene and add them to the Data
        int chks = this.transform.childCount;

        // Each Checkpoint
        for (int i = 0; i < chks; i++)
        {
            GameObject checkpoint = this.transform.GetChild(i).gameObject;
            List<Transform> spawns = new List<Transform>();
            // Each Checkpoint Spawn
            for (int j = 0; j < checkpoint.transform.childCount; j++)
            {
                spawns.Add(checkpoint.transform.GetChild(j).transform);
            }
            CheckpointData data = new CheckpointData(i, checkpoint, spawns, new List<int>());
            Checkpoints.Add(data);
        }
    }
    public void InitializePlayers()
    {
        // Get all the players that joined and add them to the first checkpoint.
        // In addition get them to the position of spawning
        List<PlayerData> tempPlayerList = PlayerDataManager.Instance.GetPlayers();
        List<Transform> checkPointSpawns = Checkpoints[0].GetCheckpointSpawns();

        for (int i = 0; i < tempPlayerList.Count; i++)
        {
            Transform spawn = checkPointSpawns[i].transform;
            // Parent Becomes spawn
            tempPlayerList[i].GetPlayerInScene().GetComponent<CharacterController>().enabled = false;
            tempPlayerList[i].GetPlayerInScene().transform.position = spawn.position;
            tempPlayerList[i].GetPlayerInScene().GetComponent<CharacterController>().enabled = true;
            tempPlayerList[i].GetPlayerInScene().transform.rotation = spawn.rotation;

            Checkpoints[0].addPlayer(tempPlayerList[i].GetID());
        }
    }

    public void ReachCheckpoint(PlayerData data, GameObject checkpoint)
    {
        // Check if the checkpoint id is bigger than the previous one
        int playerID = data.GetID();
        int currentCheckpoint = findCheckpointPlayerIsin(playerID);
        int targetCheckpoint = GetCheckpointID(checkpoint);

        // Checkpoint is HIGHER than Current
        if (targetCheckpoint > currentCheckpoint)
        {
            Checkpoints[currentCheckpoint].removePlayer(playerID);
            Checkpoints[targetCheckpoint].addPlayer(playerID);
        }

        // Check if the checkpoint reached is the last one.
        if(targetCheckpoint == Checkpoints.Count - 1)
        {
            RaceManager.Instance.ReachFinishLine(data);
        }

    }

    public void Respawn(GameObject playerObject)
    {
        int playerID = PlayerDataManager.Instance.GetPlayerData(playerObject.GetComponent<PlayerInput>()).GetID();
        int currentCheckpoint = findCheckpointPlayerIsin(playerID);

        List<Transform> spawns = Checkpoints[currentCheckpoint].GetCheckpointSpawns();
        Transform spawn = GetNonBlockedSpawn(spawns);

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

        playerObject.transform.rotation = spawn.rotation;
        playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().m_YAxis.Value = .5f;
/*        playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().m_XAxis.Value = spawn.rotation.y;*/
        playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().ForceCameraPosition(spawn.position, spawn.rotation);

        Vector3 offset = spawn.rotation * new Vector3(0, 3, -10); // 10m behind the player
        playerObject.transform.parent.GetComponentInChildren<CinemachineFreeLook>().ForceCameraPosition(spawn.position + offset, spawn.rotation); //
    }

    private Transform GetNonBlockedSpawn(List<Transform> spawns)
    {
        // Check if spawn is blocked on random Selected Spawn
        //Debug.Log("Generated Random Spawn: " + randomSpawn);
        int randomSpawn = Random.Range(0, spawns.Count);
        bool spawnBlocked = true;
        //Debug.Log("Starting While Loop");
        while (spawnBlocked == true)
        {
            randomSpawn = Random.Range(0, spawns.Count);
            spawnBlocked = spawns[randomSpawn].GetComponent<CheckpointSpawn>().spawnBlocked;
            Debug.Log("Check spawn #" + randomSpawn + " and is blocked: " + spawnBlocked);
        }

        return spawns[randomSpawn];
        //return spawns[0];
    }

    private int findCheckpointPlayerIsin(int ID)
    {
        // Loop through each Checkpoint Data
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            if (Checkpoints[i].GetPlayersInCheckpoint().Contains(ID))
            {
                return Checkpoints[i].GetID();
            }
        }
        return -1;
    }

    private int GetCheckpointID(GameObject checkpoint)
    {
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            //
            if (Checkpoints[i].GetCheckpoint() == checkpoint)
            {
                return Checkpoints[i].GetID();
            }
        }
        return -1;
    }

    public void ForceReachNextCheckpoint(PlayerData data)
    {
        // Check if the checkpoint id is bigger than the previous one
        int playerID = data.GetID();
        int currentCheckpoint = findCheckpointPlayerIsin(playerID);
        int targetCheckpoint = findCheckpointPlayerIsin(playerID) + 1;

        // Checkpoint is HIGHER than Current
        Checkpoints[currentCheckpoint].removePlayer(playerID);
        Checkpoints[targetCheckpoint].addPlayer(playerID);

        // Check if the checkpoint reached is the last one.
        if (targetCheckpoint == Checkpoints.Count - 1)
        {
            RaceManager.Instance.ReachFinishLine(data);
        }
        Respawn(data.GetPlayerInScene());
    }
}