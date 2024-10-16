using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            print("kill player ");

            // Is in Racing Gamemode
            if(FindAnyObjectByType(typeof(CheckpointManager)) != null)
            {
                CheckpointManager.Instance.Respawn(other.gameObject);
            }
            // Player is in Arena Mode
            if (FindAnyObjectByType(typeof(ArenamodeManager)) != null) {
                ArenamodeManager.Instance.PlayerOnKillZone(other.gameObject);
                // Kill the player
            }
        }
    }
}

