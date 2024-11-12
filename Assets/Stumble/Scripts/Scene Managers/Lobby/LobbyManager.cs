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

    [SerializeField] private List<PlayerLobbySlotUI> playerLobbySlotUi;
    [SerializeField] private List<LobbyRoom> lobbyRooms = new List<LobbyRoom>();

    public int rooms = 4;
    [Space]
    [SerializeField] private GameObject lobbyRoomPrefab;

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
        InitializeLobby();
        Task Setup = setup();
    }

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (GameController.Instance == null || GameController.Instance.enabled == false || GameController.Instance.initialized == false)
        {
            await Task.Delay(1);
        }

        // Once it finds it initialize the scene
        Debug.Log("Initializing Lobby Manager...         [Lobby Manager]");
        initialized = true;

        return;
    }

    private void InitializeLobby()
    {
        for (int i = 0; i < rooms; i++)
        {
            SpawnPlayerRoom(i);
        }
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
        int playerID = player.playerIndex;

        playerLobbySlotUi[playerID].PlayerJoined();
        lobbyRooms[playerID].SetTextureForRenderCam(playerLobbySlotUi[playerID].camTexture);

        playerObj.GetComponent<CharacterController>().enabled = false;
        playerObj.transform.position = lobbyRooms[playerID].roomSpawn.position;
        playerObj.transform.rotation = lobbyRooms[playerID].roomSpawn.rotation;
        playerObj.GetComponent<CharacterController>().enabled = true;
    }

    private void SpawnPlayerRoom(int id)
    {
        GameObject room = Instantiate(lobbyRoomPrefab);
        LobbyRoom lobbyRoom = room.GetComponent<LobbyRoom>();

        Vector3 spawnPos = new Vector3(id * 10, 0, 0);
        room.transform.position = spawnPos;
        lobbyRooms.Add(lobbyRoom);
    }

    public void StartGame(PlayerInput input)
    {
        if(PlayerDataHolder.Instance.GetPlayerData(input)?.isHost == false) {
            return;
        }

        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        StartCoroutine(delayStart());
    }

    private IEnumerator delayStart()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GamemodeSelect");
    }

    public void ReturnToMainMenu(PlayerInput input)
    {
        if (PlayerDataHolder.Instance.GetPlayerData(input)?.isHost == false)
        {
            return;
        }

        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        StartCoroutine(delayReturn());
    }

    private IEnumerator delayReturn()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainMenu");
    }
}
