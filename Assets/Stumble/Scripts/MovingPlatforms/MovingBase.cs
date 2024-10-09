using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingBase : MonoBehaviour
{
    private Vector3 previousPosition = Vector3.zero;
    private Quaternion previousRotation;

    public Vector3 ChangeInPosition { get { return transform.position - previousPosition; } }
    public Vector3 ChangeInRotation { get { return (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles; } }

    public MovingBase ancestor;

    private void Start()
    {
        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }

    public void UpdatePreviousRotation()
    {
        previousPosition = transform.position;
        previousRotation = transform.rotation;
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
