using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInstance : MonoBehaviour
{
    public GameObject wheel1;
    public GameObject wheel2;
    public GameObject wheel3;
    public GameObject wheel4;

    public int rpm = 60;
    public 


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(wheel1.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion localRotation = Quaternion.Euler(-rpm * Time.deltaTime, 0f, 0f);
        wheel1.transform.localRotation = Quaternion.Euler(-rpm * Time.deltaTime, 0f, 0f);
        
        //wheel3.transform.rotation = wheel1.transform.rotation * localRotation;
        //wheel4.transform.rotation = wheel1.transform.rotation * localRotation;
        Debug.Log("spin");

        CarPositioning();
    }

    private void CarPositioning()
    {
        Vector3 FrontLeft = wheel1.transform.position; // - Vector3.forward * 1.2f + Vector3.right * 0.9f;
        Vector3 FrontRight = wheel2.transform.position; // - Vector3.forward * 1;
        Vector3 BackLeft = transform.position + Vector3.right * 1;
        Vector3 BackRight = transform.position - Vector3.right * 1;

        Vector3 delta = Vector3.down * ((0.5f * 1) + .2f);

        Debug.DrawLine(FrontLeft, FrontLeft + delta, Color.red);
        Debug.DrawLine(FrontRight, FrontRight + delta, Color.blue);
        Debug.DrawLine(BackLeft, BackLeft + delta, Color.green);
        Debug.DrawLine(BackRight, BackRight + delta, Color.magenta);
    }
}
