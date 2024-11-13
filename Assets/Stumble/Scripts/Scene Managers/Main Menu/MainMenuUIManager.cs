using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenuUIManager : MonoBehaviour, IUpdateSelectedHandler
{
    [Header("Panels")]
    [Tooltip("Panel For Fading")]
    [SerializeField] private GameObject unityScreenPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject startScreenPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject transitionPanel;

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
    [SerializeField] private UnityEngine.UI.Slider _TargetFPS;
    [SerializeField] private TextMeshProUGUI _TargetFPSText;
    [SerializeField] private UnityEngine.UI.Button _ReturnToMenuFromOptions;

    private PlayerInputManager playerInputManager;
    [SerializeField] private MultiplayerEventSystem multiplayerEventSystem;

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
        _startGameButton?.onClick.AddListener(StartGameCoroutine);
        _optionsButton?.onClick.AddListener(OpenOptions);
        _creditsButton?.onClick.AddListener(OpenCredits);
        _achievementsButton?.onClick.AddListener(OpenAchievements);
        _ExitButton?.onClick.AddListener(ExitGame);

        _generalVolume.onValueChanged.AddListener(changeGeneralVolume);
        _MusicVolume.onValueChanged.AddListener(changeMusicVolume);
        _SFXVolume.onValueChanged.AddListener(changeSFXVolume);
        _TargetFPS.onValueChanged.AddListener(changeTargetFPS);
        _ReturnToMenuFromOptions.onClick.AddListener(returnToMainMenuFromOptions);
    }

    private void OnDisable()
    {
        if (menuManagerFound) {        
            //playerInputManager.onPlayerJoined -= joinHostPlayer;

            // Buttons
            _startGameButton?.onClick.RemoveAllListeners();
            _optionsButton?.onClick.RemoveAllListeners();
            _creditsButton?.onClick.RemoveAllListeners();
            _achievementsButton?.onClick.RemoveAllListeners();
            _ExitButton?.onClick.RemoveAllListeners();

            _generalVolume.onValueChanged.RemoveAllListeners();
            _MusicVolume.onValueChanged.RemoveAllListeners();
            _SFXVolume.onValueChanged.RemoveAllListeners();
            _TargetFPS.onValueChanged.RemoveAllListeners();
            _ReturnToMenuFromOptions.onClick.RemoveAllListeners();
        }
    }

    private void InitializeManager()
    {
        // Initialize Panels
        unityScreenPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        startScreenPanel.SetActive(true);

        // Disable the Transition
        //StartCoroutine(disableUnityTransition());


        changeGeneralVolume(0);
        changeMusicVolume(0);
        //changeSFXVolume(0);
        changeTargetFPS(120);

        initialized = true;

        Debug.Log("My Pony");
    }

    private void Start()
    {
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(false); }
    }

    /*    private void joinHostPlayer(PlayerInput player)
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

        private IEnumerator disableUnityTransition()S
        {
            yield return new WaitForSeconds(3f);
            unityScreenPanel.SetActive(false);
        }*/

    public void StartGameCoroutine() {
        Debug.Log("Clicked");
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        //Debug.Log("Start Game");
        /*        SceneManager.LoadScene("Lobby", LoadSceneMode.Additive);
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                GamemodeSelectScreenManager.Instance.InterpolateScreens(mainMenuPanel, transitionPanel, GamemodeSelectScreenManager.Direction.Left);
                yield return new WaitForSeconds(.5f);
                transitionPanel.SetActive(false);*/
        //SceneManager.UnloadSceneAsync("MainMenu");
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Lobby");
    }

    public void OpenOptions()
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

    public void OnUpdateSelected(BaseEventData data)
    {
        Debug.Log("OnUpdateSelected called.");
        Debug.Log(data.selectedObject.name);
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

    private void changeTargetFPS(float value)
    {
        int newValue = (int)value;
        _TargetFPSText.text = "Limit FPS:               " + newValue;
        if (OptionsManager.Instance != null)
        {
            OptionsManager.Instance.SetTargetFPS(newValue);
        }
    }
}
