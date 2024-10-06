using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;
    public event Action onLoadScreenFinishTransition;
    public GameObject LoadingScreen;

    // Set the target Y position for the Loading Screen
    public float targetYPosition = 300f; // Change this to the desired Y position
    private RectTransform loadingScreenRectTransform;

    private void Awake()
    {
        DefineAsSingleton();
        loadingScreenRectTransform = LoadingScreen.GetComponent<RectTransform>();
    }

    private void DefineAsSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void StartTransition()
    {
        if (LoadingScreen == null) { Debug.LogError("No Loading Screen Defined"); return; }
        LoadingScreen.SetActive(true);
        StartCoroutine(TransitionToLoadingScreen());
    }

    public void EndTransition()
    {
        onLoadScreenFinishTransition?.Invoke();
        /*        if (LoadingScreen == null) { Debug.LogError("No Loading Screen Defined"); return; }
                //StartCoroutine(TransitionFromLoadingScreen());*/
    }

    private IEnumerator TransitionToLoadingScreen()
    {
        float startY = loadingScreenRectTransform.anchoredPosition.y;
        float targetY = (Mathf.Approximately(startY, targetYPosition)) ? 0 : targetYPosition;
        float duration = 1f; // Duration of the transition
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float newY = Mathf.SmoothStep(startY, targetY, t);
            loadingScreenRectTransform.anchoredPosition = new Vector2(loadingScreenRectTransform.anchoredPosition.x, newY);
            yield return null;
        }

        loadingScreenRectTransform.anchoredPosition = new Vector2(loadingScreenRectTransform.anchoredPosition.x, targetY);
        EndTransition();
    }

    /*    private void OnGUI()
        {
            // Set the button's position and size
            Rect buttonRect = new Rect(100, 100, 200, 50);

            // Create a button
            if (GUI.Button(buttonRect, "Click Me"))
            {
                ButtonClicked();
            }
        }

        private void ButtonClicked()
        {
            StartTransition();
            Debug.Log("Button clicked!");
        }
    */
}
