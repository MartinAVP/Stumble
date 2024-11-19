using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    // Michael Broke Github
    private PlayerInputManager _playerInputManager;
    [SerializeField] private Animator _animator;

    private void OnEnable()
    {
        _playerInputManager = FindFirstObjectByType<PlayerInputManager>();

        _playerInputManager.onPlayerJoined += joinHostPlayer;
    }

    private void Start()
    {
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(false); }
        _animator.SetBool("start", true);
    }

    private void OnDisable()
    {
        _playerInputManager.onPlayerLeft -= joinHostPlayer;
    }

    private void joinHostPlayer(PlayerInput player)
    {
        StartCoroutine(changeToMainMenu());
    }

    private IEnumerator changeToMainMenu()
    {
        Debug.Log("IM HERE");
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("MainMenu");
    }
}
