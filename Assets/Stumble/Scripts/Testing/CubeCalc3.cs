using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCalc3 : MonoBehaviour
{
    // The force with which the sphere will bounce
    public float bounceForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");

        RaycastHit hit;
        Vector3 direction = other.transform.position - transform.position;
        Vector3 invDirection = transform.position - other.transform.position;

        //Vector3 dirVec = other.transform.TransformDirection(direction);
        Debug.DrawRay(this.transform.position, direction, Color.magenta, 100f);
        if (Physics.Raycast(other.transform.position, invDirection, out hit, 100))
        {
            Debug.Log("Hitted: " + hit.transform.name);

            Debug.DrawRay(hit.point, hit.normal, Color.cyan, 100f);

            if (other.transform.GetComponent<Rigidbody>() != null)
            {
                other.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.transform.GetComponent<Rigidbody>().AddForce(hit.normal * bounceForce, ForceMode.Impulse);
            }
        }
    }

    /*    private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Collided");

            RaycastHit hit;
            Vector3 direction = other.transform.position - transform.position;
            Vector3 invDirection = transform.position - other.transform.position;

            //Vector3 dirVec = other.transform.TransformDirection(direction);
            Debug.DrawRay(this.transform.position, direction, Color.magenta, 100f);
            if (Physics.Raycast(other.transform.position, invDirection, out hit, 100))
            {
                Debug.Log("Hitted");

                Debug.DrawRay(hit.point, hit.normal, Color.cyan, 100f);
                //Instantiate(new GameObject(), hit.normal, Quaternion.identity);

                // Find the line from the gun to the point that was clicked.
                Vector3 incomingVec = hit.point - this.transform.position;

                // Use the point's normal to calculate the reflection vector.
                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);

                // Draw lines to show the incoming "beam" and the reflection.
                Debug.DrawLine(this.transform.position, hit.point, Color.red, 100f);
                Debug.DrawRay(hit.point, reflectVec, Color.green, 100f);
            }
        }*/
}
