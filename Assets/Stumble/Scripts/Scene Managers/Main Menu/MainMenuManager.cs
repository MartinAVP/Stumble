using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour
{
    // Events
    public event Action initializeScene;

    private PlayerInputManager playerInputManager;

    public static MainMenuManager Instance { get; private set; }
    [HideInInspector] public bool initializedFinished = false;

    // Singleton
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // Start Search for Experimental Player Manager
        Task Setup = setup();
    }

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (GameController.Instance == null || GameController.Instance.enabled == false || GameController.Instance.initialized == false)
        {
            await Task.Delay(2);
        }

        // Once it finds it initialize the scene
        Debug.Log("Initializing Main Menu Manager...         [Main Menu Manager]");
        InitializeManager();
        return;
    }

    private void InitializeManager()
    {
        playerInputManager = FindAnyObjectByType<PlayerInputManager>();

        initializeScene?.Invoke();
        StartCoroutine(tempLockPlayers());

        initializedFinished = true;
    }

    // Prevent Player from Joining While there is the animation playing.
    private IEnumerator tempLockPlayers()
    {
        playerInputManager.DisableJoining();
        yield return new WaitForSeconds(1f);
        playerInputManager.EnableJoining();
    }
}
