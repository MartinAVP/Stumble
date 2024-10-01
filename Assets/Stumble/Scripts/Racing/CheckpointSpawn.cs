using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSpawn : MonoBehaviour
{
    [HideInInspector]
    public bool spawnBlocked = false;
    public List<GameObject> playersConflicting;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            playersConflicting.Add(other.gameObject);
            CheckConflicts();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            playersConflicting.Remove(other.gameObject);
            CheckConflicts();
        }
    }

    private void CheckConflicts()
    {
        if (playersConflicting.Count == 0)
        {
            spawnBlocked = false;
        }
        else
        {
            spawnBlocked = true;
        }
    }
}
