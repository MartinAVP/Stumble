using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArenaSpectator : MonoBehaviour
{
    private List<int> spectators = new List<int>();
    // First Value holds the player ID, Second Value holds the ID of the player is spectating
    private Dictionary<int, int> spectating = new Dictionary<int, int>();
    private int playerCount;

    public static ArenaSpectator Instance { get; private set; }

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
        playerCount = PlayerDataManager.Instance.GetPlayers().Count;
    }

    private void Update()
    {
        foreach (KeyValuePair<int, int> kvp in spectating)
        {
            //Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
            PlayerData targetData = PlayerDataManager.Instance.GetPlayerData(kvp.Value);
            PlayerData originData = PlayerDataManager.Instance.GetPlayerData(kvp.Key);

            GameObject player = originData.GetPlayerInScene();
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = targetData.GetPlayerInScene().transform.position;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }

    public void KillPlayer(GameObject player)
    {
        PlayerData data = PlayerDataManager.Instance.GetPlayerData(player.GetComponent<PlayerInput>());

        // Player already an spectator
        if (spectators.Contains(data.GetID())) { return; }
        if (playerCount - spectating.Count <= 1) { ArenaSpawnManager.Instance.RespawnPlayer(player); return; }

        spectators.Add(data.GetID());
        spectating.Add(data.GetID(), -1);
        spectating[data.GetID()] = GetNextAvailableIndex(spectating[data.GetID()]);

        // Check if its the last player

        //data.GetPlayerInScene().SetActive(false);
        data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().lockMovement = true;
        data.GetPlayerInScene().GetComponent<CapsuleCollider>().enabled = false;
        data.GetPlayerInScene().transform.GetChild(0).gameObject.SetActive(false);
    }

    private int GetNextAvailableIndex(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt + 1;
        // Loop through list
        if (index >= playerCount)
        {
            index = 0;
        }

        // Check if the index is busy
        if (IsSpectator(index))
        {
            index = GetNextAvailableIndex(index);
        }
        // 
        return index;
    }

    /// <summary>
    /// Gets the previous available color in the colors array that no player is using
    /// 
    /// !! Function is Recursive
    /// </summary>
    /// <param name="currentlyAt">The index of the colors array the player is currently at</param>
    /// <returns></returns>
/*    private int GetPreviousAvailableIndexColor(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt - 1;
        // Loop through list
        if (index <= colors.Count)
        {
            index = colors.Count;
        }

        // Check if the index is busy
        if (IsColorInUse(colors[index]))
        {
            index = GetPreviousAvailableIndexColor(index);
        }
        // 
        return index;
    }*/

    private bool IsSpectator(int id)
    {
        if (spectators.Contains(id))
        {
            return true;
        }
        return false;
    }
}
