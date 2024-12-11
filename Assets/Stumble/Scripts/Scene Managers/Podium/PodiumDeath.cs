using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumDeath : MonoBehaviour
{
    [SerializeField] private List<CheckpointSpawn> spawns;


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            /*            other.GetComponent<CharacterController>().enabled = false;
                        other.transform.position = respawnPos.transform.position;
                        if(other.GetComponent<ThirdPersonMovement>() != null)
                        {
                            other.GetComponent<ThirdPersonMovement>().horizontalVelocity = 0f;
                        }
                        //other.transform.rotation = respawnPos.transform.rotation;
                        other.GetComponent<CharacterController>().enabled = true;*/
            RespawnPlayer(other.gameObject);
        }
    }

    public void RespawnPlayer(GameObject other)
    {
        int random = Random.Range(0, spawns.Count);

        if (!spawns[random].spawnBlocked)
        {
            other.GetComponent<CharacterController>().enabled = false;
            other.transform.position = spawns[random].transform.position;

            if (other.GetComponent<ThirdPersonMovement>() != null)
            {
                other.GetComponent<ThirdPersonMovement>().horizontalVelocity = 0f;
            }
            //other.transform.rotation = respawnPos.transform.rotation;
            other.GetComponent<CharacterController>().enabled = true;
            return;
        }
        
        RespawnPlayer(other);
    }
}
