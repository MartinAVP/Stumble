using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PatrolMovement : MovingPlatform
{
    #region Events
    [Header("Events")]
    public UnityEvent startMoving = new UnityEvent();
    public UnityEvent stopMoving = new UnityEvent();
    #endregion

    // public int NodeCount = 4;
    private int CurrentNode = 0;
    public List<Transform> PatrolNodes = new List<Transform>();
    public float speed = 2f;
    public float rotationSpeed = 2f;

    public bool faceTowardNextNode = false;

    public float delayTime = 1f;
    private float delayCounter = 0f;
    private bool delayed = false;

    private Quaternion nodeRotation;

    public override void Move()
    {
        if (delayed == true)
        {
            delayCounter += Time.deltaTime;

            if (delayCounter < delayTime)
            {
                return;
            }
            delayed = false;

            startMoving?.Invoke();
        }
        Transform nextNode = PatrolNodes[CurrentNode];
        if (Vector3.Distance(transform.position,nextNode.position) < 0.01f)
        {
            transform.position = nextNode.position;

            delayCounter = 0f;
            delayed = true;

            CurrentNode = (CurrentNode + 1) % PatrolNodes.Count;

            stopMoving?.Invoke();
        }
        else
        {
            //this is the toggle for the rotation, object will rotate to face the next node
            if (faceTowardNextNode == true)
            {
                nodeRotation = Quaternion.LookRotation(nextNode.position - transform.position);
                float rotationStrength = Mathf.Min(rotationSpeed * Time.deltaTime, 1);
                transform.rotation = Quaternion.Lerp(transform.rotation, nodeRotation, rotationStrength);
            }

            
            transform.position = Vector3.MoveTowards(transform.position, nextNode.position, speed * Time.deltaTime);
        }
    }
}
