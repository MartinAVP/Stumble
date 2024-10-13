using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotationAxis
{
    Up,
    Right,
    Forward
}

public class RotationMovement : MovingPlatform
{
    [SerializeField] public float rotationSpeed = 2f;
    [SerializeField] public RotationAxis rotateAroundAxis;

    private bool isRotating = true;
    private int direction = 1;

    public override void Move()
    {
        if (isRotating)
        {
            Vector3 axis = transform.up;

            if (rotateAroundAxis == RotationAxis.Right)
                axis = transform.right;
            else if (rotateAroundAxis == RotationAxis.Forward)
                axis = transform.forward;

            transform.RotateAround(transform.position, axis * direction, rotationSpeed * Time.deltaTime);
        }
    }

    public void StopRotating()
    {
        isRotating = false;
    }

    public void StartRotating(bool reverseDirection)
    {
        isRotating=true;

        if(reverseDirection )
        {
            direction = direction * -1;
        }
    }
}
