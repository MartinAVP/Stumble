using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawnManager : MonoBehaviour
{

    //public List<Transform> Checkpoints;
    public List<CheckpointData> Checkpoints;

    public static ArenaSpawnManager Instance { get; private set; }

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

    private void OnEnable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.startSecondarySystems += LateStart;
        }
    }

    private void OnDisable()
    {
        Checkpoints.Clear();

        if (GameController.Instance != null)
        {
            GameController.Instance.startSecondarySystems -= LateStart;
        }
    }

    private void Start()
    {
        initializeCheckpoints();
        if (GameController.Instance == null)
        {
            InitializePlayers();
        }
    }

    private void LateStart()
    {
        InitializePlayers();
    }

    // Start is called before the first frame update

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

            Vector3 offset = spawn.rotation * new Vector3(0, 3, -10); // 10m behind the player
            tempPlayerList[i].GetPlayerInScene().transform.parent.GetComponentInChildren<CinemachineFreeLook>().ForceCameraPosition(spawn.position + offset, spawn.rotation);

            Checkpoints[0].addPlayer(tempPlayerList[i].GetID());
        }
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
                if (checkpoint.transform.GetChild(j).GetComponent<CheckpointSpawn>() != null)
                {
                    spawns.Add(checkpoint.transform.GetChild(j).transform);
                }
            }
            CheckpointData data = new CheckpointData(i, checkpoint, spawns, new List<int>());
            Checkpoints.Add(data);
        }
    }

/*    public void RespawnPlayer(GameObject ob)
    {
        int index = UnityEngine.Random.Range(0, Checkpoints.Count);
        Transform spawn = Checkpoints[index].transform;

        ob.GetComponent<CharacterController>().enabled = false;
        ob.transform.position = spawn.position;
        ob.GetComponent<CharacterController>().enabled = true;
        ob.transform.rotation = spawn.rotation;
    }*/


    public Transform GetNonBlockedSpawn()
    {
        // Check if spawn is blocked on random Selected Spawn
        //Debug.Log("Generated Random Spawn: " + randomSpawn);
        int randomSpawn = Random.Range(0, Checkpoints[0].checkpointSpawns.Count);
        bool spawnBlocked = true;
        //Debug.Log("Starting While Loop");
        while (spawnBlocked == true)
        {
            randomSpawn = Random.Range(0, Checkpoints[0].checkpointSpawns.Count);
            spawnBlocked = Checkpoints[0].checkpointSpawns[randomSpawn].GetComponent<CheckpointSpawn>().spawnBlocked;
            Debug.Log("Check spawn #" + randomSpawn + " and is blocked: " + spawnBlocked);
        }

        return Checkpoints[0].checkpointSpawns[randomSpawn];
        //return spawns[0];
    }
}
