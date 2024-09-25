using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 changeInPosition = Vector3.zero;
    public Vector3 changeInRotation = Vector3.zero;

    private Vector3 previousPosition = Vector3.zero;
    private Quaternion previousRotation;

    public Vector3 ChangeInPosition 
    { 
        get { return transform.position - previousPosition; } 
    }
    public Vector3 ChangeInRotation { get { return (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles; } }

    protected void Start()
    {
        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }

    protected void FixedUpdate()
    {
        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }
}
