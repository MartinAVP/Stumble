using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class HungryHippo : MonoBehaviour
{
    //timer for movement smoothness - delay is a lockout to prevent repeat calling
    private float timer;
    private bool delayed = false;


    [Header("Hippo Adjusments")]
    public int mouthOpenAngle = 45;
    public float movementMultiplier = 0.5f;
    public float speed;
    public float actionDuration = 2f;
    public int frameSkips = 5;
    public float inbetweenActionDelay = .5f;

    public float ChompOverTime = 1.75f;
    public int MaxRounds = 8;


    [Header("Player Tracking")]
    public float TimeBetweenPlayerTrackingUpdates = 2;
    public bool PlayerTracking = false;
    private Vector3 TrackedPlayerPos;
    private float ShortestDistance = 100;

    private int rounds = 0;


    public GameObject HippoParrent;
    private bool Collided = false;

    //standard delay inbetween each action


    //range for random delay after the full set of actions
    [Header("Random Delay For Start Of New Cycle")]
    public float minDelay = .1f;
    public float maxDelay = 1f;
    private float inActionDelay = .000000000000000000000000000000000000000000000001f;


    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Vector3 startingPos;
    public GameObject endingPos;

    private float previousMouthAngle;
    private Quaternion startingRotation;

    public GameObject playerKillzone;
    public GameObject targetPoint;

    [Header("Testing Bools")]
    public bool triggered = false;
    private bool available = true;
    public bool reset = false;

    void Start()
    {
        startingPos = transform.position;
        closedRotation = transform.rotation;
        playerKillzone.SetActive(false);
        openRotation = Quaternion.Euler(-mouthOpenAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        startingRotation = transform.rotation;
        Debug.Log(startingRotation);

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
            transform.rotation = startingRotation;
            transform.position = startingPos;
        }
    }

    private IEnumerator HippoMotion()
    {
        available = false;
        float smoothing;
        int temp = 0;

        playerKillzone.SetActive(false);
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

        if (PlayerTracking)
        {
            if (PlayerDataHolder.Instance != null)
            {
                ShortestDistance = float.MaxValue;

                for (int i = 0; i < PlayerDataHolder.Instance.GetPlayers().Count; i++)
                {
                    var d = Vector3.Distance(transform.position, PlayerDataHolder.Instance.GetPlayerData(i).input.gameObject.transform.position);
                    if (d < ShortestDistance)
                    {
                        ShortestDistance = d;
                        TrackedPlayerPos = PlayerDataHolder.Instance.GetPlayerData(i).input.gameObject.transform.position;
                    }
                }
                Vector3 directionToPlayer = (TrackedPlayerPos - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                Vector3 eulerRotation = targetRotation.eulerAngles;
                eulerRotation.x = HippoParrent.transform.rotation.eulerAngles.x;
                targetRotation = Quaternion.Euler(eulerRotation);

                HippoParrent.transform.rotation = targetRotation;

            }

        }

        while (Vector3.Distance(transform.position, targetPoint.transform.position) > 0.1f)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.transform.position, speed * Time.deltaTime);
            Vector3 directionToTarget = targetPoint.transform.position - transform.position;
            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }

            temp++;
            if (temp > frameSkips)
            {
                yield return new WaitForSeconds(inActionDelay);
                temp = 0;
            }
        }

        transform.position = targetPoint.transform.position;
        timer = 0;

        //Debug.Log("lunge complete");

        yield return new WaitForSecondsRealtime(inbetweenActionDelay);

        //Debug.Log("mouth closing");

        while (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.x, closedRotation.eulerAngles.x)) > 0.1f)
        {
            timer += Time.deltaTime;
            smoothing = Mathf.SmoothStep(0f, 1f, timer / actionDuration);
            float interpolatedX = Mathf.LerpAngle(transform.rotation.eulerAngles.x, 0, smoothing);
            Quaternion newRotation = Quaternion.Euler(interpolatedX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.rotation = newRotation;

            temp++;
            if (temp > frameSkips + 1)
            {
                yield return new WaitForSeconds(inActionDelay + .01f);
                temp = 0;
            }
        }

        timer = 0;
        smoothing = 0;

        playerKillzone.SetActive(true);

        //Debug.Log("mouth closed");

        yield return new WaitForSecondsRealtime(inbetweenActionDelay);


        //Debug.Log("retreat");

        while (Vector3.Distance(transform.position, startingPos) > .1f)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, startingPos, speed * Time.deltaTime);
            //this.transform.Translate(-Vector3.forward * movementMultiplier * Time.deltaTime, Space.World);
            temp++;
            if (temp > frameSkips)
            {
                yield return new WaitForSeconds(inActionDelay);
                temp = 0;
            }
        }

        transform.position = startingPos;
        timer = 0;

        //Debug.Log("retreat complete");
        yield return new WaitForSecondsRealtime(Random.Range(minDelay, maxDelay));

        available = true;

        rounds++;
        if (rounds < MaxRounds)
        {
            speed += ChompOverTime;
            if (rounds == 4 || rounds == 8)
            {
                frameSkips -= 1;
            }
        }

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