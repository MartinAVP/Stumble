using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class ControllerForMenus : MonoBehaviour, IMoveHandler
{
    private PlayerDataManager playerDataManager;
    private PlayerInputManager playerInputManager;

    [SerializeField] GameObject FirstSelectedItem;
    [SerializeField] public MultiplayerEventSystem eventSystem;

    public bool hostUsingController;
    public bool giveHostCursorAccess = true;
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

    }

    private void OnEnable()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    void Start()
    {
        Task Setup = setup();
    }

    private async Task setup()
    {
        // Note: This system relies on the Main Menu UI Manager & Player Data Manager.
        // If any of these components is missing it will not work.
        while (PlayerDataHolder.Instance == null || PlayerDataHolder.Instance.enabled == false)
        {
            await Task.Delay(2);
        }

        // Once it finds it initialize the scene
        Debug.Log("Initializing Controller For Menus...         [Controller for Menus]");
        InitializeUIController();
        return;
    }

    private void InitializeUIController()
    {
        playerDataManager = PlayerDataManager.Instance;
        playerInputManager.onPlayerJoined += AddPlayer;

        if (playerDataManager != null)
        {
            if (playerDataManager.GetPlayers().Count > 0)
            {
                //PlayerInput player = playerDataManager.GetPlayerData(0).GetInput();
                if (playerDataManager.GetPlayerData(0).GetInput().currentControlScheme == "Controller")
                {
                    hostUsingController = true;
                }
            }
        }
    }

    private void OnDisable()
    {
        if (playerInputManager != null) {
            playerInputManager.onPlayerJoined -= AddPlayer;
        }
    }

    private void AddPlayer(PlayerInput player)
    {
        //Debug.Log("PLAYER JOINED EPICly");
        //Is the first player
        if(player.playerIndex == 0)
        {
            if(player.currentControlScheme == "Controller")
            {
                hostUsingController = true;
                //Debug.Log("Player 0 Is Controller");
                //eventSystem.firstSelectedGameObject = FirstSelectedItem;
                eventSystem.SetSelectedGameObject(FirstSelectedItem);
            }
            else
            {
                if (giveHostCursorAccess)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }

    // IMoveHandler interface implementation
    public void OnMove(AxisEventData eventData)
    {
        // Get the current selected GameObject
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

/*        // Check if the selection has changed due to navigation
        if (currentSelected != lastSelected)
        {
            // Handle logic for navigation change (not just selection)
            Debug.Log("Navigated: " + currentSelected.name);

            // Update the last selected element
            lastSelected = currentSelected;
        }
        else
        {
            // The selection hasn't changed, but the user is still navigating
            Debug.Log("Still Navigating: " + currentSelected.name);
        }*/
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
