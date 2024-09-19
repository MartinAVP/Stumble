using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnstablePlatform : MonoBehaviour
{
    public float timeDelayBeforeFall = 5f;
    public float timeDelayForRespawn = 3f;

    public bool triggered = false;

    private Vector3 startingPosition;
    private Quaternion startingRotation;

    private Rigidbody platforRigidBody;
    private Collider platformCollider;


    void Awake()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;

        platforRigidBody = GetComponent<Rigidbody>();
        platformCollider = GetComponent<Collider>();

        platforRigidBody.isKinematic = true;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (triggered != true)
            {
                Debug.Log("touched");
                triggered = true;
                StartCoroutine(Fall());
                /*
                 * Bad code please ignore
                Debug.Log(triggered);

                while (triggeredCounter < timeDelayBeforeFall)
                {
                    triggeredCounter += Time.deltaTime;
                }
                Debug.Log(triggeredCounter);

                triggeredCounter = 0f;

                platforRigidBody.isKinematic = false;
                platforRigidBody.useGravity = true;
                */
            }
        }  
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(timeDelayBeforeFall);

        platforRigidBody.isKinematic = false;
        platforRigidBody.useGravity = true;

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(timeDelayForRespawn);

        platforRigidBody.velocity = Vector3.zero;
        platforRigidBody.angularVelocity = Vector3.zero;

        platforRigidBody.isKinematic = true;
        platforRigidBody.useGravity = false;

        transform.position = startingPosition;
        transform.rotation = startingRotation;

        triggered = false;
    }
}
