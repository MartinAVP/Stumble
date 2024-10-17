using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFlip : MonoBehaviour
{
    public bool triggered = false;

    public bool reset = false;

    private bool available = true;

    public Vector3 Edge1;
    public Vector3 Edge2;
    public Vector3 Edge3;
    public Vector3 Edge4;

    public GameObject ObjEdge1;
    public GameObject ObjEdge2;
    public GameObject ObjEdge3;
    public GameObject ObjEdge4;


    private Vector3 previousScale;
    private Vector3 previousPosition;

    void Start()
    {
        previousScale = transform.localScale;
        previousPosition = transform.position;
        edgeMather();

    }


    void FixedUpdate()
    {
        if (transform.localScale != previousScale || transform.position != previousPosition)
        {
            previousScale = transform.localScale;
            previousPosition = transform.position;

            edgeMather();
        }

        if (triggered && available)
        {

        }

        if (reset)
        {
            edgeMather();
        }

    }

    private void edgeMather()
    {
        //           ^
        //     2     z
        //  1 [] 3     x > 
        //    4
        //

        Edge1.Set(transform.position.x - transform.localScale.x / 2, transform.position.y + transform.localScale.y / 2, transform.position.z);
        Edge2.Set(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z + transform.localScale.z / 2);
        Edge3.Set(transform.position.x + transform.localScale.x / 2, transform.position.y + transform.localScale.y / 2, transform.position.z);
        Edge4.Set(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z - transform.localScale.z / 2);

        ObjEdge1.transform.position = Edge1;
        ObjEdge2.transform.position = Edge2;
        ObjEdge3.transform.position = Edge3;
        ObjEdge4.transform.position = Edge4;
    }


    // cube flips from lower edge to move platform

    // find cube edge's in code 

    // randomize which corner

    // rotate from calculated pivot point

    // speed modifier

    //












}
