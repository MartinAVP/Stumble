using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBumper : MonoBehaviour
{
    public bool isSphereCollider = false;
    public bool isPlaneCollider = false;
    public float bounceForce = 10f;

    public void Collision(Collider other)
    {
        if(checkCollision() == false) { return; }
        RaycastHit hit;
        Vector3 direction = other.transform.position - transform.position;
        Vector3 invDirection = transform.position - other.transform.position;

        //Vector3 dirVec = other.transform.TransformDirection(direction);
        Debug.DrawRay(this.transform.position, direction, Color.magenta, 100f);
        if (Physics.Raycast(other.transform.position, invDirection, out hit, 100))
        {
            //Debug.Log("Hitted");
            Debug.Log(hit.transform.name);
            

            Debug.DrawRay(hit.point, hit.normal, Color.cyan, 100f);

            if (other.transform.GetComponent<Rigidbody>() != null)
            {
                //Debug.Log("Has RB");
                other.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.transform.GetComponent<Rigidbody>().AddForce(hit.normal * bounceForce, ForceMode.Impulse);
            }
        }
    }

    private bool checkCollision()
    {
        if(isSphereCollider == true && isPlaneCollider == true)
        {
            return true;
        }
        return false;
    }
}
