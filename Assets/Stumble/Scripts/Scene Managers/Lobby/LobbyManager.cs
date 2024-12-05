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
    private bool transitioning = false;
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
        const int rows = 2;
        const int columns = 2;

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                SpawnPlayerRoom(j, i);
            }
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

    private void SpawnPlayerRoom(int columnId, int rowId)
    {
        GameObject room = Instantiate(lobbyRoomPrefab);
        LobbyRoom lobbyRoom = room.GetComponent<LobbyRoom>();

        Vector3 spawnPos = new Vector3((columnId + 10) * 10, (rowId + 10) * 10, 0f);

        room.transform.position = spawnPos;
        lobbyRooms.Add(lobbyRoom);
    }

    public void StartGame(PlayerInput input)
    {
        if(PlayerDataHolder.Instance.GetPlayerData(input)?.isHost == false) {
            return;
        }

        if(transitioning) return;
        transitioning = true;

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
        if (PlayerDataHolder.Instance.GetPlayerData(input).isHost == false)
        {
            return;
        }

        if(transitioning) return;
        transitioning = true;

        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        StartCoroutine(delayReturn("Menu"));
    }

    private IEnumerator delayReturn(string scene)
    {
        yield return new WaitForSeconds(2f);
        PlayerDataHolder.Instance.ClearAllButHost(true);
        SceneManager.LoadScene(scene);
    }

    public void RemovePlayer(PlayerInput input)
    {
        PlayerData data = PlayerDataHolder.Instance.GetPlayerData(input);
        Destroy(input.transform.parent.gameObject);

        // Only Removes it from the Database
        PlayerDataHolder.Instance.RemovePlayer(input);

        // If no players Left, Go back to StartScreen.
        if(PlayerDataHolder.Instance.GetPlayers().Count <= 0)
        {
            // All Players Including Host left, send to StartScreen.
            if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
            StartCoroutine(delayReturn("StartScreen"));
        }
    }
}
