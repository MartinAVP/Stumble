using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GamemodeSelectionUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject GamemodeSelectPanel;
    [SerializeField] private GameObject GrandPrixLevelSelectionPanel;

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

    private void OnEnable()
    {
        GrandprixButton.GetComponent<ButtonHoverEvent>().onHover.AddListener(HoverOverGrandPrix);
        ArenaButton.GetComponent<ButtonHoverEvent>().onHover.AddListener(HoverOverArena);

        GrandprixButton.onClick.AddListener(SelectGrandPrix);
        ArenaButton.onClick.AddListener(SelectArena);
        SelectModes.onClick.AddListener(GoBackToModeSelection);
    }


    private GamemodeSelected currentlySelected;

    void Start()
    {
        LoadingScreenManager.Instance.StartTransition(false);
    }

    private void HoverOverGrandPrix()
    {
        backgroundVideo.clip = grandPrixVideo;
        backgroundVideo.Play();
    }

    private void HoverOverArena()
    {
        backgroundVideo.clip = arenaVideo;
        backgroundVideo.Play();
    }

    private void SelectGrandPrix()
    {
        GamemodeSelectScreenManager.Instance.InterpolateScreens(GamemodeSelectPanel, GrandPrixLevelSelectionPanel, GamemodeSelectScreenManager.Direction.Left);
        ControllerForMenus.Instance.ChangeSelectedObject(Level1Button.gameObject);
    }

    private void SelectArena()
    {

    }

    private void GoBackToModeSelection()
    {
        GamemodeSelectScreenManager.Instance.InterpolateScreens(GrandPrixLevelSelectionPanel, GamemodeSelectPanel, GamemodeSelectScreenManager.Direction.Right);
        ControllerForMenus.Instance.ChangeSelectedObject(GrandprixButton.gameObject);
    }

    public void LoadGrandPrixLevel(string name)
    {
        LoadingScreenManager.Instance.StartTransition(true);
        StartCoroutine(loadLevelDelay(name));
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
}
