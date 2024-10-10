using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumSpawnManager : MonoBehaviour
{
    public List<Transform> spawns;
    List<PlayerData> tempPlayerList = new List<PlayerData>();

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

    //
    private void OnEnable()
    {
        GameController.Instance.startSecondarySystems += LateStart;
    }

    private void OnDisable()
    {
        GameController.Instance.startSecondarySystems -= LateStart;
    }

    // Start is called before the first frame update
    void LateStart()
    {
        if(PodiumRanking.Instance != null)
        {
            // Order player spawn based on podium
            foreach (float key in PodiumRanking.Instance.positions.Keys)
            {
                PlayerData value;
                if (PodiumRanking.Instance.positions.TryGetValue(key, out value))
                {
                    tempPlayerList.Add(value);
                }
                else
                {
                    //Console.WriteLine("Key " + key + " not found in the dictionary.");
                }
            }

        }
        else
        {
            tempPlayerList = PlayerDataManager.Instance.GetPlayers();
        }

        InitializePlayers();
        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.StartTransition(false);
        }
    }

    public void InitializePlayers()
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

            Vector3 offset = spawn.rotation * new Vector3(0, 3, -10); // 10m behind the player
            //tempPlayerList[i].GetPlayerInScene().transform.parent.GetComponentInChildren<CinemachineFreeLook>().ForceCameraPosition(spawn.position + offset, spawn.rotation); //
        }
    }

/*    public void RespawnPlayer(GameObject ob)
    {
        int index = UnityEngine.Random.Range(0, spawns.Count);
        Transform spawn = spawns[index].transform;

        ob.GetComponent<CharacterController>().enabled = false;
        ob.transform.position = spawn.position;
        ob.GetComponent<CharacterController>().enabled = true;
        ob.transform.rotation = spawn.rotation;
    }*/
}
