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
            planeBumper.Collision(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }

    protected enum collisionType
    {
        Sphere,
        Plane
    }
}
