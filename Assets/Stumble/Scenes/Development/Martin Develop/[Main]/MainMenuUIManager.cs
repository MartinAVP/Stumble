using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("Panel For Fading")]
    [SerializeField] private GameObject unityScreenPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject startScreenPanel;

    [Header("Main Buttons")]
    [SerializeField] private UnityEngine.UI.Button _startGameButton;
    [SerializeField] private UnityEngine.UI.Button _optionsButton;
    [SerializeField] private UnityEngine.UI.Button _creditsButton;
    [SerializeField] private UnityEngine.UI.Button _achievementsButton;
    [SerializeField] private UnityEngine.UI.Button _ExitButton;

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = FindAnyObjectByType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += joinHostPlayer;

        // Buttons
        _startGameButton?.onClick.AddListener(StartGameCoroutine);
        _optionsButton?.onClick.AddListener(OpenOptions);
        _creditsButton?.onClick.AddListener(OpenCredits);
        _achievementsButton?.onClick.AddListener(OpenAchievements);
        _ExitButton?.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= joinHostPlayer;

        // Buttons
        _startGameButton?.onClick.RemoveAllListeners();
        _optionsButton?.onClick.RemoveAllListeners();
        _creditsButton?.onClick.RemoveAllListeners();
        _achievementsButton?.onClick.RemoveAllListeners();
        _ExitButton?.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        // Initialize Panels
        unityScreenPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        startScreenPanel.SetActive(true);

        // Disable the Transition
        StartCoroutine(disableUnityTransition());

        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(); }
    }

    private void joinHostPlayer(PlayerInput player)
    {
        StartCoroutine(changeToMainMenu());
    }

    private IEnumerator changeToMainMenu()
    {
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(); }
        yield return new WaitForSeconds(2f);
        mainMenuPanel.SetActive(true);
        startScreenPanel.SetActive(false);
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(); }
    }

    private IEnumerator disableUnityTransition()
    {
        yield return new WaitForSeconds(3f);
        unityScreenPanel.SetActive(false);
    }

    private void StartGameCoroutine() {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        //Debug.Log("Start Game");
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(); }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Lobby");
    }

    private void OpenOptions()
    {
        Debug.Log("Open Options");
    }

    private void OpenCredits()
    {
        Debug.Log("Open Credits");
    }

    private void OpenAchievements()
    {
        Debug.Log("Open Achievements");
    }

    private void ExitGame()
    {
        //Debug.Log("Exit Game");
        Application.Quit();
    }
}
