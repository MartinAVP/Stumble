using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneIdentifier : MonoBehaviour
{
    [SerializeField] private GameState gameScene = GameState.MainMenu;
    private GameController gameController;
    private void Start()
    {
        gameController = GameController.Instance;
        if( gameController != null)
        {
            gameController.SetGameState(gameScene);
            //Debug.Log("Scene Identifier Attached Correctly");
        }
        else
        {
            //Debug.LogError("There is no GameController in the current scene.");
        }
    }
}
