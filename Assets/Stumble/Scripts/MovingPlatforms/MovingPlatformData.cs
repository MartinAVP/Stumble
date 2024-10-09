using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformData : MonoBehaviour
{
    [SerializeField] private Vector3 previousPosition;
    [SerializeField] private Vector3 linearVelocity;
    [SerializeField] private Quaternion previousRotation;
    
    private float deltaTime;
    private Vector3 previousFixedPosition;
    private Vector3 changeInPosition;
    private Vector3 changeInRotation;

    /*  Deprecated content is still being reference by StaticPlayerMovement.
     *  Once StaticPlayerMovement is deleted this code can be removed.
     */
    #region Deprecated
    [SerializeField] public MovingPlatform parent;
    #endregion

    private void LateUpdate()
    {
        linearVelocity = (transform.position - previousPosition) / Time.deltaTime;
        deltaTime = Time.deltaTime;
    }

    public void UpdatePreviousPosition()
    {
        changeInPosition = transform.position - previousPosition;
        previousPosition = transform.position;
    }

    public void UpdatePreviousRotation()
    {
        changeInRotation = (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles;
        previousRotation = transform.rotation;
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
