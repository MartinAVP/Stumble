using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PodiumSpawnManager : MonoBehaviour
{
    public List<Transform> spawns;
    List<PlayerData> tempPlayerList = new List<PlayerData>();

    public static PodiumSpawnManager Instance { get; private set; }
    public bool initialized { get; private set; }

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

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (PodiumManager.Instance == null || PodiumManager.Instance.enabled == false || PodiumManager.Instance.initialized == false ||
            PodiumManager.Instance.lookingForSpawnManager == false)
        {
            await Task.Delay(1);
        }

        UnityEngine.Debug.Log("Initializing Podium Spawn Manager...         [Podium Spawn Manager]");

        //InitializePlayers();
        initialized = true;
        return;
    }

/*    public void InitializePlayers()
    {
        // Get all the players that joined and add them to the first checkpoint.
        // In addition get them to the position of spawning

        for (int i = 0; i < tempPlayerList.Count; i++)
        {
            Transform spawn = spawns[i].transform;
            // Parent Becomes spawn
            PlayerDataManager.Instance.GetPlayerData(tempPlayerList[i].GetID()).GetPlayerInScene().GetComponent<CharacterController>().enabled = false;
            PlayerDataManager.Instance.GetPlayerData(tempPlayerList[i].GetID()).GetPlayerInScene().transform.position = spawn.position;
            PlayerDataManager.Instance.GetPlayerData(tempPlayerList[i].GetID()).GetPlayerInScene().GetComponent<CharacterController>().enabled = true;
            PlayerDataManager.Instance.GetPlayerData(tempPlayerList[i].GetID()).GetPlayerInScene().transform.rotation = spawn.rotation;
        }
    }*/

    public void SetPlayerSpawns(PlayerData data, int id)
    {
        if (id <= spawns.Count) { 
            Transform spawn = spawns[id].transform;
            // Parent Becomes spawn
            PlayerDataHolder.Instance.GetPlayerData(data.input).GetPlayerInScene().GetComponent<CharacterController>().enabled = false;
            PlayerDataHolder.Instance.GetPlayerData(data.input).GetPlayerInScene().transform.position = spawn.position;
            PlayerDataHolder.Instance.GetPlayerData(data.input).GetPlayerInScene().transform.rotation = spawn.rotation;
            PlayerDataHolder.Instance.GetPlayerData(data.input).GetPlayerInScene().GetComponent<CharacterController>().enabled = true;
        }
    }
}
