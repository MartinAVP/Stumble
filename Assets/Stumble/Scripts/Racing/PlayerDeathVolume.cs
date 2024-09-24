using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            CheckpointManager.Instance.Respawn(other.gameObject);
        }
    }
}
