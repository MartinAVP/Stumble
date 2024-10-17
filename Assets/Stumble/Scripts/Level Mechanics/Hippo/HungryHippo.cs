using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungryHippo : MonoBehaviour
{
    //timer for movement smoothness - delay is a lockout to prevent repeat calling
    private float timer;
    private bool delayed = false;

    public int mouthOpenAngle = 45;
    public float movementMultiplier = 0.5f;

    public float distance;

    public float actionDuration = 2f;
    public int frameSkips = 5;
    


    //standard delay inbetween each action
    public float inbetweenActionDelay = .5f;

    //range for random delay after the full set of actions
    public float minDelay = .1f;
    public float maxDelay = 1f;
    private float inActionDelay = .000000000000000000000000000000000000000000000001f;

    
    

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Vector3 startingPos;
    private Vector3 endingPos;

    private float previousMouthAngle;
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
        openRotation = Quaternion.Euler(-mouthOpenAngle, transform.eulerAngles.y, transform.eulerAngles.z);

        previousMouthAngle = mouthOpenAngle;
    }




    void FixedUpdate()
    {

        if (triggered && available)
        {
            
            StartCoroutine(HippoMotion());
                       
        }

        
        if (mouthOpenAngle != previousMouthAngle)
        {
            openRotation = Quaternion.Euler(-mouthOpenAngle, 0f, 0f);
            previousMouthAngle = mouthOpenAngle;
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
        float smoothing;
        int temp = 0;

        endingPos.z = startingPos.z + distance;

        while (Quaternion.Angle(transform.rotation, openRotation) > 0.1f)
        {
            timer += Time.deltaTime;
            smoothing = Mathf.SmoothStep(0f, 1f, timer / actionDuration);
            
            transform.rotation = Quaternion.Slerp(closedRotation, openRotation, smoothing);
            temp++;
            if (temp > frameSkips)
            {
                yield return new WaitForSeconds(inActionDelay);
                temp = 0;
            }

        }
        timer = 0;
        smoothing = 0;


        //Debug.Log("mouth opened");
        
        yield return new WaitForSecondsRealtime(inbetweenActionDelay);

        //Debug.Log("lunge");

        while (transform.position.z < endingPos.z)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector3.forward * movementMultiplier * Time.deltaTime, Space.World);
            temp++;
            if (temp > frameSkips)
            {
                yield return new WaitForSeconds(inActionDelay);
                temp = 0;
            }
        }
        timer = 0;

        //Debug.Log("lunge complete");

        yield return new WaitForSecondsRealtime(inbetweenActionDelay);

        //Debug.Log("mouth closing");

        while (Quaternion.Angle(transform.rotation, closedRotation) > 0.1f)
        {
            timer += Time.deltaTime;
            smoothing = Mathf.SmoothStep(0f, 1f, timer / actionDuration);

            transform.rotation = Quaternion.Slerp(openRotation, closedRotation, smoothing);
            temp++;
            if (temp > frameSkips)
            {
                yield return new WaitForSeconds(inActionDelay);
                temp = 0;
            }

        }
        timer = 0;
        smoothing = 0;

        playerKillzone.SetActive(true);
        
        //Debug.Log("mouth closed");

        yield return new WaitForSecondsRealtime(inbetweenActionDelay);
        playerKillzone.SetActive(false);

        //Debug.Log("retreat");

        while (transform.position.z > startingPos.z)    
        {
            timer += Time.deltaTime;
            transform.Translate(-Vector3.forward * movementMultiplier * Time.deltaTime, Space.World);
            temp++;
            if (temp > frameSkips)
            {
                yield return new WaitForSeconds(inActionDelay);
                temp = 0;
            }
        }
        timer = 0;

        //Debug.Log("retreat complete");
        yield return new WaitForSecondsRealtime(Random.Range(minDelay, maxDelay));

        available = true;

    }

    /*
    private IEnumerator WaitDelay(bool loopDelay)
    {
        delayed = true;
        if (loopDelay)
        {
            randomDelay = Random.Range(minDelay, maxDelay);

            yield return new WaitForSecondsRealtime(randomDelay);
        }
        if (!loopDelay)
        {

            yield return new WaitForSecondsRealtime(standardDelay);
        }
        delayed = false;
    } 
    */




    //trigger - fixed up

    //open mouth - co ru

    //shoot forward (optional) - co ru

    //close - co ru

    //kill box activates - co ru?

    //retracts (optional) - co ru

    //kill box deactivates - co ru?

    //repeat - fixed up

}
