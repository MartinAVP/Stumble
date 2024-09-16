using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCalc3 : MonoBehaviour
{
    [SerializeField] private GameObject viewer;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            Vector3 touchPos = collision.contacts[0].normal.normalized;
            viewer.transform.position = touchPos;
            Debug.DrawRay(this.transform.position, touchPos * 10, Color.yellow, 20f);
            Debug.DrawLine(this.transform.position, touchPos * 10, Color.green, 20f);
            Debug.Log("Player Bumper Hit");
        }
    }

/*    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            Vector3 touchPos = other.
        }
    }*/
}
