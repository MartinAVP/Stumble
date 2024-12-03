using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject emptyPanel;

    [Header("Main Buttons")]
    [SerializeField] private UnityEngine.UI.Button _continueGameButton;
    [SerializeField] private UnityEngine.UI.Button _optionsButton;
    //[SerializeField] private UnityEngine.UI.Button _creditsButton;
    //[SerializeField] private UnityEngine.UI.Button _achievementsButton;
    //[SerializeField] private UnityEngine.UI.Button _ExitButton;

    [Header("Options")]
    [SerializeField] private UnityEngine.UI.Slider _generalVolume;
    [SerializeField] private UnityEngine.UI.Slider _MusicVolume;
    [SerializeField] private UnityEngine.UI.Slider _SFXVolume;
/*    [SerializeField] private UnityEngine.UI.Slider _TargetFPS;
    [SerializeField] private TextMeshProUGUI _TargetFPSText;*/
    [Space]
    public UnityEngine.UI.Button _ReturnToMenuFromOptions;

    private bool isDisplayed = false;
    private bool transitioning = false;

    private pauseLocation loc = pauseLocation.None;

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
        pausePanel.SetActive(false);
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
        if (loc == pauseLocation.Options) return;

        if(isDisplayed)
        {
            isDisplayed = false;
            // Turn Off
            pausePanel.SetActive(false);
            optionsPanel.SetActive(false);
            if(hostIsController) { ControllerForMenus.Instance.ChangeSelectedObject(null); }

            Time.timeScale = 1f;
            loc = pauseLocation.None;
        }
        else
        {
            isDisplayed = true;
            // Turn On
            pausePanel.SetActive(true);
            optionsPanel.SetActive(true);

            if(hostIsController) { ControllerForMenus.Instance?.ChangeObjectSelectedWithDelay(_continueGameButton.gameObject, .5f); }
            
            Time.timeScale = 0f;
            loc = pauseLocation.Pause;
        }
    }

    public void TogglePauseMenuFromMenu()
    {
        if (isDisplayed)
        {
            isDisplayed = false;
            // Turn Off
            pausePanel.SetActive(false);
            optionsPanel.SetActive(false);

            Time.timeScale = 1f;
            loc = pauseLocation.None;
        }
        else
        {
            isDisplayed = true;
            // Turn On
            pausePanel.SetActive(true);
            optionsPanel.SetActive(true);

            Time.timeScale = 0f;
            loc = pauseLocation.Pause;
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
        GamemodeSelectScreenManager.Instance.InterpolateScreens(pausePanel, optionsPanel, GamemodeSelectScreenManager.Direction.Left);
        ControllerForMenus.Instance.ChangeObjectSelectedWithDelay(_ReturnToMenuFromOptions.gameObject, .5f);
        loc = pauseLocation.Options;
        Debug.Log("Open Options");
    }

    public void OpenCredits()
    {
        Debug.Log("Open Credits");
    }

    public void OpenAchievements()
    {
        Debug.Log("Open Achievements");
    }

    public void ExitGame()
    {
        //Debug.Log("Exit Game");
        Application.Quit();
    }

    public void GoToGamemodeSelect()
    {
        if (transitioning) return;
        transitioning = true;

        Time.timeScale = 1f;

        Destroy(ModularController.Instance.gameObject);

        StartCoroutine(sceneLoadDelay("GamemodeSelect"));
    }

    public void GoToMainMenu()
    {
        if (transitioning) return;
        transitioning = true;

        Time.timeScale = 1f;
        PlayerDataHolder.Instance.ClearAllButHost(true);

        Destroy(ModularController.Instance.gameObject);

        StartCoroutine(sceneLoadDelay("Menu"));
    }

    //====================================
    // Add a Roll for When going back to the Main Menu.
    // Remove the Library from DontDestroyOnLoad

    private IEnumerator sceneLoadDelay(string name)
    {
        yield return new WaitForSeconds(1f);
        if(Time.timeScale != 1) { Time.timeScale = 1; } // Prevent Freeze Across Scenes
        SceneManager.LoadScene(name);
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

    public void returnToPauseMenuFromOptions()
    {
        GamemodeSelectScreenManager.Instance.InterpolateScreens(optionsPanel, pausePanel, GamemodeSelectScreenManager.Direction.Right);
        ControllerForMenus.Instance.ChangeObjectSelectedWithDelay(_optionsButton.gameObject, .4f);
        loc = pauseLocation.Pause;
    }

    // IMoveHandler Implementation
    /*    public GameObject currentSelected;
        [HideInInspector] public UnityEvent<GameObject> OnChangedSelectedObject;
        private bool subToSlider = false;
        public Slider activeSlide;*/

    /*    private async Task ChangeCurrentSelected()
        {
            currentSelected = this.gameObject;
            while (true)
            {
                if (currentSelected != multiplayerEventSystem.currentSelectedGameObject)
                {
                    currentSelected = multiplayerEventSystem.currentSelectedGameObject;
                    OnChangedSelectedObject.Invoke(currentSelected);
                    CheckSlider();
                    Debug.Log($"Selected GameObject changed to: {currentSelected.name}");
                    await Task.Delay(100);
                }

                Debug.Log("Loopin");
                await Task.Delay(200);
            }

        }*/

    /*    private void FixedUpdate()
        {
            //if(currentSelected == null) { currentSelected = gameObject; }
            if (currentSelected == null) { currentSelected = multiplayerEventSystem.currentSelectedGameObject; return; }
            if (currentSelected != multiplayerEventSystem.currentSelectedGameObject)
            {
                currentSelected = multiplayerEventSystem.currentSelectedGameObject;
                OnChangedSelectedObject.Invoke(currentSelected);
                CheckSlider();
                //Debug.Log($"Selected GameObject changed to: {currentSelected.name}");
            }
        }

        private void CheckSlider()
        {
            // Prevent this Method from running if the player is not using a controller.
            PlayerInput input = PlayerDataHolder.Instance.GetPlayerData(0).input;
            if (input.currentControlScheme != "Controller") { return; }
            Debug.Log("Check #1");
            GameObject obj = multiplayerEventSystem.currentSelectedGameObject;
        }

        public void SliderSubscribe()
        {

        }

        public void Slider(Vector2 raw, PlayerInput data)
        {
            Debug.Log("Slider Performed");
            if(activeSlide == null) { return; }
            if (raw.x > .5f)
            {
                Debug.Log("Slider++");
                if (multiplayerEventSystem.currentSelectedGameObject.GetComponent<UnityEngine.UI.Slider>() != null)
                {
                    multiplayerEventSystem.currentSelectedGameObject.GetComponent<UnityEngine.UI.Slider>().value += raw.x * Time.deltaTime;
                }
            }
        }*/


    private void changeGeneralVolume(float value)
    {
        if (OptionsManager.Instance != null)
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
/*        int newValue = (int)value;
        _TargetFPSText.text = "Limit FPS:               " + newValue;
        if (OptionsManager.Instance != null)
        {
            OptionsManager.Instance.SetTargetFPS(newValue);
        }*/
    }

    private enum pauseLocation
    {
        Pause,
        Options,
        None
    }
}
