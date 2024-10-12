using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour
{
    // Events
    public event Action initializeScene;

    private PlayerInputManager playerInputManager;

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
        playerInputManager = FindAnyObjectByType<PlayerInputManager>();

        initializeScene?.Invoke();
        StartCoroutine(tempLockPlayers());
    }

    private IEnumerator tempLockPlayers()
    {
        playerInputManager.DisableJoining();
        yield return new WaitForSeconds(1f);
        playerInputManager.EnableJoining();

    }
}
