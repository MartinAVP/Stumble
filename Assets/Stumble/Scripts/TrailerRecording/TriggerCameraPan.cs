using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCameraPan : MonoBehaviour
{
    public GameObject cameraPanObject;

    private void OnTriggerEnter(Collider other)
    {
        cameraPanObject.SetActive(true);
    }
}
