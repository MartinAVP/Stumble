using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXCaller : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlaySound(string name)
    {
        if(SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySound(name, this.transform);
        }
    }
}
