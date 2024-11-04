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
            planeBumper.Collision(other.gameObject);
        }
        else
        {
            planeBumper.AddSphereOverlap(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(type == collisionType.Sphere)
        {
            planeBumper.RemoveSphereOverlaps(other.gameObject);
        }
    }

    protected enum collisionType
    {
        Sphere,
        Plane
    }
}
