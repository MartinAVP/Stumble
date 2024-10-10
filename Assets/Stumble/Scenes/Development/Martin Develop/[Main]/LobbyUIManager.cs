using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private Button startGame;

    // Start is called before the first frame update
    void Start()
    {
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(false); }
    }

    private void OnEnable()
    {
        startGame.onClick.AddListener(StartGame);
    }

    private void OnDisable()
    {
        startGame.onClick.RemoveAllListeners();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GamemodeSelect");
    }
}
