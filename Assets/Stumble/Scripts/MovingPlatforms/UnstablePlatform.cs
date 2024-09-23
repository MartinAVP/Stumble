using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnstablePlatform : MonoBehaviour
{
    [SerializeField] private float timeDelayBeforeFall = 5f;
    [SerializeField] private float fallTime = 3f;
    [SerializeField] private float timeDelayForRespawn = 3;

    private GameObject managerObject;
    private UnstablePlatformManager unstablePlatformManager;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        managerObject = new GameObject();
        unstablePlatformManager = managerObject.AddComponent<UnstablePlatformManager>();
        unstablePlatformManager.startPosition = transform.position;
        unstablePlatformManager.unstablePlatform = this;
        unstablePlatformManager.fallingObject = gameObject;
        unstablePlatformManager.unstableRigidbody = rb;

        rb.freezeRotation = true;
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("touched");
            unstablePlatformManager.Fall();
        }  
    }

    public float TimeDelayBeforeFall { get { return timeDelayBeforeFall; } }
    public float FallTime { get { return fallTime; } }
    public float TimeDelayForRespawn { get { return timeDelayForRespawn; } }
}
