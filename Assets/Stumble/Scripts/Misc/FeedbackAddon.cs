using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private void Start()
    {
        feedbackPanel.SetActive(false);
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
