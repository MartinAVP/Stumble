using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovingBase))]
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

            delayCounter = 0f;
            delayed = true;

            CurrentNode = (CurrentNode + 1) % PatrolNodes.Count;
        }
        else
        {
            //this is the toggle for the rotation, object will rotate to face the next node
            if (faceTowardNextNode == true)
            {
                nodeRotation = Quaternion.LookRotation(nextNode.position - transform.position);
                float rotationStrength = Mathf.Min(rotationSpeed * Time.fixedDeltaTime, 1);
                transform.rotation = Quaternion.Lerp(transform.rotation, nodeRotation, rotationStrength);
            }

            transform.position = Vector3.MoveTowards(transform.position, nextNode.position, speed * Time.deltaTime);
        }
    }
}
