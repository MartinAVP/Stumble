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

    public List<CosmeticHat> hats = new List<CosmeticHat>();
    public List<CosmeticColor> colors = new List<CosmeticColor>();
    public List<CosmeticBoots> boots = new List<CosmeticBoots>();

    public IDictionary<int, SelectedCosmetic> selectedCosmetic = new Dictionary<int, SelectedCosmetic>();
    public IDictionary<int, bool> playerCooldown = new Dictionary<int, bool>();

    // Singleton
    public static CosmeticManager Instance { get; private set; }
    private PlayerInputManager playerInputManager;

    [Header("Settings")]
    public float inputDelay = 1.0f;

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
        selectedCosmetic.Clear();
    }

    private void OnEnable()
    {
        // Moved to Enable so Unity has time to start the Player Input Manager
        playerInputManager = FindAnyObjectByType<PlayerInputManager>();
        playerInputManager.onPlayerJoined += AddPlayer;

        playerCooldown.Clear();
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    private void AddPlayer(PlayerInput player)
    {
        PlayerDataHolder playerDataManager = PlayerDataHolder.Instance; 
        // Player Data Manager Exists
        if (playerDataManager != null)
        {
/*            Debug.LogWarning("Applying default Color to " + player.playerIndex);
            setDefaultCosmetic(playerDataManager.GetPlayerData(player));*/
        }
        else // No Data Manager
        {
            // Set Default color to all players
            if(GameController.Instance != null)
            {
                Debug.LogWarning("No Player Data Manager, Using default Color");
                player.gameObject.transform.parent.GetComponentInChildren<MeshRenderer>().material = colors[0].colorMaterial;
            }
        }

        //Debug.Log("Contents of Dictionary;");
/*        foreach(var kvp in selectedCosmetic)
        {
            Debug.Log(kvp.Key + " at " + kvp.Value);
        }*/
        if (!selectedCosmetic.ContainsKey(player.playerIndex))
        {
            selectedCosmetic.Add(player.playerIndex, GetSelectedCosmetic(1));
        }
        //selectedCosmetic.TryAdd(player.playerIndex, GetSelectedCosmetic(1));
        if(!playerCooldown.ContainsKey(player.playerIndex))
        {
            playerCooldown.Add(player.playerIndex, false);
        }
        //Debug.Log("Player #" + player.playerIndex + " has selected category " + selectedCosmetic[player.playerIndex].ToString());


/*        // Add Cosmetic Selection
        // Get the action map and action
        var actionMap = player.actions.FindActionMap("Player"); // Replace with your action map name
        var moveCosmeticAction = actionMap.FindAction("Select"); // Replace with your action name

        // Subscribe to the action
        moveCosmeticAction.performed += MoveCosmetic;*/
    }

    public void ChangeColor(Vector2 input, PlayerData data)
    {
        if (GameController.Instance.gameState == GameState.Lobby)
        {
            // Handle Change Category

            // Player moves right
            if(input.x > 0.5f)
            {
                // Get The current Selected Color
                int currentlyAt = data.GetCosmeticData().colorIndex;
                currentlyAt = GetNextAvailableIndexColor(currentlyAt);
                // Check if there is more colors available
                // Set the Material to Cosmetic Data
                data.GetCosmeticData().SetColorIndex(currentlyAt);
                data.GetCosmeticData().SetMaterialPicked(colors[currentlyAt].colorMaterial);
                // Set the Material to the Player
                //data.GetPlayerInScene().GetComponentInChildren<MeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().body.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().colorPicked;
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().eyes.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().colorPicked;
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().fins.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().colorPicked;

                if(CosmeticUI.Instance != null)
                {
                    CosmeticUI.Instance.ChangeImage(CosmeticUI.Direction.Right, colors[currentlyAt].iconTexture, data.GetInput().playerIndex, 1);
                }
            }
            else if (input.x < -0.5f)
            {
                int currentlyAt = data.GetCosmeticData().colorIndex;
                currentlyAt = GetNextAvailableIndexColor(currentlyAt);
                // Check if there is more colors available
                // Set the Material to Cosmetic Data
                data.GetCosmeticData().SetColorIndex(currentlyAt);
                data.GetCosmeticData().SetMaterialPicked(colors[currentlyAt].colorMaterial);
                // Set the Material to the Player
                //data.GetPlayerInScene().GetComponentInChildren<MeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().body.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().colorPicked;
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().eyes.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().colorPicked;
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().fins.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().colorPicked;

                if (CosmeticUI.Instance != null)
                {
                    CosmeticUI.Instance.ChangeImage(CosmeticUI.Direction.Left, colors[currentlyAt].iconTexture, data.GetInput().playerIndex, 1);
                }
            }
        }
    }


    public void ChangeHat(Vector2 input, PlayerData data)
    {
        if (GameController.Instance.gameState == GameState.Lobby)
        {
            // Handle Change Category

            // Player moves right
            if (input.x > 0.5f)
            {
                // Get The current Selected Color
                int currentlyAt = data.GetCosmeticData().hatIndex;
                currentlyAt = GetNextAvailableIndexHat(currentlyAt);

                // Check if there is more colors available
                // Set the Material to Cosmetic Data
                data.GetCosmeticData().SetHatIndex(currentlyAt);
                data.GetCosmeticData().SetHatPrefab(hats[currentlyAt].hatPrefab);

                Transform hatpos = data.GetPlayerInScene().GetComponent<PlayerCosmetics>().hatPos;
                // Remove Any Hats if Exist
                if(hatpos.childCount > 0)
                {
                    foreach(Transform child in hatpos)
                    {
                        Destroy(child.gameObject);
                    }
                }

                GameObject newHat = Instantiate(hats[currentlyAt].hatPrefab, hatpos.transform);
                newHat.transform.SetParent(hatpos.transform);

                if (CosmeticUI.Instance != null)
                {
                    CosmeticUI.Instance.ChangeImage(CosmeticUI.Direction.Right, hats[currentlyAt].iconTexture, data.GetInput().playerIndex, 0);
                }
            }
            else if (input.x < -0.5f)
            {
                // Get The current Selected Color
                int currentlyAt = data.GetCosmeticData().hatIndex;
                currentlyAt = GetPreviousAvailableIndexHat(currentlyAt);

                // Check if there is more colors available
                // Set the Material to Cosmetic Data
                data.GetCosmeticData().SetHatIndex(currentlyAt);
                data.GetCosmeticData().SetHatPrefab(hats[currentlyAt].hatPrefab);

                Transform hatpos = data.GetPlayerInScene().GetComponent<PlayerCosmetics>().hatPos;
                // Remove Any Hats if Exist
                if (hatpos.childCount > 0)
                {
                    foreach (Transform child in hatpos)
                    {
                        Destroy(child.gameObject);
                    }
                }

                GameObject newHat = Instantiate(hats[currentlyAt].hatPrefab, hatpos.transform);
                newHat.transform.SetParent(hatpos.transform);

                if (CosmeticUI.Instance != null)
                {
                    CosmeticUI.Instance.ChangeImage(CosmeticUI.Direction.Left, hats[currentlyAt].iconTexture, data.GetInput().playerIndex, 0);
                }
            }
        }
    }

    public void ChangeBoots(Vector2 input, PlayerData data)
    {
        if (GameController.Instance.gameState == GameState.Lobby)
        {
            // Handle Change Category

            // Player moves right
            if (input.x > 0.5f)
            {
                // Get The current Selected Color
                int currentlyAt = data.GetCosmeticData().bootsIndex;
                currentlyAt = GetNextAvailableIndexBoots(currentlyAt);

                // Check if there is more colors available
                // Set the Material to Cosmetic Data
                data.GetCosmeticData().SetBootsIndex(currentlyAt);
                data.GetCosmeticData().SetRightBootPrefab(boots[currentlyAt].rightBootPrefab);
                data.GetCosmeticData().SetLeftBootPrefab(boots[currentlyAt].leftBootPrefab);

                Transform rightBootPos = data.GetPlayerInScene().GetComponent<PlayerCosmetics>().rightFoot;
                Transform leftBootPos = data.GetPlayerInScene().GetComponent<PlayerCosmetics>().leftFoot;
                // Remove Any Right Boot if Exist
                if (rightBootPos.childCount > 0)
                {
                    foreach (Transform child in rightBootPos)
                    {
                        Destroy(child.gameObject);
                    }
                }
                // Remove Any Left Boot if Exist
                if (leftBootPos.childCount > 0)
                {
                    foreach (Transform child in leftBootPos)
                    {
                        Destroy(child.gameObject);
                    }
                }

                GameObject newRightBoot = Instantiate(boots[currentlyAt].rightBootPrefab, rightBootPos.transform);
                newRightBoot.transform.SetParent(rightBootPos.transform);

                GameObject newLeftBoot = Instantiate(boots[currentlyAt].leftBootPrefab, leftBootPos.transform);
                newLeftBoot.transform.SetParent(leftBootPos.transform);

                if (CosmeticUI.Instance != null)
                {
                    CosmeticUI.Instance.ChangeImage(CosmeticUI.Direction.Right, boots[currentlyAt].iconTexture, data.GetInput().playerIndex, 2);
                }
            }
            else if (input.x < -0.5f)
            {
                // Get The current Selected Color
                int currentlyAt = data.GetCosmeticData().bootsIndex;
                currentlyAt = GetPreviousAvailableIndexBoots(currentlyAt);

                // Check if there is more colors available
                // Set the Material to Cosmetic Data
                data.GetCosmeticData().SetBootsIndex(currentlyAt);
                data.GetCosmeticData().SetRightBootPrefab(boots[currentlyAt].rightBootPrefab);
                data.GetCosmeticData().SetLeftBootPrefab(boots[currentlyAt].leftBootPrefab);

                Transform rightBootPos = data.GetPlayerInScene().GetComponent<PlayerCosmetics>().rightFoot;
                Transform leftBootPos = data.GetPlayerInScene().GetComponent<PlayerCosmetics>().leftFoot;
                // Remove Any Right Boot if Exist
                if (rightBootPos.childCount > 0)
                {
                    foreach (Transform child in rightBootPos)
                    {
                        Destroy(child.gameObject);
                    }
                }
                // Remove Any Left Boot if Exist
                if (leftBootPos.childCount > 0)
                {
                    foreach (Transform child in leftBootPos)
                    {
                        Destroy(child.gameObject);
                    }
                }

                GameObject newRightBoot = Instantiate(boots[currentlyAt].rightBootPrefab, rightBootPos.transform);
                newRightBoot.transform.SetParent(rightBootPos.transform);

                GameObject newLeftBoot = Instantiate(boots[currentlyAt].leftBootPrefab, leftBootPos.transform);
                newLeftBoot.transform.SetParent(leftBootPos.transform);

                if (CosmeticUI.Instance != null)
                {
                    CosmeticUI.Instance.ChangeImage(CosmeticUI.Direction.Left, boots[currentlyAt].iconTexture, data.GetInput().playerIndex, 2);
                }
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
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().body.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().colorPicked;
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().eyes.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().colorPicked;
                data.GetPlayerInScene().GetComponent<PlayerCosmetics>().fins.GetComponent<SkinnedMeshRenderer>().material = data.GetCosmeticData().colorPicked;
                /*                return;*/

                if (CosmeticUI.Instance != null)
                {
                    CosmeticUI.Instance.SetDefaultImage(colors[i].iconTexture, data.GetInput().playerIndex, 1);
                }

                //Debug.Log("Color set to #" + data.GetInput().playerIndex + " to " + colors[i].Title);
                break;
            }

        }

        // Apply Non Variable Cosmetics
        data.GetCosmeticData().SetHatIndex(0);
        data.GetCosmeticData().SetHatPrefab(hats[0].hatPrefab);

        if (CosmeticUI.Instance != null)
        {
            CosmeticUI.Instance.SetDefaultImage(hats[0].iconTexture, data.GetInput().playerIndex, 0);
            CosmeticUI.Instance.SetDefaultImage(boots[0].iconTexture, data.GetInput().playerIndex, 2);
        }

        // Default Color 0
        /*        data.GetCosmeticData().SetColorIndex(0);
                data.GetCosmeticData().SetMaterialPicked(colors[0].colorMaterial);*/
    }

    /// <summary>
    ///  Checks if the color is in use
    /// </summary>
    /// <param name="color">The color to be checked</param>
    /// <returns></returns>
    private bool IsColorInUse(CosmeticColor color)
    {
        List<PlayerData> players = PlayerDataHolder.Instance.GetPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            int colorID = players[i].GetCosmeticData().colorIndex;
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

    private int GetNextAvailableIndexHat(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt + 1;
        // Loop through list
        if (index >= hats.Count)
        {
            index = 0;
        }

        return index;
    }

    private int GetPreviousAvailableIndexHat(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt - 1;
        // Loop through list
        if (index < 0)
        {
            index = hats.Count - 1;
        }

        return index;
    }

    private int GetNextAvailableIndexBoots(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt + 1;
        // Loop through list
        if (index >= boots.Count)
        {
            index = 0;
        }

        return index;
    }

    private int GetPreviousAvailableIndexBoots(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt - 1;
        // Loop through list
        if (index < 0)
        {
            index = boots.Count - 1;
        }

        return index;
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

    private int GetNextCategory(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt + 1;
        // Loop through list
        if (index > System.Enum.GetValues(typeof(SelectedCosmetic)).Length - 2)
        {
            index = 0;
        }

        return index;
    }
    private int GetPreviousCategory(int currentlyAt)
    {
        // Add one to currently At
        int index = currentlyAt - 1;
        // Loop through list
        if (index < 0)
        {
            index = System.Enum.GetValues(typeof(SelectedCosmetic)).Length - 2;
        }

        return index;
    }

    private int GetSelectedCosmeticID(SelectedCosmetic cosmetic) {
        switch (cosmetic)
        {
            case SelectedCosmetic.Hats: return 0;
            case SelectedCosmetic.Colors: return 1;
            case SelectedCosmetic.Boots: return 2;
            default: return -1;
        }
    }
    private SelectedCosmetic GetSelectedCosmetic(int id)
    {
        switch (id)
        {
            case 0:
                return SelectedCosmetic.Hats;
            case 1:
                return SelectedCosmetic.Colors;
            case 2:
                return SelectedCosmetic.Boots;
            default:
                Debug.LogError("No Cosmetic Foud");
                return SelectedCosmetic.Error;
        }
    }


    public void MoveCosmetic(Vector2 value, PlayerInput input)
    {
        //while(value != Vector2.zero) { Debug.Log("Diablo"); }
        PlayerData data = PlayerDataHolder.Instance.GetPlayerData(input);
        if(data == null)
        {
            Debug.LogError("The Device is not finding a player attached");
        }
        // Player is Under Cooldown
        if (playerCooldown[data.GetInput().playerIndex]) { return; }

        // Handle Switching Categories
        if(value.y != 0)
        {
            SwitchSelectedCategory(value, data);
        }
        /*        data.GetInput().playerIndex;*/

        switch (selectedCosmetic[data.GetInput().playerIndex])
        {
            case SelectedCosmetic.Hats:
                ChangeHat(value, data);
                break;
            case SelectedCosmetic.Colors:
                ChangeColor(value, data);
                break;
            case SelectedCosmetic.Boots:
                break;
            default:
                break;
        }

        StartCoroutine(InputDelay(data.GetInput().playerIndex));

    }

    private IEnumerator InputDelay(int playerID)
    {
        playerCooldown[playerID] = true;
        yield return new WaitForSeconds(inputDelay);
        playerCooldown[playerID] = false;
    }

    private void SwitchSelectedCategory(Vector2 inputValue, PlayerData data)
    {
        if (inputValue.y > 0.5)
        {
            // Switch Up
            int playerID = data.GetInput().playerIndex;
            int id = GetPreviousCategory(GetSelectedCosmeticID(selectedCosmetic[playerID]));
            selectedCosmetic[playerID] = GetSelectedCosmetic(id);
            Debug.Log(" UP | Player #" + playerID + " now has selected:" + GetSelectedCosmetic(id).ToString() + "| with id " + id);
        }
        if (inputValue.y < -0.5) {
            // Switch Down
            int playerID = data.GetInput().playerIndex;
            int id = GetNextCategory(GetSelectedCosmeticID(selectedCosmetic[playerID]));
            selectedCosmetic[playerID] = GetSelectedCosmetic(id);
            Debug.Log("DOWN | Player #" + playerID + " now has selected:" + GetSelectedCosmetic(id).ToString() + "| with id " + id);
        }
    }

    // Enum for Selected Cosmetic
    public enum SelectedCosmetic
    {
        Hats,
        Colors,
        Boots,
        Error
    }

    // =====================================================================================================================================
    // =====================================================================================================================================
    // =====================================================================================================================================

}
