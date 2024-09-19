using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerDataManager : MonoBehaviour
{
    public List<PlayerData> players = new List<PlayerData>();
    [SerializeField] private List<LayerMask> playerLayers;
    private PlayerInputManager playerInputManager;
    public bool inLobby;

    public static event Action<PlayerData> onPlayerConnect;
    public static event Action<List<PlayerData>> onPlayerDisconnect;
    public static event Action onPlayerInputDeviceDisconnect;

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerLeft += RemovePlayer;
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
        playerInputManager.onPlayerLeft -= RemovePlayer;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    public void AddPlayer(PlayerInput input)
    {
        GameObject player = input.transform.parent.gameObject;
        PlayerData newList = new PlayerData(players.Count, player, input, input.devices[0]);
        players.Add(newList);

        // Need to use the paren due to the structure of the prefab
        Transform playerParent = input.transform.parent;
        //playerParent.position = spawnPoints[players.Count - 1].position;

        // Convert layer mask (bit) to an integer
        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        //set the layer
        //playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
        // add the layer
        //playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;

        // set the action in the custom cinemachine Input handler
        //playerParent.GetComponentInChildren<InputHandler>().horizontal = player.actions.FindAction("Look");

        //Check for player Count
        //Debug.Log(players.Count);
        //Player3ScreenToggle(players.Count);

    }
    public void RemovePlayer(PlayerInput input)
    {
        int playerID = findPlayer(input);
        if (playerID != -1)
        {
            Destroy(players[playerID].playerInScene);
            players.RemoveAt(playerID);
            Debug.Log("Remove player: " + playerID);
        }
    }
    public void RemovePlayer(InputDevice device)
    {
        int playerID = findPlayer(device);
        if (playerID != -1)
        {
            Destroy(players[playerID].playerInScene);
            players.RemoveAt(playerID);
            Debug.Log("Remove player: " + playerID);
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
                break;
            case InputDeviceChange.Reconnected:
                playerID = findPlayer(device);
                Debug.Log("Device Reconnected attached to player #" + playerID);
                break;
        }
    }

    // Find Player
    private int findPlayer(PlayerInput input)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].input == input)
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
            if (players[i].device == device)
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
    public PlayerData GetPlayerData(InputDevice device)
    {
        int playerID = findPlayer(device);
        if (playerID != -1)
        {
            return players[playerID];
        }
        return null;
    }
}
