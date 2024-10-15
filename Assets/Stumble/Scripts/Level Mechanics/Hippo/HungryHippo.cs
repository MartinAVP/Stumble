using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungryHippo : MonoBehaviour
{
    private float timer;
    public float timeToRotate = 1f;

    public float hippoMouthSpeed = 10f;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Vector3 startingPos;

    public float hippoMoveSpeed = 2;

    public float openToLungeDelay = 1f;

    public float hippoLungeTime = 4f;

    public float lungeToCloseDelay = 1f;

    public float closeToRetreatDelay = 1f;

    public float retreatToOpenDelay = 1f;

    private GameObject hippoNeck;



    public GameObject playerKillzone;

    public bool triggered = false;
    private bool available = true;

    public bool reset = false;

    void Start()
    {
        startingPos = transform.position;
        closedRotation = transform.rotation;
        playerKillzone.SetActive(false);
        hippoNeck = GameObject.Find("HippoNeck");
        openRotation = Quaternion.Euler(-45f, transform.eulerAngles.y, transform.eulerAngles.z);
    }




    void FixedUpdate()
    {
        if (triggered && available)
        {

            StartCoroutine(HippoMotion());
        }

        if (reset)
        {
            reset = false;
            triggered = false;
            available = true;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            transform.position = startingPos;
        }
        

    }


    private IEnumerator HippoMotion()
    {
        available = false;

        float smoothing = 0;

        while (Quaternion.Angle(transform.rotation, openRotation) > 0.1f)
        {
            timer += Time.deltaTime;
            smoothing = Mathf.SmoothStep(0f, 1f, timer / timeToRotate);
            Debug.Log(smoothing);
            transform.rotation = Quaternion.Slerp(closedRotation, openRotation, smoothing);

        }
        timer = 0;
        smoothing = 0;
       
        Debug.Log("mouth opened");
        
        yield return new WaitForSecondsRealtime(openToLungeDelay);

        Debug.Log("lunge");

        while (timer < hippoLungeTime)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector3.forward * Time.deltaTime, Space.World);
        }
        timer = 0;

        Debug.Log("lunge complete");

        yield return new WaitForSecondsRealtime(lungeToCloseDelay);

        Debug.Log("mouth closing");

        while (timer < timeToRotate)
        {
            timer += Time.deltaTime;

            smoothing = Mathf.SmoothStep(0f, 1f, timer / timeToRotate);
            transform.rotation = Quaternion.Lerp(openRotation, closedRotation, smoothing);

        }
        timer = 0;
        smoothing = 0;

        playerKillzone.SetActive(true);
        Debug.Log("mouth closed");

        yield return new WaitForSecondsRealtime(openToLungeDelay);

        Debug.Log("retreat");

        while (timer < hippoLungeTime)
        {
            timer += Time.deltaTime;
            transform.Translate(-Vector3.forward * Time.deltaTime, Space.World);
        }
        timer = 0;

        Debug.Log("retreat complete");
        yield return new WaitForSecondsRealtime(retreatToOpenDelay);

        available = true;

    }





    //trigger - fixed up

    //open mouth - co ru

    //shoot forward (optional) - co ru

    //close - co ru

    //kill box activates - co ru?

    //retracts (optional) - co ru

    //kill box deactivates - co ru?

    //repeat - fixed up

}
