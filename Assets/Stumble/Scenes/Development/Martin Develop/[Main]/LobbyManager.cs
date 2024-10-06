using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    public Transform[] spawns = new Transform[4];
    // Start is called before the first frame update

    private PlayerInputManager playerInputManager;

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
        playerInputManager = FindAnyObjectByType<PlayerInputManager>();
    }

    void Start()
    {
        
    }

    private void AddPlayer(PlayerInput player)
    {
        GameObject playerObj = player.gameObject;
        playerObj.GetComponent<CharacterController>().enabled = false;
        playerObj.transform.position = spawns[playerInputManager.playerCount - 1].position;
        playerObj.transform.rotation = spawns[playerInputManager.playerCount - 1].rotation;
        playerObj.GetComponent<CharacterController>().enabled = true;
    }
}
