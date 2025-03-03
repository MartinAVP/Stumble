using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Events
    public event Action initializeScene;

    private PlayerInputManager playerInputManager;

    public static MainMenuManager Instance { get; private set; }
    private bool transitioning = false;
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
        //playerInputManager.EnableJoining();
    }

    public void StartGame(PlayerInput input)
    {
        if (PlayerDataHolder.Instance.GetPlayerData(input)?.isHost == false)
        {
            return;
        }

        if (transitioning) return;
        transitioning = true;

        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        StartCoroutine(delayStart());
    }

    private IEnumerator delayStart()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Lobby");
    }

    private bool isInputLock = false;
    public void ExitGame(PlayerInput input)
    {
        if(isInputLock) { return; }
        isInputLock = true;

        StartCoroutine(delayPress());

        Debug.LogWarning("Checkpointed #1");
        if (PlayerDataHolder.Instance.GetPlayerData(input).isHost == false)
        {
            return;
        }

        if(MainMenuUIManager.Instance.currentMenu == MainMenuUIManager.Menu.Controls)
        {
            MainMenuUIManager.Instance.returnToMainMenuFromControls();
            return;
        }

        if (MainMenuUIManager.Instance.currentMenu == MainMenuUIManager.Menu.Options)
        {
            MainMenuUIManager.Instance.returnToMainMenuFromOptions();
            return;
        }

        // Is on Main Menu
        if (transitioning) return;
        transitioning = true;

        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        StartCoroutine(delayExit());
    }

    private IEnumerator delayPress()
    {
        yield return new WaitForSeconds(1.1f);
        isInputLock = false;
    }

    private IEnumerator delayExit()
    {
        yield return new WaitForSeconds(2f);
        //PlayerDataHolder.Instance.ClearAllButHost(true);
        Application.Quit();
        //SceneManager.LoadScene("Menu");
    }

    public void OpenCredits()
    {
        if (transitioning) return;
        transitioning = true;

        StartCoroutine(LoadCredits());
    }

    private IEnumerator LoadCredits()
    {
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Credits");
    }
}
