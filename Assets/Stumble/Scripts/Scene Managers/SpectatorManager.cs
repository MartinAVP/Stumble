using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpectatorManager : MonoBehaviour
{
    public List<int> spectators = new List<int>();
    // First Value holds the player ID, Second Value holds the ID of the player is spectating
    private Dictionary<int, int> spectating = new Dictionary<int, int>();
    private int playerCount;

    public static SpectatorManager Instance { get; private set; }
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
        while (RacemodeManager.Instance == null || RacemodeManager.Instance.enabled == false || RacemodeManager.Instance.initialized == false)
        {
            // Await 5 ms and try finding it again.
            // It is made 5 seconds because it is
            // a core gameplay mechanic.
            await Task.Delay(2);
        }

        // Once it finds it initialize the scene
        Debug.Log("Initializing Spectator Manager...         [Spectator Manager]");
        initialized = true;

        InitializeManager();
        return;
    }

    private void InitializeManager()
    {
        playerCount = PlayerDataManager.Instance.GetPlayers().Count;

        spectating.Clear();
        spectators.Clear();
    }

    private void Update()
    {
        if (!initialized) { return; }
        foreach (KeyValuePair<int, int> kvp in spectating)
        {
            //Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
            PlayerData targetData = PlayerDataManager.Instance.GetPlayerData(kvp.Value);
            PlayerData originData = PlayerDataManager.Instance.GetPlayerData(kvp.Key);

            GameObject player = originData.GetPlayerInScene();
            //player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = targetData.GetPlayerInScene().transform.position;
            //player.GetComponent<CharacterController>().enabled = true;
        }
    }

    public void AddToSpectator(PlayerData data)
    {
        //PlayerData data = PlayerDataManager.Instance.GetPlayerData(player.GetComponent<PlayerInput>());

        // Player already an spectator
        if (spectators.Contains(data.GetID())) { return; }
        //if (playerCount - spectating.Count <= 1) { ArenaSpawnManager.Instance.RespawnPlayer(player); return; }

        spectators.Add(data.GetID());
        spectating.Add(data.GetID(), -1);

        // Check if its the last player
        spectating[data.GetID()] = GetNextAvailableIndex(spectating[data.GetID()]);

        // Add a subscription to the selection
        data.GetPlayerInScene().GetComponent<PlayerSelectAddon>().OnSelectInput.AddListener(SwitchSpectatingPlayer);
        Debug.LogWarning("Added Listener");

        //data.GetPlayerInScene().SetActive(false);
        data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().lockVeritcalMovement = true;
        data.GetPlayerInScene().GetComponent<CapsuleCollider>().enabled = false;
        data.GetPlayerInScene().transform.GetChild(0).gameObject.SetActive(false);

        // turn off the third person movenet - Test
        data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().enabled = false;
        data.GetPlayerInScene().GetComponent<CharacterController>().center = Vector3.up * 300;
        data.GetPlayerInScene().GetComponent<CharacterController>().enabled = false;

        Destroy(data.GetPlayerInScene().GetComponent<ThirdPersonMovement>());
        Destroy(data.GetPlayerInScene().GetComponent<CharacterController>());
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

    private int GetPreviousAvailableIndex(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt - 1;
        // Loop through list
        if (index <= spectators.Count)
        {
            index = spectators.Count;
        }

        // Check if the index is busy
        if (IsSpectator(index))
        {
            index = GetPreviousAvailableIndex(index);
        }
        // 
        return index;
    }

    private void SwitchSpectatingPlayer(Vector2 value, PlayerInput input)
    {
        Debug.Log("Listener Called");
        PlayerData data = PlayerDataManager.Instance.GetPlayerData(input);
        if (data == null)
        {
            Debug.LogError("The Device is not finding a player attached");
        }

        // Right Selection
        if (value.x > .5f)
        {
            spectating[data.GetID()] = GetNextAvailableIndex(spectating[data.GetID()]);
        }

        // Left Selection
        if(value.x < -.5f)
        {
            spectating[data.GetID()] = GetPreviousAvailableIndex(spectating[data.GetID()]);
        }

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

    public bool IsSpectator(int id)
    {
        Debug.Log("Is Spectator Called");
        if(spectators.Count == 0) { return false; }

        if (spectators.Contains(id))
        {
            return true;
        }
        Debug.Log("Is Spectator Ended");
        return false;
    }
}
