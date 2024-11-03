using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBumper : Bumper
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<IBumper>() != null)
        {
            //Debug.Log("Collided");

            RaycastHit hit;
            Vector3 direction = collision.gameObject.transform.position - transform.position;
            Vector3 invDirection = transform.position - collision.gameObject.transform.position;

            //Vector3 dirVec = other.transform.TransformDirection(direction);
            Debug.DrawRay(this.transform.position, direction, Color.magenta, 100f);
            if (Physics.Raycast(collision.gameObject.transform.position, invDirection, out hit, 100))
            {
                IBumper bumpedObject = collision.gameObject.GetComponent<IBumper>();
                if (bumpedObject == null) return;

                bumpedObject.Bump(hit.normal, bounceForce, this);
                Debug.DrawRay(hit.point, hit.normal, Color.cyan, 100f);
            }

            // Sounds
            if(SFXManager.Instance != null)
            {
                SFXManager.Instance.PlaySound("BumperBounce", collision.gameObject.transform);
            }
        }


    }
}
