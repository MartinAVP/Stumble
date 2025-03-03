using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnstablePlatformManager : MonoBehaviour
{
    public GameObject fallingObject;
    public UnstablePlatform unstablePlatform;
    public Rigidbody unstableRigidbody;
    public Vector3 startPosition;

    private bool triggered = false;

    public void Fall()
    {
        print("Start Falling");
        if (triggered == false)
        {
            triggered = true;
            StartCoroutine(StartFallDelay());
        }
    }

    private IEnumerator StartFallDelay()
    {
        unstablePlatform.onContact?.Invoke(unstablePlatform.TimeDelayBeforeFall);
        yield return new WaitForSeconds(unstablePlatform.TimeDelayBeforeFall);

        unstableRigidbody.isKinematic = false;
        unstableRigidbody.useGravity = true;

        unstablePlatform.onStartFalling?.Invoke();

        StartCoroutine(WaitWhileFalling());
    }

    private IEnumerator WaitWhileFalling()
    {
        yield return new WaitForSeconds(unstablePlatform.FallTime);

        fallingObject.SetActive(false);

        unstablePlatform.onEndFalling?.Invoke();

        StartCoroutine(WaitForRespawn());
    }

    private IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(unstablePlatform.TimeDelayForRespawn);

        fallingObject.SetActive(true);
        unstableRigidbody.isKinematic = true;
        unstableRigidbody.useGravity = false;
        fallingObject.transform.position = startPosition;
        triggered = false;
        unstablePlatform.onRespawn?.Invoke();
    }
}
