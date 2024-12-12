using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlaneBumper : Bumper
{
    private bool isSphereCollider = false;
    private bool isPlaneCollider = false;

    [SerializeField] private SphereCollider sphere;

    private List<GameObject> sphereOverlaps = new List<GameObject>();
    private LayerMask layerMask;

    public UnityEvent<Transform> bounceEvent;

    private void Start()
    {
        string[] maskedLayers = { "Default", "IgnoreRaycast" };
        layerMask = LayerMask.GetMask(maskedLayers);
        layerMask = ~(layerMask); // invert layer mask
    }

    public void AddSphereOverlap(GameObject overlap)
    {
        sphereOverlaps.Add(overlap);
    }

    public void RemoveSphereOverlaps(GameObject overlap)
    {
        sphereOverlaps.Remove(overlap);
    }

    public void Collision(GameObject other)
    {
        if (!sphereOverlaps.Contains(other)) return;

        Debug.Log("Bumped");

        if (SFXManager.Instance != null)
        {
            if (soundType == BumperSoundType.Bounce)
                SFXManager.Instance.PlaySound("BumperBounce", other.gameObject.transform);
            else if (soundType == BumperSoundType.Plastic)
                SFXManager.Instance.PlaySound("FurnitureHit", other.gameObject.transform);
        }

        bounceEvent?.Invoke(other.transform);
        IBumper bumpedObject = other.GetComponent<IBumper>();
        if (bumpedObject == null) return;
        bumpedObject.Bump(transform.up, bounceForce, sourceType);

        //Debug.DrawRay(other.transform.position, transform.up, Color.magenta, 100f);
    }

    /// <summary>
    /// Checks if the player is colliding with the Plane and the Sphere, therefore inside the bounce radius
    /// </summary>
    /// <returns></returns>
    private bool checkCollision()
    {
        if (isSphereCollider == true && isPlaneCollider == true)
        {
            return true;
        }
        return false;
    }

    public override Vector3 GetBumpDirection(GameObject other)
    {
        if(sphereOverlaps.Contains(other))
            return transform.up;
        else
            return Vector3.zero;
    }

    public bool IsSphereCollider {  get { return isSphereCollider; } set { isSphereCollider = value; } }
    public bool IsPlaneCollider { get { return isPlaneCollider; } set { isPlaneCollider = value; }}
}
