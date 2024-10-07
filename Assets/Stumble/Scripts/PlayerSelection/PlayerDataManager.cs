using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    public List<PlayerData> players = new List<PlayerData>();

    private PlayerInputManager playerInputManager;

    // Subscribable Event
    public event Action<PlayerData> onPlayerConnect;
    public event Action<PlayerInput> onPlayerAdded;
    public event Action<PlayerData> onPlayerInputDeviceDisconnect;
    public event Action<PlayerData> onPlayerInputDeviceReconnect;

    // Scene Variables
    // Eliminate this variable ASAP
    public bool isLobby;

    public static PlayerDataManager Instance { get; private set; }

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
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneSwitch;
    }

    private void OnEnable()
    {
        /*        playerInputManager = FindObjectOfType<PlayerInputManager>();*/
        // Subscribe to the sceneLoaded event
        playerInputManager = FindObjectOfType<PlayerInputManager>();

        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerLeft += RemovePlayer;
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneSwitch;

        playerInputManager.onPlayerJoined -= AddPlayer;
        playerInputManager.onPlayerLeft -= RemovePlayer;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
        playerInputManager.onPlayerLeft -= RemovePlayer;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnSceneSwitch(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Switched");
        playerInputManager = FindObjectOfType<PlayerInputManager>();

        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerLeft += RemovePlayer;
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    public void AddPlayer(PlayerInput input)
    {
        GameObject player = input.transform.parent.gameObject;

        PlayerData tempPlayerData;
        if(input.playerIndex == 0)
        {
            tempPlayerData = new PlayerData(input.playerIndex, player, input, input.devices[0], true, new CosmeticData());
        }
        else
        {
            tempPlayerData = new PlayerData(input.playerIndex, player, input, input.devices[0], false, new CosmeticData());
        }

        //Debug.Log("Added Player " + input.playerIndex + " to data manager");
        players.Add(tempPlayerData);
        onPlayerAdded?.Invoke(tempPlayerData.GetInput());
        onPlayerConnect?.Invoke(tempPlayerData);
    }
    public void RemovePlayer(PlayerInput input)
    {
        GameState state = GameController.Instance.gameState;
        if(state != GameState.Lobby)
        {
            return;
        }
        //Debug.Log("PlayerDataManager isLobby" + isLobby.ToString());
/*        if (!isLobby) { return; }*/
        int playerID = findPlayer(input);
/*        List<PlayerData> tempPlayers = new List<PlayerData>();
        tempPlayers = players;
        tempPlayers.RemoveAt(playerID);*/

        if (playerID != -1)
        {
            //Destroy(players[playerID].GetPlayerInScene());
            //players.RemoveAt(playerID);
            players.Remove(players[playerID]);
            Debug.Log("Remove player: " + playerID + " player remaining: " + players.Count);
        }

        // Redefine Ids and Set Host to Player 0
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetID(i);
            players[i].SetHost(false);
        }

        if(playerInputManager.playerCount > 0)
        {
            players[0].SetHost(true);
        }

    }
    public void RemovePlayer(InputDevice device)
    {
        int playerID = findPlayer(device);
        if (playerID != -1)
        {
            Destroy(players[playerID].GetPlayerInScene());
            players.RemoveAt(playerID);
            Debug.Log("Remove player: " + playerID);
        }

        // Set the IDS again
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetID(i);
        }
    }

    // Device Reconnection System
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        int playerID = 0;
        switch (change)
        {
            case InputDeviceChange.Removed:
                Debug.Log($"Device removed: {device}");
                playerID = findPlayer(device);
                Debug.Log("Device Disconnected belonged to player #" + playerID);
                onPlayerInputDeviceDisconnect?.Invoke(players[findPlayer(device)]);
                break;
            case InputDeviceChange.Reconnected:
                playerID = findPlayer(device);
                Debug.Log("Device Reconnected attached to player #" + playerID);
                onPlayerInputDeviceReconnect?.Invoke(players[findPlayer(device)]);
                break;
        }
    }

    // Find Player
    private int findPlayer(PlayerInput input)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetInput() == input)
            {
                return i;
            }
        }
        return -1;
    }
    private int findPlayer(InputDevice device)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetDevice() == device)
            {
                return i;
            }
        }
        return -1;
    }

    // Get Player Data
    public PlayerData GetPlayerData(PlayerInput input)
    {
        int playerID = findPlayer(input);
        if (playerID != -1)
        {
            return players[playerID];
        }
        return null;
    }
    public PlayerData GetPlayerData(int id)
    {
        //Debug.Log("Current Players PlayerDataMAnager: " + players.Count);
        if (id <= players.Count)
        {
            return players[id];
        }
        return null;
    }
    public PlayerData GetPlayerData(InputDevice device)
    {
        int playerID = findPlayer(device);
        if (playerID != -1)
        {
            return players[playerID];
        }
        return null;
    }

    // Get All Players
    public List<PlayerData> GetPlayers()
    {
        return players;
    }
    public void ClearPlayers()
    {
        players.Clear();
    }

    public int GetPlayersWithInGameCharacter()
    {
        int count = 0;
        foreach (PlayerData player in players)
        {
            if(player.GetPlayerInScene() != null)
            {
                count++;
            }
        }

        return count;
    }
}
