using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestHandler : MonoBehaviour
{
    public event Action startSystems;

    public static TestHandler Instance { get; private set; }

    // Executed on Awake
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
        {
            Debug.Log("Clicked the button with text");
            startSystems?.Invoke();
        }
    }
}
