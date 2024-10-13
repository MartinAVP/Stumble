using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBumper : MonoBehaviour
{
    public float bounceForce;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<IBumper>() != null)
        {
            //Debug.Log("Collided");

            RaycastHit hit;
            Vector3 direction = other.transform.position - transform.position;
            Vector3 invDirection = transform.position - other.transform.position;

            //Vector3 dirVec = other.transform.TransformDirection(direction);
            Debug.DrawRay(this.transform.position, direction, Color.magenta, 100f);
            if (Physics.Raycast(other.transform.position, invDirection, out hit, 100))
            {
                /*                Debug.Log("Hitted: " + hit.transform.name);

                                Debug.DrawRay(hit.point, hit.normal, Color.cyan, 100f);

                                if (other.transform.GetComponent<Rigidbody>() != null)
                                {
                                    other.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                                    other.transform.GetComponent<Rigidbody>().AddForce(hit.normal * bounceForce, ForceMode.Impulse);
                                }*/

                // Potential Momentum Keep
                Vector3 fwd = other.transform.forward.normalized;
                fwd = fwd * other.GetComponent<ThirdPersonMovement>().horizontalVelocity / 100;
                Vector3 dir = fwd + hit.normal;
                //Vector3 dir = hit.normal;
                //Vector3 dir = fwd + hit.normal;
                other.GetComponent<IBumper>().Bump(dir, bounceForce);
                Debug.DrawRay(hit.point, hit.normal, Color.cyan, 100f);
            }

            // Sounds
            if(SFXManager.Instance != null)
            {
                SFXManager.Instance.PlaySound("BumperBounce", other.transform);
            }
        }

    }
}
