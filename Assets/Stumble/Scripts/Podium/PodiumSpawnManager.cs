using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumSpawnManager : MonoBehaviour
{
    public List<Transform> spawns;

    public static PodiumSpawnManager Instance { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {
        InitializePlayers();
    }

    public void InitializePlayers()
    {
        // Get all the players that joined and add them to the first checkpoint.
        // In addition get them to the position of spawning
        List<PlayerData> tempPlayerList = PlayerDataManager.Instance.GetPlayers();

        for (int i = 0; i < tempPlayerList.Count; i++)
        {
            Transform spawn = spawns[i].transform;
            // Parent Becomes spawn
            tempPlayerList[i].GetPlayerInScene().GetComponent<CharacterController>().enabled = false;
            tempPlayerList[i].GetPlayerInScene().transform.position = spawn.position;
            tempPlayerList[i].GetPlayerInScene().GetComponent<CharacterController>().enabled = true;
            tempPlayerList[i].GetPlayerInScene().transform.rotation = spawn.rotation;

            Vector3 offset = spawn.rotation * new Vector3(0, 3, -10); // 10m behind the player
            //tempPlayerList[i].GetPlayerInScene().transform.parent.GetComponentInChildren<CinemachineFreeLook>().ForceCameraPosition(spawn.position + offset, spawn.rotation); //
        }
    }

    public void RespawnPlayer(GameObject ob)
    {
        int index = UnityEngine.Random.Range(0, spawns.Count);
        Transform spawn = spawns[index].transform;

        ob.GetComponent<CharacterController>().enabled = false;
        ob.transform.position = spawn.position;
        ob.GetComponent<CharacterController>().enabled = true;
        ob.transform.rotation = spawn.rotation;
    }
}
