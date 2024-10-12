using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class ArenaGameManager : MonoBehaviour
{
    public event Action testEvent;

    private void OnEnable()
    {
        testEvent += TestCall;
    }

    private void OnDisable()
    {
        testEvent -= TestCall;
    }

    // Start is called before the first frame update
    void Start()
    {
        testEvent?.Invoke();
    }

    private void TestCall()
    {
        UnityEngine.Debug.LogWarning("What's up dude");
    }
}
