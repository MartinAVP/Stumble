using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBumper : Bumper
{
    private bool isSphereCollider = false;
    private bool isPlaneCollider = false;

    public void Collision(Collider other)
    {
        if (checkCollision() == false) { return; }
        RaycastHit hit;
        Vector3 direction = other.transform.position - transform.position;
        Vector3 invDirection = transform.position - other.transform.position;

        //Vector3 dirVec = other.transform.TransformDirection(direction);
        Debug.DrawRay(this.transform.position, direction, Color.magenta, 100f);
        if (Physics.Raycast(other.transform.position, invDirection, out hit, 100))
        {
            IBumper bumpedObject = other.GetComponent<IBumper>();
            if (bumpedObject == null) return;
            bumpedObject.Bump(hit.normal, bounceForce, this);
            Debug.DrawRay(hit.point, hit.normal, Color.cyan, 100f);
        }

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
