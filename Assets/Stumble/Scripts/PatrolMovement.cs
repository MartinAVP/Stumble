using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    // public int NodeCount = 4;
    private int CurrentNode = 0;
    public List<Transform> PatrolNodes = new List<Transform>();
    public float speed = 2f;
    public float rotationSpeed = 2f;

    private GameObject player;

    public bool faceTowardNextNode = false;

    private float delayTime = 1f;
    private float delayCounter = 0f;
    private bool delayed = false;

    private Quaternion nodeRotation;

    void FixedUpdate()
    {
        if (delayed == true)
        {
            delayCounter += Time.deltaTime;
            //Debug.Log("Delay Counter ms: " + delayCounter);

            if (delayCounter < delayTime)
            {
                return;
            }
            delayed = false;
        }
        Transform nextNode = PatrolNodes[CurrentNode];
        if (Vector3.Distance(transform.position,nextNode.position) < 0.01f)
        {
            transform.position = nextNode.position;
            //Debug.Log("next node: "  + nextNode);

            delayCounter = 0f;
            delayed = true;

            CurrentNode = (CurrentNode + 1) % PatrolNodes.Count;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, nextNode.position, speed * Time.deltaTime);
            if (faceTowardNextNode == true)
            {
                nodeRotation = Quaternion.LookRotation(nextNode.position - transform.position);
                float rotationStrength = Mathf.Min(rotationSpeed * Time.deltaTime, 1);
                transform.rotation = Quaternion.Lerp(transform.rotation, nodeRotation, rotationStrength);

                Debug.Log(player);

                if (player != null)
                {
                    //player.transform.rotation = Quaternion.identity;

                    //Debug.Log(nodeRotation);

                    // old "rotation" - cube will snap into direction
                    //transform.LookAt(nextNode.position);
                }
            }
        }

        //Debug.Log(player.transform.position);
    }

    /*
    //Player parrenting on collision with plat. 
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("connection");
            player = collision.gameObject;
            Debug.Log("Player Object should not be null: " + player.transform.rotation);

            collision.collider.transform.SetParent(transform);

        }
    } 

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("seperation");

            Debug.Log("Player Object: " + player);
            player = null;
            Debug.Log("Player Object null: " + player);
            collision.collider.transform.SetParent(null);
        }
    }
    */

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("connection");
            player = other.gameObject;
            //Debug.Log("Player Object should not be null: " + player.transform.rotation);

            other.transform.SetParent(transform); 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("separation");

            //Debug.Log("Player Object: " + player);
            player = null;
            // Debug.Log("Player Object null: " + player);

            other.transform.SetParent(null);
        }
    }

}
