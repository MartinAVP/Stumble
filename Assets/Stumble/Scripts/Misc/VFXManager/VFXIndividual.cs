using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXIndividual : MonoBehaviour
{
    [SerializeField] private VFXType type;
    [HideInInspector] public VFXplayer player;

    public float time;
    private bool running;
    [Space]
    [SerializeField] private bool playOnStart = true;

    public void Start()
    {
        if(playOnStart)
        {
            StartVFX();
        }
    }

    public void StartVFX()
    {
        if(type == VFXType.TimeBased)
        {
            if (running) return;                // Prevent Executing 2 Times by the same call
            running = true;

            StartCoroutine(delayTimer());       // Start Timer Delay

        }
    }

    private IEnumerator delayTimer()
    {
        yield return new WaitForSeconds(time);
        VFXManager.Instance.RemoveActiveVFX(player);
        Destroy(this.gameObject);
        running = false;
    }

    public void SetParent(Transform parent)
    {
        this.transform.SetParent(parent, false);
    }

    public enum VFXType
    {
        TimeBased,      // Will Loop and end based on time
        Cancelable,     // Will play and loop until canceled.
        None            // Default
    }
}
