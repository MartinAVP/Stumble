using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformData : MonoBehaviour
{
    [SerializeField] public MovingPlatform parent;

    private Vector3 previousPosition;
    private Vector3 linearVelocity;
    private Quaternion previousRotation;

    private void Update()
    {
        UpdatePreviousPosition();
        UpdatePreviousRotation();
    }

    private void LateUpdate()
    {
        linearVelocity = (transform.position - previousPosition) / Time.deltaTime;
    }

    public void UpdatePreviousPosition()
    {
        previousPosition = transform.position;
    }

    public void UpdatePreviousRotation()
    {
        previousRotation = transform.rotation;
    }

    public Vector3 ChangeInPosition
    {
        get { return transform.position - previousPosition; }
    }

    public Vector3 ChangeInRotation
    {
        get { return (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles; }
    }

    public Vector3 LinearVelocity
    {
        get { return linearVelocity; }
    }
}
