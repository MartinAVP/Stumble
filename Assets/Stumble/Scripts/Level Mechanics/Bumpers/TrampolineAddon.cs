using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineAddon : MonoBehaviour
{
    public void TriggerBounce(Transform pos)
    {
        if(VFXManager.Instance != null)
        {
            Debug.Log("Triggered Trampoline VFX");
            VFXManager.Instance.PlayVFX("Trampoline", pos.position);
        }
    }
}
