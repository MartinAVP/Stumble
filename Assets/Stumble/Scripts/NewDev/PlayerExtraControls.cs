using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using UnityEngine.Events;

public class PlayerExtraControls : MonoBehaviour
{

    private GameController gameController;
    private LobbyManager lobbyManager;
    private MainMenuManager mainMenuManager;
    private PlayerInput input;

    [HideInInspector] public UnityEvent<PlayerInput> pressContinue;
    [HideInInspector] public UnityEvent<PlayerInput> pressBack;

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

        if (gameController.gameState == GameState.MainMenu)
        {
            while (MainMenuManager.Instance == null || MainMenuManager.Instance.enabled == false)
            {
                await Task.Delay(1);
            }

            mainMenuManager = MainMenuManager.Instance;
        }
    }

    public void Continue(InputAction.CallbackContext context)
    {
        // Send a Message
        if (context.started)
        {
            if(gameController.gameState == GameState.Lobby)
            {
                Debug.Log("Pressing Enter or Start");
                lobbyManager.StartGame(input);
            }
            else if(gameController.gameState == GameState.MainMenu)
            {
                mainMenuManager.StartGame(input);
            }
            pressContinue.Invoke(input);
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
            else if (gameController.gameState == GameState.MainMenu)
            {
                mainMenuManager.ExitGame(input);
            }
            pressBack.Invoke(input);
        }
    }
}
