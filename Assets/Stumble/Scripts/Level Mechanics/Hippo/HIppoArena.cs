using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HippoArena : MonoBehaviour
{

    [Header("Arena Values")]
    [Tooltip("Adding more numbers to the decimal will change the shrinking speed as well")]
    public float ArenaShrinkPercent = .999f;
    [Tooltip("The amount runs of the loop before a quick pause, helps with making the 'animation' smoother")]
    public float frameSkips = 2;
    private float inActionDelay = .00000000001f;

    [Tooltip("The Delay Will Be In Secounds")]
    public int DelayUntilShrink = 600;

    [Tooltip("Ending Scale of the Hippo_Board Parrent gameObject")]
    public float EndingScale = 4;
    private float temp = 0;
    private bool available = true;
    private bool timerComplete = false;

    private int counter = 0;
    private Vector3 currentScale;
    private Vector3 dumpScale;
    private float arenaY;

    private Vector3 origionalScale;
    private float origionalBumperX;

    public GameObject Bumper1;
    public GameObject Bumper2;
    public GameObject Bumper3;
    public GameObject Bumper4;

    public int BumperScaleFrames = 5;
    private int dumpCounter;

    // Start is called before the first frame update
    void Start()
    {
        arenaY = transform.localScale.y;
        origionalBumperX = Bumper1.transform.localScale.x;
        origionalScale = transform.localScale;
        dumpScale.y = Bumper1.transform.localScale.y;
        dumpScale.z = Bumper1.transform.localScale.z;
        dumpCounter = BumperScaleFrames;

        StartCoroutine(DummyTimer());  
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (timerComplete && available && transform.localScale.x > EndingScale && transform.localScale.z > EndingScale)
        {
            StartCoroutine(ArenaShrink());
        }
        else
        {

        }

    }

    private IEnumerator DummyTimer()
    {
        yield return new WaitForSeconds(DelayUntilShrink);
        timerComplete = true;
    }

    private IEnumerator ArenaShrink()
    {
        available = false;
        currentScale.x = ArenaShrinkPercent * transform.localScale.x;
        currentScale.z = ArenaShrinkPercent * transform.localScale.z;

        
        if (Bumper1.transform.localScale.x > ( EndingScale / origionalScale.x) * origionalBumperX)
        {
            dumpScale.x = Bumper1.transform.localScale.x * .9995f;
            dumpScale.y = Bumper1.transform.localScale.y * 1.0005f;
            dumpScale.z = Bumper1.transform.localScale.z * 1.0005f;
        }

        dumpCounter++;

        Bumper1.transform.localScale = dumpScale;
        Bumper2.transform.localScale = dumpScale;
        Bumper3.transform.localScale = dumpScale;
        Bumper4.transform.localScale = dumpScale;


        currentScale.y = arenaY;




        transform.localScale = currentScale;

        counter++;
        if (counter > frameSkips)
        {
            yield return new WaitForSeconds(inActionDelay);
            counter = 0;
        }
        available = true;
    }
}
