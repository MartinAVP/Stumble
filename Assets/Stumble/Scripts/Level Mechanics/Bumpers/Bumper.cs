using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour, IBumper
{
    public float bounceForce;
    public float maxRigidbodySpeed = 50f;
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
        if (magnitude < 0.1f) return;

        Vector3 impulse = direction * magnitude;

        float velocityAlongImpulse = Vector3.Dot(direction, rb.velocity);
        rb.velocity = rb.velocity - direction  * velocityAlongImpulse;

        rb.AddForce(impulse, ForceMode.Impulse);
    }

    public void Bump(Vector3 direction, Vector3 position, float magnitude, BumpSource source)
    {
        if (!isRigidbodyBumper) return;
        if (magnitude < 0.1f) return;

        Vector3 impulse = direction * magnitude;

        float velocityAlongImpulse = Vector3.Dot(direction, rb.velocity);
        rb.velocity = rb.velocity - direction * velocityAlongImpulse;

        rb.AddForceAtPosition(impulse, position, ForceMode.Impulse);

        if(rb.velocity.magnitude > maxRigidbodySpeed)
        {
            rb.velocity = rb.velocity.normalized * maxRigidbodySpeed;
        }
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
