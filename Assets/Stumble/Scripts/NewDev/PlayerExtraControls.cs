using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class PlayerExtraControls : MonoBehaviour
{

    private GameController gameController;

    private LobbyManager lobbyManager;

    private PlayerInput input;

    private void Awake()
    {
        Task setup = Setup();
    }

    private void Start()
    {
        input = this.GetComponent<PlayerInput>();
    }

    private async Task Setup()
    {
        while (GameController.Instance == null || GameController.Instance.enabled == false)
        {
            await Task.Delay(1);
        }

        gameController = GameController.Instance;

        if(gameController.gameState == GameState.Lobby)
        {
            while (LobbyManager.Instance == null || LobbyManager.Instance.enabled == false)
            {
                await Task.Delay(1);
            }

            lobbyManager = LobbyManager.Instance;
        }
    }

    public void Continue(InputAction.CallbackContext context)
    {
        // Send a Message
        if (context.started)
        {
            if(gameController.gameState == GameState.Lobby)
            {
                lobbyManager.StartGame(input);
            }
        }
    }

    public void Return(InputAction.CallbackContext context)
    {
        // Send a Message.
        if(!context.started)
        {
            if (gameController.gameState == GameState.Lobby)
            {
                lobbyManager.ReturnToMainMenu(input);
            }
        }
    }
}
