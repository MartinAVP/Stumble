using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HippoArena : MonoBehaviour
{

    [Header("Arena Values")]
    [Tooltip("Adding more numbers to the decimal will change the shrinking speed as well")]
    public float ArenaShrinkPercent = 1f;
    [Tooltip("The amount runs of the loop before a quick pause, helps with making the 'animation' smoother")]
    public float frameSkips = 1;
    private float inActionDelay = .00000001f;

    [Tooltip("The Delay Will Be In Secounds")]
    public int DelayUntilShrink = 600;

    [Tooltip("Ending Scale of the Hippo_Board Parrent gameObject")]
    public float EndingScale = 4;
    private float temp = 0;
    private bool available = true;
    private bool timerComplete = false;

    private int counter = 0;
    private Vector3 currentScale;
    private float arenaY;

    // Start is called before the first frame update
    void Start()
    {
        arenaY = transform.localScale.y;
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
