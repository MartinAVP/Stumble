using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaneBumperComponent : MonoBehaviour
{
    [SerializeField] private PlaneBumper planeBumper;
    [SerializeField] private collisionType type;

    public string soundFXId;
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
