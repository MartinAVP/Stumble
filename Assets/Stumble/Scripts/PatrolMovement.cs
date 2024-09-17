using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    // public int NodeCount = 4;
    private int CurrentNode = 0;
    public List<Transform> PatrolNodes = new List<Transform>();
    public float speed = 2f;

    public bool faceTowardNextNode = false;

    private float delayTime = 1f;
    private float delayCounter = 0f;
    private bool delayed = false;
    
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
                transform.LookAt(nextNode.position);
            }
        }
    }
}
