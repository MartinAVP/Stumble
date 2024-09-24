using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBase : MonoBehaviour
{
    private Vector3 changeInPosition = Vector3.zero;
    private Vector3 changeInRotation = Vector3.zero;

    private Vector3 previousPosition = Vector3.zero;
    private Quaternion previousRotation;

    public Vector3 ChangeInPosition { get { return changeInPosition; } }
    public Vector3 ChangeInRotation { get { return changeInRotation; } }

    private void Start()
    {
        changeInPosition = transform.position - previousPosition;
        changeInRotation = Vector3.zero;

        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        changeInPosition = transform.position - previousPosition;
        changeInRotation = (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles;

        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }
}
