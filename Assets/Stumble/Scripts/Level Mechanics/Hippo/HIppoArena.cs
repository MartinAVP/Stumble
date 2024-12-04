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

    private float arenaShrinkRatio;
    private Vector3 origionalScale;
    private float origionalBumperY;

    public GameObject Bumper1;
    public GameObject Bumper2;
    public GameObject Bumper3;
    public GameObject Bumper4;

    // Start is called before the first frame update
    void Start()
    {
        arenaY = transform.localScale.y;
        if (Bumper1 != null)
        {
            origionalBumperY = Bumper1.transform.localScale.y;
            arenaShrinkRatio = origionalBumperY / origionalScale.y;
        }
        origionalScale = transform.localScale;
        dumpScale.x = 0.045f;
        dumpScale.z = 0.113f;

        StartCoroutine(DummyTimer());  
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (timerComplete && available && transform.localScale.x > EndingScale && transform.localScale.z > EndingScale)
        {
            StartCoroutine(ArenaShrink());
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
        dumpScale.y = 1f * arenaShrinkRatio * arenaShrinkRatio;
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
