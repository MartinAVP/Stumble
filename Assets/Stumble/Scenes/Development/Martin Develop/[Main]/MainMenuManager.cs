using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // Events
    public event Action initializeScene;

    private void Awake()
    {
        DefineAsSingleton();
    }
    private void DefineAsSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public static MainMenuManager Instance;

    private void Start()
    {
        initializeScene?.Invoke();
    }
}
