using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData
{
    public PlayerData(int id, GameObject playerInScene, PlayerInput input, InputDevice device)
    {
        this.id = id;
        this.playerInScene = playerInScene;
        this.input = input;
        this.device = device;
    }

    public int id;
    public GameObject playerInScene;
    public PlayerInput input;
    public InputDevice device;
}
