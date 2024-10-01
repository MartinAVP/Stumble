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
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion localRotation = Quaternion.Euler(-rpm * Time.deltaTime, 0f, 0f);
        wheel1.transform.rotation = wheel1.transform.rotation * localRotation;
        Debug.Log("spin");


    }
}
