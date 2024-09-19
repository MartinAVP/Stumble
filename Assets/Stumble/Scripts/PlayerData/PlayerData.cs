using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerData
{
    public PlayerData(int id, GameObject playerInScene, PlayerInput input, InputDevice device)
    {
        this.id = id;
        this.playerInScene = playerInScene;
        this.input = input;
        this.device = device;
    }

    public int id;                      // Id of the player [0,1,2,3]
    public GameObject playerInScene;    // The player in the scene (can be changed depending on the level)
    public PlayerInput input;           // Should be static for every scene once the player joins
    public InputDevice device;          // The Device the player is connected to

                                        // Cosmetics
}
