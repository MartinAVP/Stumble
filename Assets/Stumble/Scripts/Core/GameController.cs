using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    public GameState gameState { get; private set; }
    public GameState viewer;

    //public event Action startSystems;
    //public event Action startSecondarySystems;

    [HideInInspector] public bool initialized = false;

    // Singleton
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetGameState(GameState state){
        gameState = state;
        viewer = state;

        Debug.Log("================= " + SceneManager.GetActiveScene().name + " =================");

        // Initialize Systems after the Game State is Inherited from the Scene
        Debug.Log("Start Systems initializing... [Game Controller]");
        //StartCoroutine(StartSytems());

        StartCoroutine(framwaiter());
    }

    private IEnumerator framwaiter()
    {
        yield return new WaitForEndOfFrame();
        initialized = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        initialized = false;
        //Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        //Debug.Log($"Scene {scene.name} loaded with mode {mode}");

        // You can add additional logic here for when a scene is switched
    }

    /*    public IEnumerator StartSytems()
        {
            yield return new WaitForEndOfFrame();
            startSystems?.Invoke();
            yield return new WaitForEndOfFrame();
            startSecondarySystems?.Invoke();
        }*/
}
