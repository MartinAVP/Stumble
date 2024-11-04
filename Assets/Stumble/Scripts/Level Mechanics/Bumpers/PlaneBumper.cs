using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlaneBumper : Bumper
{
    private bool isSphereCollider = false;
    private bool isPlaneCollider = false;

    [SerializeField] private SphereCollider sphere;
    private bool dirtyOverlaps = true;

    private List<GameObject> sphereOverlaps = new List<GameObject>();
    private LayerMask layerMask;

    private void FixedUpdate()
    {
        dirtyOverlaps = true;
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
        print("Sphere Overlaps");
        foreach(var overlap in sphereOverlaps)
        {
            print("Overlap: " + overlap.name);
        }

        if (!sphereOverlaps.Contains(other)) return;

        IBumper bumpedObject = other.GetComponent<IBumper>();
        if (bumpedObject == null) return;
        bumpedObject.Bump(transform.up, bounceForce, this);
        Debug.DrawRay(other.transform.position, transform.up, Color.magenta, 100f);

        // Sounds
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySound("BumperBounce", other.transform);
        }
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

    public bool IsSphereCollider {  get { return isSphereCollider; } set { isSphereCollider = value; } }
    public bool IsPlaneCollider { get { return isPlaneCollider; } set { isPlaneCollider = value; }}
}
