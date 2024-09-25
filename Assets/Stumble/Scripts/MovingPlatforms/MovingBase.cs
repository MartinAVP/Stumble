using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingBase : MonoBehaviour
{
    public MovingPlatform owner;

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
            movingBase.owner = owner;

            foreach (Transform t in checkForMovingBase)
            {
                stack.Push(t);
            }
        }
    }
}
