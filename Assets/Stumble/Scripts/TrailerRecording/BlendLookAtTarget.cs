using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BlendLookAtTarget : MonoBehaviour
{
    public LookAtConstraint lookAtConstraint;
    public CinemachineDollyCart dollyCart;
    public float[] distances;

    public void Update()
    {
        for (int i = 0; i < lookAtConstraint.sourceCount; i++)
        {
            ConstraintSource source = lookAtConstraint.GetSource(i);
            source.weight = distances[i] * dollyCart.m_Position;
            lookAtConstraint.SetSource(i, source);
        }
    }
}
