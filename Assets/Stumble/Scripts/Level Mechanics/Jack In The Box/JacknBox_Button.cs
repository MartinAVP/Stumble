using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JacknBox_Button : MonoBehaviour
{
    public CapsuleCollider jackInTheBox;
    public GameObject Squish;

    private float jackInTheBoxControl;

    public float expandedRadius = 3f;
    public float expansionMultiplier = 7;
    public int loopInterations = 10;

    public float timeTillReset = 5f;


    private float resetTimer = 0;
    private bool triggered = false;

    void Start()
    {
        jackInTheBoxControl = jackInTheBox.radius;
        Debug.Log(jackInTheBoxControl);
    }

    void FixedUpdate()
    {
        if (triggered)
        {
            StartCoroutine(jackReset(triggered));
            triggered = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && triggered != true)
        {
            Debug.Log(jackInTheBox.radius);

            while (jackInTheBox.radius < expandedRadius )
            {
                jackInTheBox.radius += jackInTheBox.radius * expansionMultiplier * Time.deltaTime;
            }

            
            Debug.Log(jackInTheBox.radius);

            Debug.Log("radius complete");

            triggered = true;
        }


    }

    private IEnumerator jackReset(bool triggered)
    {
        Debug.Log(timeTillReset);
        yield return new WaitForSecondsRealtime(timeTillReset);
        Debug.Log("timer complete");
        jackInTheBox.radius = jackInTheBoxControl;

        Debug.Log("control " + jackInTheBoxControl);
        Debug.Log("modified rad: " + jackInTheBox.radius);
        
    }

}
