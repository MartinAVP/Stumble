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
                other.GetComponent<IBumper>().Bump(hit.normal, bounceForce);
                Debug.DrawRay(hit.point, hit.normal, Color.cyan, 100f);
            }
        }

    }
}
