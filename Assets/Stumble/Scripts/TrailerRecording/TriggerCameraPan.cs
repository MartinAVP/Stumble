using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TriggerCameraPan : MonoBehaviour
{
    public GameObject[] turnOnObjects;
    public CinemachineDollyCart[] dollyCarts;
    public float dollySpeed = 0.075f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var turnOnObject in turnOnObjects)
        {
            turnOnObject.SetActive(true);
        }

        foreach (var dollyObject in dollyCarts)
        {
            dollyObject.m_Speed = dollySpeed;
        }
    }
}
