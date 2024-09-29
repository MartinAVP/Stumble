using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

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
        // Spawn the player
        int playersInGame = playerInputManager.playerCount;
        Debug.Log("The current player count is " + playersInGame);
        string playerControlScheme = playerDataManager.GetPlayerData(0).input.currentControlScheme;
        InputDevice playerDevice = playerDataManager.GetPlayerData(0).device;
        //playerInputManager.JoinPlayer(0, playersInGame - playerDataManager.GetPlayers().Count, playerControlScheme, playerDevice);
        playerInputManager.JoinPlayer(0, playersInGame, playerControlScheme, playerDevice);
        Debug.Log("The current player count is #2 " + playerInputManager.playerCount);

        // Assign UI Controller
    }

    private void AddPlayer(PlayerInput input)
    {
        //multiplayerEventSystem.playerRoot = input.gameObject.transform.parent.gameObject;

        //canvas.transform.parent = input.gameObject.transform.parent;
        //eventSystem.transform.parent = input.gameObject.transform.parent;

        input.uiInputModule = multiplayerEventSystem.transform.GetComponent<InputSystemUIInputModule>();
        input.camera = Camera.main;


        PlayerDataManager.Instance.GetPlayerData(0).SetPlayerInput(input);
        PlayerDataManager.Instance.GetPlayerData(0).SetPlayerInScene(input.transform.parent.gameObject);

        Debug.Log(input.currentControlScheme);
        if(input.currentControlScheme == "Controller")
        {
            Debug.LogWarning("The player is using a controller.");
            multiplayerEventSystem.firstSelectedGameObject = firstSelectedIfController.gameObject;
        }
    }
}
