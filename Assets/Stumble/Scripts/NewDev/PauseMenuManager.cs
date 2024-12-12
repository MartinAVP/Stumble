using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("Panel For Fading")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Main Buttons")]
    [SerializeField] private UnityEngine.UI.Button _startGameButton;
    [SerializeField] private UnityEngine.UI.Button _optionsButton;
    [SerializeField] private UnityEngine.UI.Button _controlsButton;
    //[SerializeField] private UnityEngine.UI.Button _creditsButton;
    //[SerializeField] private UnityEngine.UI.Button _achievementsButton;
    //[SerializeField] private UnityEngine.UI.Button _ExitButton;

    [Header("Options")]
    [SerializeField] private UnityEngine.UI.Slider _generalVolume;
    [SerializeField] private UnityEngine.UI.Slider _MusicVolume;
    [SerializeField] private UnityEngine.UI.Slider _SFXVolume;
    [SerializeField] private UnityEngine.UI.Slider _TargetFPS;
    [SerializeField] private TextMeshProUGUI _TargetFPSText;
    [Space]
    [SerializeField] private TextMeshProUGUI _generalVolumeText;
    [SerializeField] private TextMeshProUGUI _musicVolumeText;
    [SerializeField] private TextMeshProUGUI _SFXVolumeText;
    [Space]
    [SerializeField] private UnityEngine.UI.Button _ReturnToMenuFromOptions;

    [Header("Controls")]
    [SerializeField] private Button _leftArrowControls;
    [SerializeField] private Button _rightArrowControls;
    [SerializeField] private TextMeshProUGUI _controlScheme;
    [Space]

    private bool isDisplayed = false;
    private bool transitioning = false;
    private bool canChange = true;

    private pauseLocation currentMenu = pauseLocation.None;

    public static PauseMenuManager Instance { get; private set; }
    [HideInInspector] public bool initialized = false;

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

        //
        Task Setup = setup();
    }

    private bool menuManagerFound = false;

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (PlayerDataHolder.Instance == null || PlayerDataHolder.Instance.enabled == false)
        {
            // Await 5 ms and try finding it again.
            // It is made 5 seconds because it is
            // a core gameplay mechanic.
            await Task.Delay(2);
        }

        // Once it finds it initialize the scene
        Debug.Log("Initializing Pause Menu UI Manager...         [Pause Manager]");

        //playerInputManager = FindAnyObjectByType<PlayerInputManager>();

        //InitializeManagerSubs();
        InitializeManager();
        menuManagerFound = true;
        return;
    }

    private void Start()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    private void InitializeManager()
    {
        PlayerInput input = PlayerDataHolder.Instance.GetPlayerData(0).input;
        //input.
    }

    public void TogglePauseMenu(PlayerInput input, bool isContinue)
    {
        bool hostIsController = false;

        // Check Permission
        if (!PlayerDataHolder.Instance.GetPlayerData(input).isHost) return;

        // Check for Control Scheme
        // Start for GamePad
        // Esc for Keyboard
        if(input.currentControlScheme == "Gamepad")
        {
            if (!isContinue) return;
            hostIsController = true;
        }
        else
        {
            if(isContinue) return;
            hostIsController = false;
        }

        // Prevent Exit Pause Menu from Options Menu
        if (currentMenu == pauseLocation.Options) return;

        if(isDisplayed)
        {
            isDisplayed = false;
            // Turn Off
            mainMenuPanel.SetActive(false);
            optionsPanel.SetActive(false);
            if(hostIsController) { ControllerForMenus.Instance.ChangeSelectedObject(null); }

            Time.timeScale = 1f;
            currentMenu = pauseLocation.None;
        }
        else
        {
            isDisplayed = true;
            // Turn On
            mainMenuPanel.SetActive(true);
            optionsPanel.SetActive(true);

            if(hostIsController) { ControllerForMenus.Instance?.ChangeObjectSelectedWithDelay(_startGameButton.gameObject, .5f); }
            
            Time.timeScale = 0f;
            currentMenu = pauseLocation.Pause;
        }
    }

    public void StopGame()
    {
        if (transitioning) { return; }
        transitioning = true;
        //StartCoroutine(delayTransition());

        canChange = false;
        StartCoroutine(returnToMenuCooldown());
    }

    private IEnumerator returnToMenuCooldown()
    {
        yield return new WaitForSeconds(20f);

        if(MenuMusicController.Instance != null )
        {
            MenuMusicController.Instance.StartMusic();
        }
        if(GameMusicController.Instance != null)
        {
            GameMusicController.Instance.EndMusic(1.2f);
        }

        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        yield return new WaitForSeconds(2f);

        if (ModularController.Instance != null)
        {
            // Reset all player Points
            foreach (PlayerData data in PlayerDataHolder.Instance.GetPlayers())
            {
                data.points = 0;
            }
        }

        // Destroy the Object
        Destroy(ModularController.Instance.gameObject);
        SceneManager.LoadScene("GamemodeSelect");
    }

    public void TogglePauseMenuFromMenu()
    {
        if (!canChange) { return; }
        if (isDisplayed)
        {
            isDisplayed = false;
            // Turn Off
            mainMenuPanel.SetActive(false);
            optionsPanel.SetActive(false);

            Time.timeScale = 1f;
            currentMenu = pauseLocation.None;
        }
        else
        {
            isDisplayed = true;
            // Turn On
            mainMenuPanel.SetActive(true);
            optionsPanel.SetActive(true);

            Time.timeScale = 0f;
            currentMenu = pauseLocation.Pause;
        }
    }

    public void StartGameCoroutine()
    {
        MainMenuManager.Instance.StartGame(PlayerDataHolder.Instance.GetPlayerData(0).input);
        /*        Debug.Log("Clicked");             // MOVED TO THE MAIN MENU MANAGER
                if(transfering) { return; }
                StartCoroutine(StartGame());*/
    }

    private IEnumerator StartGame()
    {
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Lobby");
    }

    public void OpenOptions()
    {
        if(transitioning) { return; }
        transitioning = true;
        StartCoroutine(delayTransition());

        Debug.Log("OPTIONS OPENING");
        GamemodeSelectScreenManager.Instance.InterpolateScreens(mainMenuPanel, optionsPanel, GamemodeSelectScreenManager.Direction.Left);
        ControllerForMenus.Instance.ChangeObjectSelectedWithDelay(_ReturnToMenuFromOptions.gameObject, .5f);
        currentMenu = pauseLocation.Options;
        //Debug.Log("Open Options");
    }

    public void OpenControls()
    {
        if (transitioning) { return; }
        transitioning = true;
        StartCoroutine(delayTransition());

        Debug.Log("OPTIONS CONTROL");
        GamemodeSelectScreenManager.Instance.InterpolateScreens(mainMenuPanel, controlsPanel, GamemodeSelectScreenManager.Direction.Left);
        //ControllerForMenus.Instance.ChangeObjectSelectedWithDelay(null, .5f);
        ControllerForMenus.Instance.ChangeSelectedObject(null);

        ControlScheme.Instance.OpenControls();
        currentMenu = pauseLocation.Controls;
        //Debug.Log("Open Controls");
    }

    private IEnumerator delayTransition()
    {

        yield return new WaitForSeconds(1);
        transitioning = false;
    }

    public void OpenAchievements()
    {
        Debug.Log("Open Achievements");
    }

    private bool navLock = false;
    public void Nav(Vector2 v2)
    {
        if (currentMenu == pauseLocation.Controls)
        {
            if (v2.x > .5f)
            {
                ControlScheme.Instance.NavRight();
            }
            else if (v2.x < -.5f)
            {
                ControlScheme.Instance.NavLeft();
            }
        }
    }

    public void returnToMainMenuFromOptions()
    {
        currentMenu = pauseLocation.Pause;
        GamemodeSelectScreenManager.Instance.InterpolateScreens(optionsPanel, mainMenuPanel, GamemodeSelectScreenManager.Direction.Right);
        ControllerForMenus.Instance.ChangeObjectSelectedWithDelay(_optionsButton.gameObject, .4f);

        ControlScheme.Instance.CloseControls();
    }

    public void returnToMainMenuFromControls()
    {
        currentMenu = pauseLocation.Pause;
        GamemodeSelectScreenManager.Instance.InterpolateScreens(controlsPanel, mainMenuPanel, GamemodeSelectScreenManager.Direction.Right);
        ControllerForMenus.Instance.ChangeObjectSelectedWithDelay(_controlsButton.gameObject, .4f);
    }

    // IMoveHandler Implementation
    #region OptionsMethods
    public void ExitGame()
    {
        //Debug.Log("Exit Game");
        Application.Quit();
    }
    public void FullScreen()
    {
        Screen.fullScreen = true;
    }
    public void Windowed()
    {
        Screen.fullScreen = false;
    }
    public void ChangeTargetFPS(int index)
    {
        switch (index)
        {
            case 0:
                Application.targetFrameRate = 120;
                break;
            case 1:
                Application.targetFrameRate = 90;
                break;
            case 2:
                Application.targetFrameRate = 60;
                break;
            case 3:
                Application.targetFrameRate = 30;
                break;
            case 4:
                Application.targetFrameRate = 300;
                break;
        }

    }

    private void changeGeneralVolume(float value)
    {
        if (OptionsManager.Instance != null)
        {
            _generalVolumeText.text = (value + 10).ToString();
            OptionsManager.Instance.SetGeneralVolume(value);
        }
    }
    private void changeMusicVolume(float value)
    {
        if (OptionsManager.Instance != null)
        {
            _musicVolumeText.text = (value + 10).ToString();
            OptionsManager.Instance.SetMusicVolume(value);
        }
    }
    private void changeSFXVolume(float value)
    {
        if (OptionsManager.Instance != null)
        {
            _SFXVolumeText.text = (value + 10).ToString();
            OptionsManager.Instance.SetSFXVolume(value);
        }
    }
    private void changeTargetFPS(float value)
    {
        int newValue = (int)value;
        _TargetFPSText.text = "Limit FPS:               " + newValue;
        if (OptionsManager.Instance != null)
        {
            OptionsManager.Instance.SetTargetFPS(newValue);
        }
    }
    #endregion

    public enum Menu
    {
        Main,
        Options,
        Controls,
        Achievements,
        Credits
    }

    private enum pauseLocation
    {
        Pause,
        Options,
        Controls,
        None
    }
}
