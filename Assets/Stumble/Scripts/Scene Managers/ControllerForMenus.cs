using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class ControllerForMenus : MonoBehaviour
{
    private PlayerDataManager playerDataManager;
    private PlayerInputManager playerInputManager;
    private PlayerInput input;

    [SerializeField] GameObject FirstSelectedItem;
    [SerializeField] public MultiplayerEventSystem eventSystem;
    public GameObject currentItem;

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

        playerInputManager = FindAnyObjectByType<PlayerInputManager>();
    }

    void Start()
    {
        Task Setup = setup();
    }

    private async Task setup()
    {
        // Note: This system relies on the Main Menu UI Manager & Player Data Manager.
        // If any of these components is missing it will not work.
        while (MainMenuUIManager.Instance == null || MainMenuUIManager.Instance.enabled == false || MainMenuUIManager.Instance.initialized == false
            || PlayerDataManager.Instance == null || PlayerDataManager.Instance.enabled == false)
        {
            // Await 5 ms and try finding it again.
            // It is made 5 seconds because it is
            // a core gameplay mechanic.
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

/*        if (playerDataManager != null)
        {
            //Debug.Log(playerDataManager.GetPlayers().Count);
            if (playerDataManager.GetPlayers().Count > 0)
            {
                //PlayerInput player = playerDataManager.GetPlayerData(0).GetInput();
                if (playerDataManager.GetPlayerData(0).GetInput().currentControlScheme == "Controller")
                {
                    hostUsingController = true;
                }
            }
        }*/
    }

    private void OnDisable()
    {
        if (playerInputManager != null) {
            playerInputManager.onPlayerJoined -= AddPlayer;
        }
    }

    private void AddPlayer(PlayerInput player)
    {
        // Is the first player
        if(player.playerIndex == 0)
        {
            input = player;
            if(player.currentControlScheme == "Controller" || player.currentControlScheme == "Gamepad")
            {
                hostUsingController = true;

                player.uiInputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
                player.camera = Camera.main;

                eventSystem.SetSelectedGameObject(FirstSelectedItem);
                Debug.Log("Current Selected: " + eventSystem.currentSelectedGameObject.name);
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

    public void ChangeSelectedObject(GameObject selected)
    {
        if (hostUsingController)
        {
            eventSystem.SetSelectedGameObject(selected);
            currentItem = selected;
/*            Debug.Log("[CFM] Changed Selected Item for " + selected.name);

            if (selected.GetComponent<Button>() != null)
            {
                Debug.Log("Navigation: Up: " +
                selected.GetComponent<Button>().navigation.selectOnUp.name +
                " Down: " +
                selected.GetComponent<Button>().navigation.selectOnDown.name);
            }

            if (selected.GetComponent<Slider>() != null)
            {
                Debug.Log("Navigation: Up: " +
                selected.GetComponent<Slider>().navigation.selectOnUp.name +
                " Down: " +
                selected.GetComponent<Slider>().navigation.selectOnDown.name);
            }

            CheckSlider();*/
        }
    }
/*
    private bool usingSlider = false;
    private Slider activeSlider = null;
    public void CheckSlider()
    {
        if (currentItem.GetComponent<Slider>() != null)
        {
            Debug.Log("This thing has a slider");
            usingSlider = true;
            activeSlider = currentItem.GetComponent<Slider>();
            if (input.currentActionMap.name == "Player")
            {
                input.gameObject.GetComponent<PlayerSelectAddon>().OnSelectInput.AddListener(SliderControl);
                // Lock Prevention, prevents slider locking.
                Debug.Log("Press Send #2.1");
                input.gameObject.GetComponent<PlayerInputOverhaul>().OnSouthPressed.AddListener(SliderLockPrevention);
            }
            else if (input.currentActionMap.name == "UI")
            {
                input.gameObject.GetComponent<PlayerSelectAddon>().OnSelectInput.AddListener(SliderControl);
                Debug.Log("Press Send #2.2");
                input.gameObject.GetComponent<PlayerInputOverhaul>().OnSouthPressed.AddListener(SliderLockPrevention);
            }
        }
        else
        {
            usingSlider = false;
            activeSlider = null;
            input.gameObject.GetComponent<PlayerSelectAddon>().OnSelectInput.RemoveListener(SliderControl);

            input.gameObject.GetComponent<PlayerInputOverhaul>().OnSouthPressed.RemoveListener(SliderLockPrevention);
        }

        if (currentItem.GetComponent<Button>() != null)
        {
            currentItem = eventSystem.currentSelectedGameObject;
        }
    }

    public void SliderControl(Vector2 raw, PlayerInput data)
    {
        if (activeSlider != null)
        {
            Debug.Log("Slider Performed");
            if (raw.x > .5f)
            {
                Debug.Log("Slider++");
                activeSlider.value = activeSlider.value + raw.x;
            }
            else if (raw.x < -.5f)
            {
                Debug.Log("Slider--");
                activeSlider.value = activeSlider.value + raw.x;
            }

            if (raw.y > .5f)
            {
                ChangeSelectedObject(activeSlider.navigation.selectOnUp.gameObject);
            }
            if (raw.y < -.5f)
            {
                ChangeSelectedObject(activeSlider.navigation.selectOnDown.gameObject);
            }
            Debug.Log("Value: " + activeSlider?.value);
        }
    }
    public void SliderLockPrevention()
    {
        Debug.Log("Press Send #3");
        ChangeObjectSelectedWithDelay(currentItem, .3f);
    }*/

    // Timeds
    public void ChangeObjectSelectedWithDelay(GameObject selected, float delay)
    {
        StartCoroutine(changeObj(selected, delay));
    }
    private IEnumerator changeObj(GameObject selected, float delay)
    {
        yield return new WaitForSeconds(delay);
        ChangeSelectedObject(selected);
    }
}
