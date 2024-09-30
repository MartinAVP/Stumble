using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MenuPlayerManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    private PlayerDataManager playerDataManager;
    [SerializeField] private MultiplayerEventSystem multiplayerEventSystem;
    [SerializeField] private Transform canvas;
    [SerializeField] private Transform eventSystem;
    [SerializeField] private Transform firstSelectedIfController;

    private void Awake()
    {
        playerDataManager = PlayerDataManager.Instance;
        playerInputManager = this.GetComponent<PlayerInputManager>();
        multiplayerEventSystem = FindObjectOfType<MultiplayerEventSystem>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
        
    }

    private void Start()
    {
        for (int i = 0; i < playerDataManager.GetPlayers().Count; i++)
        {
            int playersInGame = playerInputManager.playerCount;
            //Debug.Log("The current player count is " + playersInGame);
            string playerControlScheme = playerDataManager.GetPlayerData(playersInGame).input.currentControlScheme;
            InputDevice playerDevice = playerDataManager.GetPlayerData(playersInGame).device;
            playerInputManager.JoinPlayer(playersInGame, playersInGame - playerDataManager.GetPlayers().Count, playerControlScheme, playerDevice);
            //Debug.Log("Split Screen Index for " + i + " is " + playersInGame);
        }
    }

    private void AddPlayer(PlayerInput input)
    {
        if (playerDataManager.GetPlayers().Count == 0) {
            input.uiInputModule = multiplayerEventSystem.transform.GetComponent<InputSystemUIInputModule>();
        }
        input.camera = Camera.main;

        int playerID = playerDataManager.GetPlayersWithInGameCharacter();

        playerDataManager.GetPlayerData(playerID).SetPlayerInput(input);
        playerDataManager.GetPlayerData(playerID).SetPlayerInScene(input.transform.gameObject);

        // Need to use the paren due to the structure of the prefab
        Transform playerParent = input.transform.parent;
        //playerParent.position = spawnPoints[playerInputManager.playerCount - 1].position;
        playerParent.name = "Player #" + (playerID);

        if (playerDataManager.GetPlayerData(playerID).CheckIsHost())
        {
            // Player is host
            if (input.currentControlScheme == "Controller")
            {
                //Debug.LogWarning("Player #0 is using a Controller.");
                multiplayerEventSystem.firstSelectedGameObject = firstSelectedIfController.gameObject;
            }
        }

    }
}
