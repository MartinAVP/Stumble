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
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("touched");
            unstablePlatformManager.Fall();
        }  
    }

    private void OnContact(float timeUntilFall)
    {
        StartCoroutine(MaterialInterpolate(timeUntilFall));
    }

    private IEnumerator MaterialInterpolate(float duration)
    {
        material =  GetComponent<Renderer>().material;
        Vector3 baseColor = new Vector3(material.color.r, material.color.g, material.color.b);
        float alpha = material.color.a;
        Vector3 red = new Vector3(255, 0, 0);
        float timeElapsed = 0;
        
        while (timeElapsed < duration)
        {
            Vector3 newColor = Vector3.Lerp(baseColor, red, timeElapsed / duration * maxTintStrength);
            newColor[0] = Mathf.Clamp(newColor[0], 0, 255);
            material.color = new Color(newColor[0], newColor[1], newColor[2], alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public float TimeDelayBeforeFall { get { return timeDelayBeforeFall; } }
    public float FallTime { get { return fallTime; } }
    public float TimeDelayForRespawn { get { return timeDelayForRespawn; } }
}
