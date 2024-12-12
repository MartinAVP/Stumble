using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraTag : MonoBehaviour
{
    public int cameraNum = 0;
    public GameObject player;

    private void Start()
    {
        if(player.layer == LayerMask.NameToLayer("Player1"))
            gameObject.tag = "Camera1-"+cameraNum;
        if (player.layer == LayerMask.NameToLayer("Player2"))
            gameObject.tag = "Camera2-" + cameraNum;
        if (player.layer == LayerMask.NameToLayer("Player3"))
            gameObject.tag = "Camera3-" + cameraNum;
        if (player.layer == LayerMask.NameToLayer("Player4"))
            gameObject.tag = "Camera4-" + cameraNum;
    }
}
