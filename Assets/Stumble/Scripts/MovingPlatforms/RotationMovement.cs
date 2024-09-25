using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotationAxis
{
    Up,
    Right,
    Forward
}

[RequireComponent(typeof(MovingBase))]
public class RotationMovement : MovingPlatform
{
    [SerializeField] protected float rotationSpeed = 2f;
    [SerializeField] protected RotationAxis rotateAroundAxis;

    private Vector3 startAxis;
    
    protected void Start()
    {
        base.Start();

        if (rotateAroundAxis == RotationAxis.Up)
            startAxis = transform.up;
        else if (rotateAroundAxis == RotationAxis.Right)
            startAxis = transform.right;
        else if (rotateAroundAxis ==  RotationAxis.Forward)
            startAxis = transform.forward;

        MovingBase movingBase = GetComponent<MovingBase>();
        movingBase.owner = this;
        movingBase.PropagateToChildren();
    }

    protected void FixedUpdate()
    {
        base.FixedUpdate();

        transform.RotateAround(transform.position, startAxis, rotationSpeed * Time.deltaTime);
    }
}
