using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof (MovingPlatformData))]
public abstract class MovingPlatform : MonoBehaviour
{
    [SerializeField] protected List<MovingPlatformData> movingPlatforms = new List<MovingPlatformData>();

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
            movingPlatforms.Add(movingPlatformData);

            foreach (Transform t in checkForData)
            {
                // Won't search branches that have another moving platform component since the data component is required with this.
                if (t.GetComponent<MovingPlatform>() == null)
                    stack.Push(t);
            }
        }
    }

    protected void Update()
    {
        Move();
    }

    protected void LateUpdate()
    {
        foreach (MovingPlatformData movingPlatformData in movingPlatforms)
        {
            movingPlatformData.UpdatePreviousPosition();
            movingPlatformData.UpdatePreviousRotation();
        }
    }

    public abstract void Move();

    /*  Deprecated content is still being reference by StaticPlayerMovement.
     *  Once StaticPlayerMovement is deleted this code can be removed.
     */
    #region Deprecated

    private Vector3 previousPosition;
    private Quaternion previousRotation;


    public void UpdatePreviousPosition()
    {
        previousPosition = transform.position;
    }

    public void UpdatePreviousRotation()
    {
        previousRotation = transform.rotation;
    }

    public Vector3 ChangeInPosition
    {
        get { return transform.position - previousPosition; }
    }

    public Vector3 ChangeInRotation
    {
        get { return (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles; }
    }

    #endregion
}
