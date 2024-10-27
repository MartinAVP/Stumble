using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            // Which player triggered it?
            PlayerData data = PlayerDataHolder.Instance.GetPlayerData(other.GetComponent<PlayerInput>());
            CheckpointManager.Instance.ReachCheckpoint(data, this.gameObject);
        }
    }
}
