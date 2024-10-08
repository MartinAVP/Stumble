using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class ControllerForMenus : MonoBehaviour
{
    private PlayerDataManager playerDataManager;
    private PlayerInputManager playerInputManager;

    [SerializeField] GameObject FirstSelectedItem;
    [SerializeField] MultiplayerEventSystem eventSystem;

    public bool hostUsingController;
    public static ControllerForMenus Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        playerInputManager = FindAnyObjectByType<PlayerInputManager>();
    }

    void Start()
    {
        playerDataManager = PlayerDataManager.Instance;

        if (playerDataManager != null)
        {
            if(playerDataManager.GetPlayers().Count != 0)
            {
                if(playerDataManager.GetPlayerData(0).GetInput().currentControlScheme == "Controller")
                {
                    hostUsingController = true;
                }
            }
        }
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    private void AddPlayer(PlayerInput player)
    {
        // Is the first player
        if(player.playerIndex == 0)
        {
            if(player.currentControlScheme == "Controller")
            {
                hostUsingController = true;
                //Debug.Log("Player 0 Is Controller");
                //eventSystem.firstSelectedGameObject = FirstSelectedItem;
                eventSystem.SetSelectedGameObject(FirstSelectedItem);
            }
        }
    }

    public void ChangeSelectedObject(GameObject selected)
    {
        if (hostUsingController)
        {
            eventSystem.SetSelectedGameObject(selected);
            Debug.Log("Changed Selected Item for " + selected.name);
        }
    }

    public void ChangeObjectSelectedWithDelay(GameObject selected, float delay) {
        StartCoroutine(changeObj(selected, delay));
    }

    private IEnumerator changeObj(GameObject selected, float delay)
    {
        yield return new WaitForSeconds(delay);
        eventSystem.SetSelectedGameObject(selected);
    }
}
