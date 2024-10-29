using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameSceneIdentifier : MonoBehaviour
{
    [SerializeField] private GameState gameScene = GameState.MainMenu;
    private GameController gameController;

    private void Start()
    {
        Task Setup = setup();
    }

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (GameController.Instance == null || GameController.Instance.enabled == false)
        {
            // Await 5 ms and try finding it again.
            // It is made 5 seconds because it is
            // a core gameplay mechanic.
            await Task.Delay(5);
        }

        // Once it finds it initialize the scene
        InitializeScene();
        return;
    }

    private void InitializeScene()
    {
        gameController = GameController.Instance;
        if (gameController != null)
        {
            // Set the Game Scene Type and Start
            // Chain events. More notes on the Game Controller
            gameController.SetGameState(gameScene);
            Debug.Log("The Game Scene has been identified correctly as " + gameScene.ToString() + ". Initializing Systems...        [Game Scene Identifier]");
        }
    }
}
