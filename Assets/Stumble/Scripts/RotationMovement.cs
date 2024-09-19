using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMovement : MonoBehaviour
{
    //public float speed = 2f;
    public float rotationSpeed = 2f;

    private GameObject player;
    
    //prevents overlap of parrent func if patroling script already active on gameObj
    private bool PatrolingPlatform = false;
    
    private Quaternion nodeRotation;

    //public bool Parrent = true;
    //private bool Child = false;

    //pu
    public GameObject BoxCollider;
   

    private void Awake()
    {
        if (gameObject.GetComponent("PatrolMovement") != null)
        {
            PatrolingPlatform = true;
        }
    }

    void FixedUpdate()
    {
        transform.Rotate( 0f, rotationSpeed * Time.deltaTime, 0f);
    }

    // this requires a trigger collider to function, place one and elevate it roughly 2.5-3 on the collider's y axis
    // will not be active if patrolling script is on same object

    void OnTriggerEnter(Collider other)
    {
        if (PatrolingPlatform == false)
        {
            if (other.CompareTag("Player"))
            {
                player = other.gameObject; //get player object
                other.transform.SetParent(transform); //sets platform as parrent
            }
        } 
    }

    void OnTriggerExit(Collider other)
    {
        if (PatrolingPlatform == false)
        {
            if (other.CompareTag("Player"))
            {
                player = null; //emptys out player object
                other.transform.SetParent(null); //unparrents the player from platform
            }
        }
    }
}
