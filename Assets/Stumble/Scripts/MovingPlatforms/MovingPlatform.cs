using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof (MovingPlatformData))]
public abstract class MovingPlatform : MonoBehaviour
{
    private Vector3 previousPosition;
    private Quaternion previousRotation;

    protected void Start()
    {
        Stack<Transform> stack = new Stack<Transform>();
        stack.Push(transform);

        // Add moving platform data to children
        while (stack.Count > 0)
        {
            Transform checkForData = stack.Pop();

            MovingPlatformData movingPlatformData = checkForData.GetComponent<MovingPlatformData>();
            if (movingPlatformData == null)
            {
                movingPlatformData = checkForData.AddComponent<MovingPlatformData>();
            }
            movingPlatformData.parent = this;

            foreach (Transform t in checkForData)
            {
                if (t.GetComponent<MovingPlatform>() == null)
                    stack.Push(t);
            }
        }

        FindManager();
    }

    protected void FindManager()
    {
        MovingPlatformManager manager = null;   
        Transform parent = transform.parent;
        while (parent != null)
        {
            manager = parent.GetComponent<MovingPlatformManager>();
            if (manager != null)
            {
                break;
            }

            parent = parent.parent;
        }

        if (manager == null)
        {
            manager = gameObject.AddComponent<MovingPlatformManager>();
            print("manger = " + manager.name);  
        }

        manager.AddMovingPlatform(this);
    }

    protected void OnDestroy()
    {
        print(gameObject);

        Stack<Transform> stack = new Stack<Transform>();
        stack.Push(transform);

        // Remove moving platform data from children
        while (stack.Count > 0)
        {
            Transform checkForData = stack.Pop();

            print(checkForData.gameObject);

            MovingPlatformData movingPlatformData = checkForData.GetComponent<MovingPlatformData>();
            if (movingPlatformData != null)
            {
                Destroy(movingPlatformData);
            }

            foreach (Transform t in checkForData)
            {
                if (t.GetComponent<MovingPlatform>() == null)
                    stack.Push(t);
            }
        }
    }

    public void UpdatePreviousPosition()
    {
        previousPosition = transform.position;
    }

    public void UpdatePreviousRotation()
    {
        previousRotation = transform.rotation;
    }

    public abstract void Move();

    public Vector3 ChangeInPosition
    {
        get { return transform.position - previousPosition; }
    }

    public Vector3 ChangeInRotation
    {
        get { return (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles; }
    }

}
