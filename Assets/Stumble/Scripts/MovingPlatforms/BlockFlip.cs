using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFlip : MonoBehaviour
{
    public bool flipping = false;

    public float rotationSpeed = 2f;

    //delay before next flip
    private float delayTime = 1f;
    private float delayCounter = 0f;
    private bool delayed = false;

    void FixedUpdate()
    {
        if (flipping == true)
        {
            //transform.RotateAround(transform.position, startAxis, rotationSpeed * Time.deltaTime);

        }
    }
}
