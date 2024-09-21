using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBumperComponent : MonoBehaviour
{
    private PlaneBumper planeBumper;
    [SerializeField] private collisionType type;

    private void Start()
    {
        planeBumper = this.transform.parent.GetComponent<PlaneBumper>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (type == collisionType.Plane)
        {
            planeBumper.isPlaneCollider = true;
            planeBumper.Collision(other);
        }
        else
        {
            planeBumper.isSphereCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (type == collisionType.Plane)
        {
            planeBumper.isPlaneCollider = false;
        }
        else
        {
            planeBumper.isSphereCollider = false;
        }
    }

    protected enum collisionType
    {
        Sphere,
        Plane
    }
}
