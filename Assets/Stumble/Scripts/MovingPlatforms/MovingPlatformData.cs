using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformData : MonoBehaviour
{
    private Vector3 previousPosition;
    private Vector3 linearVelocity;
    private Quaternion previousRotation;
    
    private float deltaTime;
    private Vector3 previousFixedPosition;
    [SerializeField] private Vector3 changeInPosition;
    [SerializeField] private Vector3 changeInRotation;

    /*  Deprecated content is still being reference by StaticPlayerMovement.
     *  Once StaticPlayerMovement is deleted this code can be removed.
     */
    #region Deprecated
    [SerializeField] public MovingPlatform parent;
    #endregion

    private void LateUpdate()
    {

    }

    public void UpdatePreviousPosition()
    {
        //changeInPosition = transform.position - previousPosition;
        previousPosition = transform.position;
    }

    public void UpdatePreviousRotation()
    {
        //changeInRotation = (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles;
        previousRotation = transform.rotation;
    }

    public void UpdateDeltas(float deltaTime)
    {
        changeInPosition = transform.position - previousPosition;
        changeInRotation = (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles;

        this.deltaTime = deltaTime;
        linearVelocity = (transform.position - previousPosition) / deltaTime;
    }

    public Vector3 ChangeInPosition
    {
        get { return changeInPosition; }
    }

    public Vector3 ChangeInRotation
    {
        get { return changeInRotation; }
    }

    public Vector3 LinearVelocity
    {
        get { return linearVelocity; }
    }

    /// <summary>
    /// The delta time of the previous frame, when this was last presumably updated.
    /// </summary>
    public float DeltaTime { get { return deltaTime; } }    
}
