using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GamemodeSelectionUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject GamemodeSelectPanel;
    [SerializeField] private GameObject GrandPrixLevelSelectionPanel;

    [SerializeField] private GameObject minigameModeDescription;
    [SerializeField] private GameObject partyModeDescription;

    [Header("Buttons")]
    [SerializeField] private Button GrandprixButton;
    [SerializeField] private Button ArenaButton;
    [SerializeField] private Button SelectModes;
    [Space]
    [SerializeField] private Button Level1Button;

    [Header("Video Clips")]
    [SerializeField] private VideoClip grandPrixVideo;
    [SerializeField] private VideoClip arenaVideo;
    [SerializeField] private VideoPlayer backgroundVideo;

    private bool transitioning;
    private GamemdoeScreen currentScreen;

    public static GamemodeSelectionUIManager Instance { get; private set; }
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
    }

    private void OnEnable()
    {
        GrandprixButton.GetComponent<ButtonHoverEvent>().onHover.AddListener(HoverOverGrandPrix);
        ArenaButton.GetComponent<ButtonHoverEvent>().onHover.AddListener(HoverOverArena);

        //GrandprixButton.onClick.AddListener(SelectGrandPrix);
        ArenaButton.onClick.AddListener(SelectArena);
        SelectModes.onClick.AddListener(GoBackToModeSelection);
    }

    void Start()
    {
        LoadingScreenManager.Instance.StartTransition(false);

        HoverOverGrandPrix();
    }

    private void HoverOverGrandPrix()
    {
        backgroundVideo.clip = grandPrixVideo;
        backgroundVideo.Play();

        partyModeDescription?.SetActive(true);
        minigameModeDescription?.SetActive(false);
    }

    private void HoverOverArena()
    {
        backgroundVideo.clip = arenaVideo;
        backgroundVideo.Play();

        partyModeDescription?.SetActive(false);
        minigameModeDescription?.SetActive(true);
    }

    private void SelectArena()
    {
        if (transitioning)
        {
            return;
        }
        else
        {
            StartCoroutine(clickDelay());
            transitioning = true;
        }

        currentScreen = GamemdoeScreen.Minigame;

        //SceneManager.LoadScene("ArenaScene");
        GamemodeSelectScreenManager.Instance.InterpolateScreens(GamemodeSelectPanel, GrandPrixLevelSelectionPanel, GamemodeSelectScreenManager.Direction.Left);
        ControllerForMenus.Instance.ChangeSelectedObject(ModularGamemodeDisplay.Instance?.cards[0]);
    }

    private void GoBackToModeSelection()
    {
        if(transitioning)
        {
            return;
        }

        GamemodeSelectScreenManager.Instance.InterpolateScreens(GrandPrixLevelSelectionPanel, GamemodeSelectPanel, GamemodeSelectScreenManager.Direction.Right);
        ControllerForMenus.Instance.ChangeSelectedObject(ArenaButton.gameObject);
    }

    public void LoadGrandPrixLevel(string name)
    {
        LoadingScreenManager.Instance.StartTransition(true);
        StartCoroutine(loadLevelDelay(name));
        
        if(MenuMusicController.Instance != null)
        {
            MenuMusicController.Instance.EndMusic(2.8f);
        }
    }

    private IEnumerator loadLevelDelay(string name)
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(name);
    }

    private enum GamemodeSelected
    {
        GrandPrix,
        Arena,
    }

    private IEnumerator clickDelay()
    {
        yield return new WaitForSeconds(1f);
        transitioning = false;
    }




    public void Return(PlayerInput input)
    {
        // Check Permission
        if (!PlayerDataHolder.Instance.GetPlayerData(input).isHost) return;

        if (transitioning) return;

        Debug.Log("Current Screen: Main");
        switch (currentScreen)
        {
            case GamemdoeScreen.Main:
                Debug.Log("Current Screen: Main");
                if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }

                transitioning = true;

                StartCoroutine(delayReturn());
                break;
            case GamemdoeScreen.Minigame:
                Debug.Log("Current Screen: Minigame");

                GoBackToModeSelection();
                currentScreen = GamemdoeScreen.Main;
                break;
        }
/*        if (currentScreen == GamemdoeScreen.Main)
        {
            if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
            StartCoroutine(delayReturn());
        }*/

    }

    private IEnumerator delayReturn()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Lobby");
    }

    public enum GamemdoeScreen
    {
        Main,
        Party,
        Minigame
    }
}
