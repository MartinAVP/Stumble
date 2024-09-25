using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingBase : MonoBehaviour
{
    private Vector3 changeInPosition = Vector3.zero;
    private Vector3 changeInRotation = Vector3.zero;

    private Vector3 previousPosition = Vector3.zero;
    private Quaternion previousRotation;

    public Vector3 ChangeInPosition { get { resetPrevious = false; return transform.position - previousPosition; } }
    public Vector3 ChangeInRotation { get { resetPrevious = false; return (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles; } }

    public MovingBase ancestor;

    public bool resetPrevious = false;

    private void Start()
    {
        changeInPosition = transform.position - previousPosition;
        changeInRotation = Vector3.zero;

        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        changeInPosition = transform.position - previousPosition;
        changeInRotation = (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles;

        previousPosition = transform.position;
        previousRotation = transform.rotation;

        resetPrevious = true;
    }

    public void PropagateToChildren()
    {
        Stack<Transform> stack = new Stack<Transform>();
        stack.Push(transform);

        while (stack.Count > 0)
        {
            Transform checkForMovingBase = stack.Pop();

            Debug.Log("Checking " + checkForMovingBase.gameObject.name);

            MovingBase movingBase = checkForMovingBase.GetComponent<MovingBase>();
            if (movingBase == null)
            {
                movingBase = checkForMovingBase.AddComponent<MovingBase>();
            }
            movingBase.ancestor = ancestor;

            foreach (Transform t in checkForMovingBase)
            {
                stack.Push(t);
            }
        }
    }
}
