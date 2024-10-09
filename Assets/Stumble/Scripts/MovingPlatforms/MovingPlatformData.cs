using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformData : MonoBehaviour
{
    [SerializeField] private Vector3 previousPosition;
    [SerializeField]  private Vector3 linearVelocity;
    [SerializeField]  private Quaternion previousRotation;

    /*  Deprecated content is still being reference by StaticPlayerMovement.
     *  Once StaticPlayerMovement is deleted this code can be removed.
     */
    #region Deprecated
    [SerializeField] public MovingPlatform parent;
    #endregion

    private void LateUpdate()
    {
        linearVelocity = (transform.position - previousPosition) / Time.deltaTime;
    }

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

    public Vector3 LinearVelocity
    {
        get { return linearVelocity; }
    }
}
