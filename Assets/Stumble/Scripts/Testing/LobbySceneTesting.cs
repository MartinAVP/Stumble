using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbySceneTesting : MonoBehaviour
{

    public void Load(string name)
    {

        // Prevent Data Clear when changing scene
        PlayerDataManager.Instance.isLobby = false;
        //DontDestroyOnLoad (Camera.main.gameObject);
        //Camera.main.enabled = false;
/*
        List<PlayerData> list = PlayerDataManager.Instance.GetPlayers();

        for (int i = 0; i < list.Count; i++)
        {
            GameObject tempHolder = new GameObject("Input Holder " + i);
            tempHolder.AddComponent<PlayerInput>();
            MovePlayerInput(list[i].GetInput(), tempHolder, i);
            tempHolder.transform.parent = playerHolder.transform;
        }*/


        // Load the Scene
        SceneManager.LoadScene(name);
    }

    void MovePlayerInput(PlayerInput sourceGameObject, GameObject targetGameObject, int index)
    {
        // Get the PlayerInput component from the source GameObject
        PlayerInput playerInput = sourceGameObject;

        if (playerInput != null)
        {
            // Create a new PlayerInput component on the target GameObject
            PlayerInput newPlayerInput = targetGameObject.AddComponent<PlayerInput>();

            // Copy properties from the old PlayerInput to the new one
            newPlayerInput.actions = playerInput?.actions;
            newPlayerInput.defaultActionMap = playerInput?.defaultActionMap;
            newPlayerInput.currentActionMap = playerInput?.currentActionMap;

            // Optionally, transfer any other settings you need
            PlayerDataManager.Instance.GetPlayerData(playerInput).SetPlayerInput(playerInput);

            // Remove the old PlayerInput component
            Destroy(playerInput);
        }
        else
        {
            Debug.LogWarning("No PlayerInput component found on the source GameObject.");
        }
    }
}
