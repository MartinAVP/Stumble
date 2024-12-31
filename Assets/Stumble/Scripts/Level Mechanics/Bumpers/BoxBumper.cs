using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxBumper : Bumper
{
    public UnityEvent<Vector3, Vector3> BounceEvent = new UnityEvent<Vector3, Vector3>();

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<IBumper>() != null)
        {
            Debug.Log("Collided");

            RaycastHit hit;
            Vector3 direction = collision.gameObject.transform.position - transform.position;
            Vector3 invDirection = transform.position - collision.gameObject.transform.position;

            //Vector3 dirVec = other.transform.TransformDirection(direction);
            Debug.DrawRay(this.transform.position, direction, Color.magenta, 100f);
            if (Physics.Raycast(collision.gameObject.transform.position, invDirection, out hit, 100))
            {
                IBumper bumpedObject = collision.gameObject.GetComponent<IBumper>();
                if (bumpedObject == null) return;

                bumpedObject.Bump(hit.normal, bounceForce, sourceType);
                Debug.DrawRay(hit.point, hit.normal, Color.cyan, 100f);
            }

            // Sounds
            BounceEvent?.Invoke(hit.point, hit.normal);
            Debug.Log("Invoked Bounce");

            if(SFXManager.Instance != null)
            {
                if(soundType == BumperSoundType.Bounce)
                    SFXManager.Instance.PlaySound("BumperBounce", collision.gameObject.transform);
                else if(soundType == BumperSoundType.Plastic)
                    SFXManager.Instance.PlaySound("FurnitureHit", collision.gameObject.transform);
            }

        }
    }

    public override Vector3 GetBumpDirection(GameObject other)
    {
        RaycastHit hit;
        Vector3 direction = other.transform.position - transform.position;
        Vector3 invDirection = transform.position - other.transform.position;

        //Vector3 dirVec = other.transform.TransformDirection(direction);

        Debug.DrawRay(this.transform.position, direction, Color.magenta, 100f);
        if (Physics.Raycast(other.transform.position, invDirection, out hit, 100))
        {
            BounceEvent?.Invoke(other.transform.position, direction);
            return hit.normal;
        }

        BounceEvent?.Invoke(other.transform.position, direction);
/*        BounceEvent?.Invoke(hit.point, hit.normal);*/

        return Vector3.zero;
    }

    public override void ExecuteBumpVFX()
    {

        return;
    }
}
