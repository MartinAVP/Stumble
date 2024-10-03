using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTestLoader : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    private PlayerInputManager playerInputManager;
    private PlayerDataManager playerDataManager;
    [SerializeField] string quickAccess1, quickAccess2, quickAccess3;
    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        playerDataManager = PlayerDataManager.Instance;
    }

    private void SubmitName(string arg0)
    {
        Debug.Log("Loading Scene... " + arg0);
        if(SceneManager.GetSceneByName(arg0) != null && arg0 != "")
        {
            PlayerDataManager.Instance.isLobby = false;
            SceneManager.LoadScene(arg0);
        }
        else
        {
            Debug.LogError("The scene " + arg0 + " was not found");
        }
    }

    private void Start()
    {
/*        for (int i = 0; i < playerDataManager.GetPlayers().Count; i++)
        {
            int playersInGame = playerInputManager.playerCount;
            //Debug.Log("The current player count is " + playersInGame);
            string playerControlScheme = playerDataManager.GetPlayerData(playersInGame).input.currentControlScheme;
            InputDevice playerDevice = playerDataManager.GetPlayerData(playersInGame).device;
            playerInputManager.JoinPlayer(playersInGame, playersInGame - playerDataManager.GetPlayers().Count, playerControlScheme, playerDevice);
            //Debug.Log("Split Screen Index for " + i + " is " + playersInGame);
        }*/

        int playersInGame = playerInputManager.playerCount;
        //Debug.Log("The current player count is " + playersInGame);
        string playerControlScheme = "Keyboard";
        //InputDevice playerDevice = playerInputManager.;
        //playerInputManager.JoinPlayer(0, 1, null, playerDevice);
    }
    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
        input.onEndEdit.AddListener(SubmitName);  // This also works 
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
        input.onEndEdit.RemoveListener(SubmitName);  // This also works 

    }

    private void AddPlayer(PlayerInput input)
    {
        input.camera = Camera.main;

        //int playerID = playerDataManager.GetPlayersWithInGameCharacter();
        int playerID = 0;

        playerDataManager.GetPlayerData(playerID).SetPlayerInput(input);
        playerDataManager.GetPlayerData(playerID).SetPlayerInputDevice(input.devices[0]);
        playerDataManager.GetPlayerData(playerID).SetPlayerInScene(input.transform.gameObject);

        // Need to use the paren due to the structure of the prefab
        Transform playerParent = input.transform.parent;
        //playerParent.position = spawnPoints[playerInputManager.playerCount - 1].position;
        playerParent.name = "Player #" + (playerID);

        CosmeticManager.Instance.setDefaultCosmetic(playerDataManager.GetPlayerData(playerID));

    }

    public void loadQuickAccesLevel(int level)
    {
        switch (level)
        {
            case 1:
                SubmitName(quickAccess1);
                break;
            case 2:
                SubmitName(quickAccess2);
                break;
            case 3:
                SubmitName(quickAccess3);
                break;
            default:
                Debug.LogError("Not a possible Scene");
                break;
        }
    }


}
