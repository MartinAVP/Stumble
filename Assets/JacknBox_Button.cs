using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JacknBox_Button : MonoBehaviour
{
    public CapsuleCollider jackInTheBox;
    private CapsuleCollider jackInTheBoxControl;

    public float expandedRadius = 4f;
    public float expansionMultiplier = 7;
    public int loopInterations = 10;

    public float timeToReset = 4;


    private float resetTimer = 0;
    private bool triggered = false;

    void Start()
    {
        jackInTheBoxControl = jackInTheBox;   
    }

    void FixedUpdate()
    {
        if (triggered)
        {
            resetTimer += Time.deltaTime;
            if (resetTimer >= timeToReset)
            {
                jackInTheBox.radius = jackInTheBoxControl.radius; 
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(jackInTheBox.radius);

            while (jackInTheBox.radius < expandedRadius )
            {
                jackInTheBox.radius += jackInTheBox.radius * expansionMultiplier * Time.deltaTime;
            }
                            

            Debug.Log(jackInTheBox.radius);

            Debug.Log("radius complete");
        }


    }

}
