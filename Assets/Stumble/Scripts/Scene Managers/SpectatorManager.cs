using Cinemachine;
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

    private GameController gameController;

    public static SpectatorManager Instance { get; private set; }
    public bool initialized { get; private set; }

    // Singleton
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            //Destroy(this);
        }
        else
        {
            Instance = this;
        }

        Task Setup = setup();
    }

    private async Task setup()
    {
        while (GameController.Instance == null || GameController.Instance.enabled == false || GameController.Instance.initialized == false)
        {
            await Task.Delay(1);
        }
        gameController = GameController.Instance;

/*        if(gameController.gameState == GameState.Race)
        {
            while (RacemodeManager.Instance == null || RacemodeManager.Instance.enabled == false || RacemodeManager.Instance.lookingForSpectator == false)
            {
                await Task.Delay(1);
            }
        }
        if (gameController.gameState == GameState.Arena)
        {
            while (ArenamodeManager.Instance == null || ArenamodeManager.Instance.enabled == false || ArenamodeManager.Instance.lookingForSpectator == false)
            {
                await Task.Delay(1);
            }
        }*/

        // Once it finds it initialize the scene
        InitializeManager();
        Debug.Log("Initializing Spectator Manager...         [Spectator Manager]");
        initialized = true;

        return;
    }

    private void InitializeManager()
    {
        playerCount = PlayerDataHolder.Instance.GetPlayers().Count;

        spectating.Clear();
        spectators.Clear();
    }

    private void Update()
    {
/*        if (!initialized) { return; }
        foreach (KeyValuePair<int, int> kvp in spectating)
        {
            //Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
*//*            PlayerData targetData = PlayerDataHolder.Instance.GetPlayerData(kvp.Value);
            PlayerData originData = PlayerDataHolder.Instance.GetPlayerData(kvp.Key);*//*

            GameObject player = originData.GetPlayerInScene();
            //player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = targetData.GetPlayerInScene().transform.position;
            //player.GetComponent<CharacterController>().enabled = true;
        }*/
    }

    public void AddToSpectator(PlayerData data)
    {
        // Player already an spectator
        if (spectators.Contains(data.GetID())) { return; }

        spectators.Add(data.GetID());
        spectating.Add(data.GetID(), -1);

        // Check if its the last player
        int index = GetNextAvailableIndex(spectating[data.GetID()], 0);
        if(index == -1) { return; }
        spectating[data.GetID()] = index;

        // Add a subscription to the selection
        data.GetPlayerInScene().GetComponent<PlayerSelectAddon>().OnSelectInput.AddListener(SwitchSpectatingPlayer);
        Debug.LogWarning("Added Listener");

        //data.GetPlayerInScene().SetActive(false);
        data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().lockVeritcalMovement = true;
        data.GetPlayerInScene().GetComponent<CapsuleCollider>().enabled = false;

        int id = spectating[data.GetID()];

        data.GetPlayerInScene().transform.parent.GetComponentInChildren<CinemachineFreeLook>().LookAt = PlayerDataHolder.Instance.GetPlayerData(id).input.transform;
        data.GetPlayerInScene().transform.parent.GetComponentInChildren<CinemachineFreeLook>().Follow = PlayerDataHolder.Instance.GetPlayerData(id).input.transform;

        data.GetPlayerInScene().GetComponent<CharacterController>().center = Vector3.up * 300;
    }

    private int GetNextAvailableIndex(int currentlyAt, int iteration)
    {
        int index = (currentlyAt + 1) % playerCount; // Wrap around using modulo

        // Check if the index is busy
        if (IsSpectator(index))
        {
            iteration++;
            if(iteration > 20f) { return  -1; }
            index = GetNextAvailableIndex(index, iteration); // Be cautious of potential infinite recursion
        }

        return index;
    }

/*    private int GetNextAvailableIndex(int currentlyAt)
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
    }*/
/*    private int GetPreviousAvailableIndex(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt - 1;
        // Loop through list
        if (index <= playerCount)
        {
            index = playerCount;
        }

        // Check if the index is busy
        if (IsSpectator(index))
        {
            index = GetPreviousAvailableIndex(index);
        }
        // 
        return index;
    }*/
    private int GetPreviousAvailableIndex(int currentlyAt, int iteration)
    {
        int index = (currentlyAt - 1 + playerCount) % playerCount; // Wrap around

        // Check if the index is busy
        if (IsSpectator(index))
        {
            iteration++;
            if(iteration > 20f) { return  -1; }
            index = GetPreviousAvailableIndex(index, iteration); // Be cautious of potential infinite recursion
        }

        return index;
    }

    private void SwitchSpectatingPlayer(Vector2 value, PlayerInput input)
    {
        Debug.Log("Listener Called");
        PlayerData data = PlayerDataHolder.Instance.GetPlayerData(input);
        if (data == null)
        {
            Debug.LogError("The Device is not finding a player attached");
        }

        // Right Selection
        if (value.x > .5f)
        {
            int index = GetNextAvailableIndex(spectating[data.GetID()], 0);
            if (index == -1) { return; }
            spectating[data.GetID()] = index;

            int id = spectating[data.GetID()];

            Debug.Log(data.GetPlayerInScene().name);
            data.GetPlayerInScene().transform.parent.GetComponentInChildren<CinemachineFreeLook>().LookAt = PlayerDataHolder.Instance.GetPlayerData(id).input.transform;
            data.GetPlayerInScene().transform.parent.GetComponentInChildren<CinemachineFreeLook>().Follow = PlayerDataHolder.Instance.GetPlayerData(id).input.transform;
        }

        // Left Selection
        if(value.x < -.5f)
        {
            int index = GetPreviousAvailableIndex(spectating[data.GetID()], 0);
            if (index == -1) { return; }
            spectating[data.GetID()] = index;

            int id = spectating[data.GetID()];
            data.GetPlayerInScene().transform.parent.GetComponentInChildren<CinemachineFreeLook>().LookAt = PlayerDataHolder.Instance.GetPlayerData(id).input.transform;
            data.GetPlayerInScene().transform.parent.GetComponentInChildren<CinemachineFreeLook>().Follow = PlayerDataHolder.Instance.GetPlayerData(id).input.transform;
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
