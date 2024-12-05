using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Button = UnityEngine.UI.Button;

public class MainMenuUIManager : MonoBehaviour
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
    [SerializeField] private UnityEngine.UI.Button _ReturnToMenuFromOptions;

    [Header("Controls")]
    [SerializeField] private Button _leftArrowControls;
    [SerializeField] private Button _rightArrowControls;
    [SerializeField] private TextMeshProUGUI _controlScheme;

    private PlayerInputManager playerInputManager;
    //[SerializeField] private MultiplayerEventSystem multiplayerEventSystem;

    private bool transfering = false;           // Transferring prevent Spamming.

    public Menu currentMenu = Menu.Main;

    public static MainMenuUIManager Instance { get; private set; }
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
        while (MainMenuManager.Instance == null || MainMenuManager.Instance.enabled == false || MainMenuManager.Instance.initializedFinished == false)
        {
            // Await 5 ms and try finding it again.
            // It is made 5 seconds because it is
            // a core gameplay mechanic.
            await Task.Delay(2);
        }

        // Once it finds it initialize the scene
        Debug.Log("Initializing Main Menu UI Manager...         [Main Menu UI Manager]");

        //playerInputManager = FindAnyObjectByType<PlayerInputManager>();
        menuManagerFound = true;

        InitializeManagerSubs();
        InitializeManager();
        return;
    }

    private void InitializeManagerSubs()
    {
        //playerInputManager.onPlayerJoined += joinHostPlayer;

        // Buttons
/*        _startGameButton?.onClick.AddListener(StartGameCoroutine);
        _optionsButton?.onClick.AddListener(OpenOptions);
        _creditsButton?.onClick.AddListener(OpenCredits);
        _achievementsButton?.onClick.AddListener(OpenAchievements);
        _ExitButton?.onClick.AddListener(ExitGame);*/

        _generalVolume?.onValueChanged.AddListener(changeGeneralVolume);
        _MusicVolume?.onValueChanged.AddListener(changeMusicVolume);
        _SFXVolume?.onValueChanged.AddListener(changeSFXVolume);
        _TargetFPS?.onValueChanged.AddListener(changeTargetFPS);
        //_ReturnToMenuFromOptions.onClick.AddListener(returnToMainMenuFromOptions);
    }

    private void OnDisable()
    {
        if (menuManagerFound) {        
            //playerInputManager.onPlayerJoined -= joinHostPlayer;

            // Buttons
/*            _startGameButton?.onClick.RemoveAllListeners();
            _optionsButton?.onClick.RemoveAllListeners();
            _creditsButton?.onClick.RemoveAllListeners();
            _achievementsButton?.onClick.RemoveAllListeners();
            _ExitButton?.onClick.RemoveAllListeners();*/

            _generalVolume.onValueChanged.RemoveAllListeners();
            _MusicVolume.onValueChanged.RemoveAllListeners();
            _SFXVolume.onValueChanged.RemoveAllListeners();
            _TargetFPS?.onValueChanged.RemoveAllListeners();
            //_ReturnToMenuFromOptions.onClick.RemoveAllListeners();
        }
    }

    private void InitializeManager()
    {
        // Initialize Panels
        //unityScreenPanel.SetActive(true);
        mainMenuPanel.SetActive(true);
        //startScreenPanel.SetActive(true);

        // Disable the Transition
        //StartCoroutine(disableUnityTransition());


        changeGeneralVolume(0);
        changeMusicVolume(0);
        //changeSFXVolume(0);
        changeTargetFPS(120);

        PlayerInput input = PlayerDataHolder.Instance.GetPlayerData(0).input;
        //input.gameObject.GetComponent<PlayerSelectAddon>().OnSelectInput.AddListener(Slider);

        //Task task = ChangeCurrentSelected();
        initialized = true;
    }

    private void Start()
    {
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(false); }

        Debug.LogWarning("Selection Started");

        // Adding Slider Sub
        //InputActionMap inputActionMap = input.
        //if (input.currentControlScheme == "Controller") { return; }
/*        input.actions["Move"].performed += Slider;
        input.actions["Select"].performed += Slider;*/

    }

    public void StartGameCoroutine() {
        if(PlayerDataHolder.Instance == null)
        {
            Debug.LogWarning("There is no player data holder, can't start the game");
            return;
        } // Check if there is a player Data Holder
        else
        {
            if(PlayerDataHolder.Instance.players.Count == 0) { Debug.LogWarning("There is no player in the menu, can't start game"); return; }
        } // Check if there is a player to start
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
        GamemodeSelectScreenManager.Instance.InterpolateScreens(mainMenuPanel, optionsPanel, GamemodeSelectScreenManager.Direction.Left);
        ControllerForMenus.Instance.ChangeObjectSelectedWithDelay(_ReturnToMenuFromOptions.gameObject, .5f);
        currentMenu = Menu.Options;
        Debug.Log("Open Options");
    }

    public void OpenControls()
    {
        GamemodeSelectScreenManager.Instance.InterpolateScreens(mainMenuPanel, controlsPanel, GamemodeSelectScreenManager.Direction.Left);
        //ControllerForMenus.Instance.ChangeObjectSelectedWithDelay(null, .5f);
        ControllerForMenus.Instance.ChangeSelectedObject(null);

        ControlScheme.Instance.OpenControls();
        currentMenu = Menu.Controls;
        Debug.Log("Open Controls");
    }

    public void OpenCredits()
    {
        Debug.Log("Open Credits");
    }

    public void OpenAchievements()
    {
        Debug.Log("Open Achievements");
    }

    private bool navLock = false;
    public void Nav(Vector2 v2)
    {
        if(currentMenu == Menu.Controls)
        {
            if(v2.x > .5f)
            {
                ControlScheme.Instance.NavRight();
            }
            else if(v2.x < -.5f)
            {
                ControlScheme.Instance.NavLeft();
            }
        }
    }

    public void returnToMainMenuFromOptions()
    {
        currentMenu = Menu.Main;
        GamemodeSelectScreenManager.Instance.InterpolateScreens(optionsPanel, mainMenuPanel, GamemodeSelectScreenManager.Direction.Right);
        ControllerForMenus.Instance.ChangeObjectSelectedWithDelay(_optionsButton.gameObject, .4f);

        ControlScheme.Instance.CloseControls();
    }

    public void returnToMainMenuFromControls()
    {
        currentMenu = Menu.Main;
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

/*    public enum ControlScheme
    {
        Keyboard,
        Controller
    }*/
}
