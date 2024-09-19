using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerData
{
    public PlayerData(int id, GameObject playerInScene, PlayerInput input, InputDevice device, bool isHost)
    {
        this.id = id;
        this.playerInScene = playerInScene;
        this.input = input;
        this.device = device;
        this.isHost = isHost;
    }

    public int id;                      // Id of the player [0,1,2,3]
    public GameObject playerInScene;    // The player in the scene (can be changed depending on the level)
    public PlayerInput input;           // Should be static for every scene once the player joins
    public InputDevice device;          // The Device the player is connected to
    public bool isHost;
    // Add Cosmetics Here

    #region Setters
    public void SetID(int id)
    {
        this.id = id;
    }
    public void SetPlayerInScene(GameObject player)
    {
        this.playerInScene = player;
    }
    public void SetPlayerInput(PlayerInput input)
    {
        this.input = input;
    }
    public void SetPlayerInputDevice(InputDevice inputDevice)
    {
        this.device = inputDevice;
    }
    public void SetHost(bool isHost)
    {
        this.isHost = isHost;
    }
    #endregion

    #region Getters
    public int GetID()
    {
        return this.id;
    }

    public GameObject GetPlayerInScene()
    {
        return this.playerInScene;
    }

    public PlayerInput GetInput() 
    {
        return this.input;
    }

    public InputDevice GetDevice()
    {
        return this.device;
    }

    public bool CheckIsHost()
    {
        return this.isHost;
    }
    #endregion
}
