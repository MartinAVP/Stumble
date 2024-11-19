using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject emptyPanel;
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private GameObject mainLobbyPanel;

    //[SerializeField] private Button startGame;

    // Start is called before the first frame update
    public bool initialized { private set; get; }

    private void Start()
    {
        //StartCoroutine(AwakeCoroutine());
    }

    private IEnumerator AwakeCoroutine()
    {
        GamemodeSelectScreenManager.Instance.InterpolateScreens(emptyPanel, transitionPanel, GamemodeSelectScreenManager.Direction.Left);
        yield return new WaitForSeconds(1f);
        GamemodeSelectScreenManager.Instance.InterpolateScreens(transitionPanel, mainLobbyPanel, GamemodeSelectScreenManager.Direction.Left);
    }

    private void Awake()
    {
        Task Setup = setup();
    }

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (LobbyManager.Instance == null || LobbyManager.Instance.enabled == false || LobbyManager.Instance.initialized == false)
        {
            // Await 5 ms and try finding it again.
            // It is made 5 seconds because it is
            // a core gameplay mechanic.
            await Task.Delay(2);
        }

        // Once it finds it initialize the scene
        Debug.Log("Initializing Lobby UI Manager...         [Lobby UI Manager]");
        Initialize();
        initialized = true;
        //InitializeManager();
        return;
    }

    private void Initialize()
    {
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(false); }
    }


/*    private void OnEnable()
    {
        startGame.onClick.AddListener(StartGame);
    }

    private void OnDisable()
    {
        startGame.onClick.RemoveAllListeners();
    }*/

    public void StartGame()
    {
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        StartCoroutine(delayStart());
    }
    private IEnumerator delayStart()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GamemodeSelect");
    }


}
