using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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
    [SerializeField] private GameObject optionsPanel;

    [Header("Main Buttons")]
    [SerializeField] private UnityEngine.UI.Button _startGameButton;
    [SerializeField] private UnityEngine.UI.Button _optionsButton;
    [SerializeField] private UnityEngine.UI.Button _creditsButton;
    [SerializeField] private UnityEngine.UI.Button _achievementsButton;
    [SerializeField] private UnityEngine.UI.Button _ExitButton;

    [Header("Options")]
    [SerializeField] private UnityEngine.UI.Slider _generalVolume;
    [SerializeField] private UnityEngine.UI.Slider _MusicVolume;
    [SerializeField] private UnityEngine.UI.Slider _SFXVolume;
    [SerializeField] private UnityEngine.UI.Button _ReturnToMenuFromOptions;

    private PlayerInputManager playerInputManager;
    [SerializeField] private MultiplayerEventSystem multiplayerEventSystem;

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

        _generalVolume.onValueChanged.AddListener(changeGeneralVolume);
        _MusicVolume.onValueChanged.AddListener(changeMusicVolume);
        _SFXVolume.onValueChanged.AddListener(changeSFXVolume);
        _ReturnToMenuFromOptions.onClick.AddListener(returnToMainMenuFromOptions);
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

        _generalVolume.onValueChanged.RemoveAllListeners();
        _MusicVolume.onValueChanged.RemoveAllListeners();
        _SFXVolume.onValueChanged.RemoveAllListeners();
        _ReturnToMenuFromOptions.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        // Initialize Panels
        unityScreenPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        startScreenPanel.SetActive(true);

        // Disable the Transition
        StartCoroutine(disableUnityTransition());

        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(false); }

        changeGeneralVolume(0);
        changeMusicVolume(0);
        changeSFXVolume(0);
    }

    private void joinHostPlayer(PlayerInput player)
    {
        StartCoroutine(changeToMainMenu());
    }

    private IEnumerator changeToMainMenu()
    {
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        yield return new WaitForSeconds(3f);
        mainMenuPanel.SetActive(true);
        startScreenPanel.SetActive(false);
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(false); }
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
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Lobby");
    }

    private void OpenOptions()
    {
        GamemodeSelectScreenManager.Instance.InterpolateScreens(mainMenuPanel, optionsPanel, GamemodeSelectScreenManager.Direction.Left);
        ControllerForMenus.Instance.ChangeSelectedObject(_generalVolume.gameObject);
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

    private void returnToMainMenuFromOptions()
    {
        GamemodeSelectScreenManager.Instance.InterpolateScreens(optionsPanel, mainMenuPanel, GamemodeSelectScreenManager.Direction.Right);
    }

    public void Slider(InputAction.CallbackContext context)
    {
        Vector2 raw = context.ReadValue<Vector2>();
        if(raw.x > .5f)
        {
            if (multiplayerEventSystem.currentSelectedGameObject.GetComponent<UnityEngine.UI.Slider>() != null)
            {
                multiplayerEventSystem.currentSelectedGameObject.GetComponent<UnityEngine.UI.Slider>().value += raw.x * Time.deltaTime;
            }
        }
    }

    private void changeGeneralVolume(float value)
    {
        if(OptionsManager.Instance != null)
        {
            OptionsManager.Instance.SetGeneralVolume(value);
        }
    }

    private void changeMusicVolume(float value)
    {
        if (OptionsManager.Instance != null)
        {
            OptionsManager.Instance.SetMusicVolume(value);
        }
    }

    private void changeSFXVolume(float value)
    {
        if (OptionsManager.Instance != null)
        {
            OptionsManager.Instance.SetSFXVolume(value);
        }
    }
}
