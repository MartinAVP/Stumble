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
    [SerializeField] protected float rotationSpeed = 2f;
    [SerializeField] protected RotationAxis rotateAroundAxis;

    public override void Move()
    {
        Vector3 axis;

        if (rotateAroundAxis == RotationAxis.Up)
            axis = transform.up;
        else if (rotateAroundAxis == RotationAxis.Right)
            axis = transform.right;
        else if (rotateAroundAxis == RotationAxis.Forward)
            axis = transform.forward;

        transform.RotateAround(transform.position, transform.up, rotationSpeed * Time.deltaTime);
    }
}
