using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public Transform[] spawns = new Transform[4];
    // Start is called before the first frame update

    private PlayerInputManager playerInputManager;
    public static LobbyManager Instance;

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

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

    public void StartGame()
    {
        SceneManager.LoadScene("Lobby");
    }

    // Change Spawn
    private void AddPlayer(PlayerInput player)
    {
        GameObject playerObj = player.gameObject;
        playerObj.GetComponent<CharacterController>().enabled = false;
        playerObj.transform.position = spawns[playerInputManager.playerCount - 1].position;
        playerObj.transform.rotation = spawns[playerInputManager.playerCount - 1].rotation;
        playerObj.GetComponent<CharacterController>().enabled = true;
    }
}
