using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour, IBumper
{
    public float bounceForce;
    private BumpSource sourceType = BumpSource.StaticBumper;

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

        print(name + " bumper awake.\n" +
            "Is rigidbody bumper? " + isRigidbodyBumper.ToString());
    }

    public void Bump(Vector3 direction, float magnitude, IBumper source)
    {
        if(!isRigidbodyBumper) return;

        rb.AddForce(direction * magnitude, ForceMode.Impulse);
    }

    public BumpSource GetSourceType()
    {
        return sourceType;
    }

}
