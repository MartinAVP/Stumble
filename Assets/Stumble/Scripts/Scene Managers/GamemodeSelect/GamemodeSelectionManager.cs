using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GamemodeSelectionManager : MonoBehaviour
{
    private bool transitioning;

    public static GamemodeSelectionManager Instance { get; private set; }

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

    public void GoBackToLobby(PlayerInput input)
    {
        // Check Permission
        if (!PlayerDataHolder.Instance.GetPlayerData(input).isHost) return;

        if (transitioning) return;
        transitioning = true;

        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        StartCoroutine(delayReturn());
    }

    private IEnumerator delayReturn()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Lobby");
    }
}
