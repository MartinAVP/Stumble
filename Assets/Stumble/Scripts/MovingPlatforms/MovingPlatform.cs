using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof (MovingPlatformData))]
public abstract class MovingPlatform : MonoBehaviour
{
    [SerializeField] protected List<MovingPlatformData> movingPlatformsData = new List<MovingPlatformData>();

    public delegate void OnPreMovePlatforms();
    public OnPreMovePlatforms onPreMovePlatforms;

    public delegate void OnMovePlatforms();
    public OnMovePlatforms onMovePlatforms;
    
    public delegate void OnPostMovePlatforms();
    public OnPostMovePlatforms onPostMovePlatforms;

     [SerializeField] private MovingPlatform manager = null;

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
            movingPlatformsData.Add(movingPlatformData);

            foreach (Transform t in checkForData)
            {
                // Won't search branches that have another moving platform component since the data component is required with this.
                if (t.GetComponent<MovingPlatform>() == null)
                    stack.Push(t);
            }
        }

        FindManager();
    }

    protected void FindManager()
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            manager = parent.GetComponent<MovingPlatform>();

            parent = parent.parent;
        }

        if(manager == null)
            manager = this;

        manager.onPreMovePlatforms += UpdatePreviousPositionRotations;
        manager.onMovePlatforms += Move;
        manager.onPostMovePlatforms += UpdateDeltas;
    }

    protected void Update()
    {
        onPreMovePlatforms?.Invoke();
        onMovePlatforms?.Invoke();
        onPostMovePlatforms?.Invoke();
    }

    protected void UpdatePreviousPositionRotations()
    {
        foreach (var data in movingPlatformsData)
        {
            data.UpdatePreviousPosition();
            data.UpdatePreviousRotation();
        }
    }

    protected void UpdateDeltas()
    {
        foreach (var data in movingPlatformsData)
        {
            data.UpdateDeltas(Time.deltaTime);
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
