using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerData
{
    public PlayerData(int id, GameObject playerInScene, PlayerInput input, InputDevice device, bool isHost, CosmeticData cosmeticData)
    {
        this.id = id;
        this.playerInScene = playerInScene;
        this.input = input;
        this.device = device;
        this.isHost = isHost;
        this.cosmeticData = cosmeticData;
    }

    // Main Data
    public int id { get; set; }                         // Id of the player [0,1,2,3]
    public GameObject playerInScene { get; set; }       // The player in the scene (can be changed depending on the level)
    public PlayerInput input { get; set; }              // Should be static for every scene once the player joins
    public InputDevice device { get; set; }             // The Device the player is connected to
    public bool isHost { get; set; }

    // Add Cosmetics Here
    public CosmeticData cosmeticData { get; set; }

    // Main
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

    public CosmeticData GetCosmeticData()
    {
        return this.cosmeticData;
    }
    #endregion

    // Cosmetics
/*    public void SetColor(Material color)
    {
        this.color = color;
    }

    public Material GetColor(Material color) {
        return this.color;
    }*/
}
