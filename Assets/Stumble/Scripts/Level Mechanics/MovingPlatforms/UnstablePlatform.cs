using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class UnstablePlatform : MonoBehaviour
{
    [SerializeField] private float timeDelayBeforeFall = 5f;
    [SerializeField] private float fallTime = 3f;
    [SerializeField] private float timeDelayForRespawn = 3;
    [SerializeField, Range(0, 1)] private float maxTintStrength = .65f;
    private Material material;
    private Color startColor;

    private GameObject managerObject;
    private UnstablePlatformManager unstablePlatformManager;

    [SerializeField] public UnityEvent<float> onContact;
    [SerializeField] public UnityEvent onStartFalling;
    [SerializeField] public UnityEvent onEndFalling;
    [SerializeField] public UnityEvent onRespawn;

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

        onContact.AddListener(OnContact);
        onRespawn.AddListener(OnRespawn);

        material = GetComponent<Renderer>().material;
        startColor = material.color;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("touched");
            unstablePlatformManager.Fall();
        }  
    }

    private void OnContact(float timeUntilFall)
    {
        StartCoroutine(MaterialInterpolate(timeUntilFall));
    }

    private void OnRespawn()
    {
        material.color = startColor;
    }

    private IEnumerator MaterialInterpolate(float duration)
    {
        float timeElapsed = 0;
        
        while (timeElapsed < duration)
        {
            material.color = Color.Lerp(startColor, (Color.red * startColor) + Color.red, timeElapsed/duration * maxTintStrength);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public float TimeDelayBeforeFall { get { return timeDelayBeforeFall; } }
    public float FallTime { get { return fallTime; } }
    public float TimeDelayForRespawn { get { return timeDelayForRespawn; } }
}
