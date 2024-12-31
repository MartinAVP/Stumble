using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetCameraTag : MonoBehaviour
{
    public int cameraNum = 0;
    public PlayerInput playerInput;

    private void Start()
    {
        int index = -1;
        if(playerInput != null)
        {
            PlayerDataHolder playerDataHolder = PlayerDataHolder.Instance;

            index = playerDataHolder.GetPlayerData(playerInput).id + 1;
        }
        else
        {
            Debug.LogError("Cannot set camera " + gameObject.name + " name correctly. Invalid player input.");
        }

        gameObject.tag = "Camera"+index+"-"+cameraNum;
    }

}
