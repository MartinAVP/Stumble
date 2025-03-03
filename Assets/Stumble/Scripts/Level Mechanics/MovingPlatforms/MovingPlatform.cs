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

    protected void Start()
    {
        Stack<Transform> stack = new Stack<Transform>();
        stack.Push(transform);

        // Add moving platform data to children
        while (stack.Count > 0)
        {
            Transform checkForData = stack.Pop();

            foreach (Transform t in checkForData)
            {
                // Won't search branches that have another moving platform component since the data component is required with this.
                if (t.GetComponent<MovingPlatform>() == null)
                    stack.Push(t);
            }

            if (checkForData.GetComponent<Collider>() == null)
            {
                continue;
            }
            MovingPlatformData movingPlatformData = checkForData.GetComponent<MovingPlatformData>();
            if (movingPlatformData == null)
            {
                movingPlatformData = checkForData.AddComponent<MovingPlatformData>();
            }
            movingPlatformData.parent = this;
            movingPlatformsData.Add(movingPlatformData);
        }

        MovingPlatformManager manager = MovingPlatformManager.Instance;

        MovingPlatformEventBus.Subscribe(MovingPlatformEvent.PreMove, UpdatePreviousPositionRotations);
        MovingPlatformEventBus.Subscribe(MovingPlatformEvent.Move, Move);
        MovingPlatformEventBus.Subscribe(MovingPlatformEvent.PostMove, UpdateDeltas);
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

    private void OnDestroy()
    {
        MovingPlatformEventBus.Unsubscribe(MovingPlatformEvent.PreMove, UpdatePreviousPositionRotations);
        MovingPlatformEventBus.Unsubscribe(MovingPlatformEvent.Move, Move);
        MovingPlatformEventBus.Unsubscribe(MovingPlatformEvent.PostMove, UpdateDeltas);
    }
}
