using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public Transform[] spawns = new Transform[4];
    // Start is called before the first frame update

    private PlayerInputManager playerInputManager;
    public static LobbyManager Instance;

    [HideInInspector] public bool initialized = false;


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
        Task Setup = setup();
    }

/*    public void StartGame()
    {
        SceneManager.LoadScene("Lobby");
    }*/

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (ExperimentalPlayerManager.Instance == null || ExperimentalPlayerManager.Instance.enabled == false || ExperimentalPlayerManager.Instance.finishedSystemInitializing == false)
        {
            // Await 5 ms and try finding it again.
            // It is made 5 seconds because it is
            // a core gameplay mechanic.
            await Task.Delay(140);
        }

        // Once it finds it initialize the scene
        Debug.Log("Initializing Lobby Manager...         [Lobby Manager]");
        initialized = true;
        //InitializeManager();
        return;
    }


    // Sub to Player Join Event
    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
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
