using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour, IBumper
{
    public float bounceForce;
    protected BumpSource sourceType = BumpSource.StaticBumper;

    private Rigidbody rb;
    private bool isRigidbodyBumper = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            isRigidbodyBumper = true;
            sourceType = BumpSource.RigidbodyBumper;
        }
    }

    public void Bump(Vector3 direction, float magnitude, BumpSource source)
    {
        if(!isRigidbodyBumper) return;
        if (magnitude < 0f) return;

        Vector3 impulse = direction * magnitude;

        if (direction.y * rb.velocity.y < 0)
        {
            Vector3 newVelocity = rb.velocity;
            newVelocity.y = 0;
            rb.velocity = newVelocity;
        }

        rb.AddForce(impulse, ForceMode.Impulse);
    }

    public void Bump(Vector3 direction, Vector3 position, float magnitude, BumpSource source)
    {
        if (!isRigidbodyBumper) return;
        if (magnitude < 0f) return;

        Vector3 impulse = direction * magnitude;

        if(direction.y * rb.velocity.y < 0)
        {
            Vector3 newVelocity = rb.velocity;
            newVelocity.y = 0;
            rb.velocity = newVelocity;
        }

        rb.AddForceAtPosition(impulse, position, ForceMode.Impulse);
    }

    public float GetBumpMagnitude()
    {
        return bounceForce;
    }

    public virtual Vector3 GetBumpDirection(GameObject other)
    {
        return Vector3.zero;
    }

    public BumpSource GetBumpSource()
    {
        return sourceType;
    }
}
