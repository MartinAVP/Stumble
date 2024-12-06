using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineAddon : MonoBehaviour
{
    public void TriggerTrampolineBounce(Transform pos)
    {
        if(VFXManager.Instance != null)
        {
            Debug.Log("Triggered Trampoline VFX");
            VFXManager.Instance.PlayVFX("Trampoline", pos.position + new Vector3(0,-.6f,0));
        }
    }

    public void TriggerBouncerBounce(Vector3 position, Vector3 direction)
    {
        Debug.Log("[!] Inside Trigger Bounce");
        if (VFXManager.Instance != null)
        {
            Debug.Log("Triggered Bouncer VFX");
            VFXManager.Instance.PlayVFX("Bumper", position + new Vector3(0, -.6f, 0));
        }
    }
}
