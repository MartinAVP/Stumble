using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CosmeticManager : MonoBehaviour
{
    // Important:
    // This script is only meant to be in the lobby scene if its
    // on another scene it is possible that it will clash with
    // other systems.

    public List<CosmeticColor> colors = new List<CosmeticColor>();
/*    public SelectedCosmetic currentCosmeticSelection;

    public IDictionary<int, SelectedCosmetic> selectedCosmetic;
    public IDictionary<int, int> cosmeticColors;*/
    //public bool canEdit;

    // Singleton
    public static CosmeticManager Instance { get; private set; }
    private PlayerInputManager playerInputManager;

    // Singleton
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
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
        playerInputManager = FindAnyObjectByType<PlayerInputManager>();
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    private void AddPlayer(PlayerInput player)
    {
        PlayerDataManager playerDataManager = PlayerDataManager.Instance; 
        // Player Data Manager Exists
        if (playerDataManager != null)
        {
/*            Debug.LogWarning("Applying default Color to " + player.playerIndex);
            setDefaultCosmetic(playerDataManager.GetPlayerData(player));*/
        }
        else // No Data Manager
        {
            // Set Default color to all players
            Debug.LogWarning("No Player Data Manager, Using default Color");
            player.gameObject.transform.parent.GetComponentInChildren<MeshRenderer>().material = colors[0].colorMaterial;
        }


/*        // Add Cosmetic Selection
        // Get the action map and action
        var actionMap = player.actions.FindActionMap("Player"); // Replace with your action map name
        var moveCosmeticAction = actionMap.FindAction("Select"); // Replace with your action name

        // Subscribe to the action
        moveCosmeticAction.performed += MoveCosmetic;*/
    }

    public void ChangeCosmetic(Vector2 input, PlayerData data)
    {
        if (GameController.Instance.gameState == GameState.Lobby)
        {
            // Handle Change Category

            // Player moves right
            if(input.x > 0.5f)
            {
                // Get The current Selected Color
                int currentlyAt = data.GetCosmeticData().GetColorIndex();
                currentlyAt = GetNextAvailableIndexColor(currentlyAt);
                // Check if there is more colors available
                // Set the Material to Cosmetic Data
                data.GetCosmeticData().SetColorIndex(currentlyAt);
                data.GetCosmeticData().SetMaterialPicked(colors[currentlyAt].colorMaterial);
                // Set the Material to the Player
                //data.GetPlayerInScene().GetComponentInChildren<MeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().body.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().eyes.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().fins.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();

            }
            else if (input.x < -0.5f)
            {
                int currentlyAt = data.GetCosmeticData().GetColorIndex();
                currentlyAt = GetNextAvailableIndexColor(currentlyAt);
                // Check if there is more colors available
                // Set the Material to Cosmetic Data
                data.GetCosmeticData().SetColorIndex(currentlyAt);
                data.GetCosmeticData().SetMaterialPicked(colors[currentlyAt].colorMaterial);
                // Set the Material to the Player
                //data.GetPlayerInScene().GetComponentInChildren<MeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().body.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().eyes.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().fins.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
            }
        }
    }


    /// <summary>
    /// Takes the playerData when getting setting the default cosmetic and applies a non used
    /// color to the new player
    /// </summary>
    /// <param name="data"> The Data of the New joined player</param>
    public void setDefaultCosmetic(PlayerData data)
    {
        // Set Color
/*        data.GetCosmeticData().SetColorIndex(0);
        data.GetCosmeticData().SetMaterialPicked(colors[0].color);*/

        // Set a Color that is Not used
        List<PlayerData> playerData = new List<PlayerData>();
        // Loop through all players
        // Check if the color is being used by the player
        for (int i = 0; i < colors.Count; i++)
        {
            // Check if the color is used
            if (!IsColorInUse(colors[i]))
            {
                // Color is not in Use
                data.GetCosmeticData().SetColorIndex(i);
                //Debug.Log("Color with ID " + i + " " + colors[i].Title + " is not in use");
                data.GetCosmeticData().SetMaterialPicked(colors[i].colorMaterial);
                //data.GetPlayerInScene().GetComponentInChildren<MeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().body.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().eyes.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().fins.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                return;
            }
        }

        // Default Color 0
        data.GetCosmeticData().SetColorIndex(0);
        data.GetCosmeticData().SetMaterialPicked(colors[0].colorMaterial);
    }

    /// <summary>
    ///  Checks if the color is in use
    /// </summary>
    /// <param name="color">The color to be checked</param>
    /// <returns></returns>
    private bool IsColorInUse(CosmeticColor color)
    {
        List<PlayerData> players = PlayerDataManager.Instance.GetPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            int colorID = players[i].GetCosmeticData().GetColorIndex();
            // Check if the ColorID is in the array size
            if(colorID < 0 || colorID > colors.Count)
            {
                return false;
            }

            if (colors[colorID] == color)
            {
                //Debug.Log("Color is in Use by Player #" + i);
                return true;
            }
        }

        //Debug.Log("Color is not in Use");
        return false;
    }

    /// <summary>
    /// Gets the next Available color that is not used by any player
    /// 
    /// !! Function is Recursive
    /// </summary>
    /// <param name="currentlyAt">The Index ID of the player's current color</param>
    /// <returns></returns>
    private int GetNextAvailableIndexColor(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt + 1;
        // Loop through list
        if(index >= colors.Count)
        {
            index = 0;
        }
        
        // Check if the index is busy
        if (IsColorInUse(colors[index]))
        {
            index = GetNextAvailableIndexColor(index);
        }
        // 
        return index;
    }

    /// <summary>
    /// Gets the previous available color in the colors array that no player is using
    /// 
    /// !! Function is Recursive
    /// </summary>
    /// <param name="currentlyAt">The index of the colors array the player is currently at</param>
    /// <returns></returns>
    private int GetPreviousAvailableIndexColor(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt - 1;
        // Loop through list
        if (index <= colors.Count)
        {
            index = colors.Count;
        }

        // Check if the index is busy
        if (IsColorInUse(colors[index]))
        {
            index = GetPreviousAvailableIndexColor(index);
        }
        // 
        return index;
    }

/*    private void AddPlayer(PlayerInput player)
    {

    }*/

    public void MoveCosmetic(Vector2 value, PlayerInput device)
    {
        //Vector2 value = context.ReadValue<Vector2>();
        //Debug.Log(context.action.activeControl.device);
        //PlayerInput playerInput = this.GetComponent<PlayerInput>();
        PlayerData data = PlayerDataManager.Instance.GetPlayerData(device);
        if(data == null)
        {
            Debug.LogError("The Device is not finding a player attached");
        }
        ChangeCosmetic(value, data);
    }

    // Enum for Selected Cosmetic
    public enum SelectedCosmetic
    {
        Colors,
        Hats,
    }
}
