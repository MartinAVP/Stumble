using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(PodiumManager))]
public class FeedbackAddon : PodiumManager
{
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private Button feedbackButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        feedbackButton.onClick.AddListener(openFeedbackForm);
        exitButton.onClick.AddListener(BackToGame);
    }

    private void OnDisable()
    {
        feedbackButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (ExperimentalPlayerManager.Instance == null || ExperimentalPlayerManager.Instance.enabled == false || ExperimentalPlayerManager.Instance.finishedSystemInitializing == false)
        {
            // Await 2 ms and try finding it again.
            // It is made 2 seconds because it is
            // a core gameplay mechanic.
            await Task.Delay(1);
        }

        // Once it finds it initialize the scene
        UnityEngine.Debug.Log("Initializing Podium Manager...         [Podium Manager]");
        //GameController.Instance.startSystems += LateStart;

        InitializeManager();
        //initialized = true;
        return;
    }

    private void Start()
    {
        feedbackPanel.SetActive(false);
    }

    private void InitializeManager()
    {
        //Debug.Log("I've been executed");
        StartCountdown();
    }

    public override void StartCountdown()
    {
        StartCoroutine(returnToMenuCooldown());
    }

    private IEnumerator returnToMenuCooldown()
    {
        yield return new WaitForSeconds(.8f);
        if (CinematicController.Instance != null)
        {
            CinematicController.Instance.StartTimeline();
        }
        yield return new WaitForSeconds(20f);
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        yield return new WaitForSeconds(2f);
        feedbackPanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void openFeedbackForm(){
        Application.OpenURL("https://www.youtube.com/watch?v=f_e3NbtwKAo");
    }
    private void BackToGame(){
        SceneManager.LoadScene("GamemodeSelect");
    }
}
