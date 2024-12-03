using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputOverhaul : MonoBehaviour
{
    public UnityEvent OnSouthPressed;

    private GameController gameController;

    private MainMenuUIManager mainMenuUIManager;

    private Coroutine holdCoroutine;

    private void Start()
    {
        gameController = GameController.Instance;
        if (gameController.gameState == GameState.MainMenu)
        {
            mainMenuUIManager = MainMenuUIManager.Instance;
        }
    }

    public void SouthButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Press Send");
            OnSouthPressed.Invoke();
        }
    }

    public void NavigateOverhaul(InputAction.CallbackContext context)
    {
        Vector2 nav = context.ReadValue<Vector2>();
        if(context.performed)
        {
            if(gameController.gameState == GameState.MainMenu)
            {
                mainMenuUIManager.Nav(nav);
            }
        }
    }

    public void LeaveChecker(InputAction.CallbackContext context)
    {
        // Check if the input has just started
        if (context.started)
        {
            // Start the coroutine to track how long the input is held
            if (holdCoroutine == null)
            {
                holdCoroutine = StartCoroutine(HoldDurationCheck());
            }
        }

        // Check if the input has been performed (released or completed)
        if (context.canceled)
        {
            // Stop the coroutine if the input is canceled
            if (holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                holdCoroutine = null;
            }
        }
    }

    // Coroutine that tracks the input hold duration
    private IEnumerator HoldDurationCheck()
    {
        yield return new WaitForSeconds(3f);

        // Complete
        Debug.Log("Input has been held for more than 3 seconds!");
        if(gameController.gameState == GameState.Lobby) {
            LobbyManager.Instance.RemovePlayer(this.GetComponent<PlayerInput>());
        }
    }
}
