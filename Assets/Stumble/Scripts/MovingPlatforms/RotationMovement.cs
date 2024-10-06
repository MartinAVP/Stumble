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

    private Vector3 startAxis;
    
    private void Start()
    {
        base.Start();

        if (rotateAroundAxis == RotationAxis.Up)
            startAxis = transform.up;
        else if (rotateAroundAxis == RotationAxis.Right)
            startAxis = transform.right;
        else if (rotateAroundAxis ==  RotationAxis.Forward)
            startAxis = transform.forward;
    }

    public override void Move()
    {
        transform.RotateAround(transform.position, startAxis, rotationSpeed * Time.deltaTime);
    }
}
