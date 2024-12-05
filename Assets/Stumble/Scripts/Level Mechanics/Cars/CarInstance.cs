using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInstance : MonoBehaviour
{
    [Header("Wheel Storage")] 
    public GameObject wheel1;
    public GameObject wheel2;
    public GameObject wheel3;
    public GameObject wheel4;

    [Header("Time Until It Despawns")]
    public float despawnTime = 5f;

    [Header("Movement Speed / Wheel Rotation")]
    public float speed = 3;

    [Header("Max Rotation the Car Can Achieve Downward")]
    public float MaxRotation = -70f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DespawnAfterTime());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion localRotation = Quaternion.Euler(-speed * 35 * Time.deltaTime, 0f, 0f);
        wheel1.transform.rotation = wheel1.transform.rotation * localRotation;
        wheel2.transform.rotation = wheel2.transform.rotation * localRotation;
        wheel3.transform.rotation = wheel3.transform.rotation * localRotation;
        wheel4.transform.rotation = wheel4.transform.rotation * localRotation;
        Debug.Log("spin");

        //transform.position ;
        transform.position -= transform.forward * Time.deltaTime * speed;

        ClampParentRotation();

        CarPositioning();
    }

    private void CarPositioning()
    {
        Vector3 FrontLeft = wheel1.transform.position; // - Vector3.forward * 1.2f + Vector3.right * 0.9f;
        Vector3 FrontRight = wheel2.transform.position; // - Vector3.forward * 1;
        Vector3 BackLeft = wheel3.transform.position;
        Vector3 BackRight = wheel4.transform.position;

        Vector3 delta = Vector3.down * ((0.5f * 1) + .2f);

        Debug.DrawLine(FrontLeft, FrontLeft + delta, Color.red);
        Debug.DrawLine(FrontRight, FrontRight + delta, Color.blue);
        Debug.DrawLine(BackLeft, BackLeft + delta, Color.green);
        Debug.DrawLine(BackRight, BackRight + delta, Color.magenta);
    }

    private void ClampParentRotation()
    {
        Vector3 localRotation = transform.localEulerAngles;

        // Convert X angle to a range of -180 to 180 for clamping
        if (localRotation.x > 180)
        {
            localRotation.x -= 360;
        }

        // Clamp rotations
        localRotation.x = Mathf.Clamp(localRotation.x, MaxRotation, 0f);
        localRotation.y = 0f;
        localRotation.z = 0f;

        // Apply clamped rotation
        transform.localEulerAngles = localRotation;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "CullZone")
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSeconds(despawnTime); // Wait for the specified time
        Destroy(gameObject); // Destroy the car
    }
}
