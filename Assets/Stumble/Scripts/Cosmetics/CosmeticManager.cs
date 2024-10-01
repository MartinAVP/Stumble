using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticManager : MonoBehaviour
{

    public List<CosmeticColor> colors = new List<CosmeticColor>();
/*    public SelectedCosmetic currentCosmeticSelection;

    public IDictionary<int, SelectedCosmetic> selectedCosmetic;
    public IDictionary<int, int> cosmeticColors;*/
    public bool canEdit;

    // Singleton
    public static CosmeticManager Instance { get; private set; }

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

    private void Start()
    {
        canEdit = GameSceneManager.Instance.GetLobby();
    }

    public void ChangeCosmetic(Vector2 input, PlayerData data)
    {
        if (canEdit)
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
                data.GetCosmeticData().SetMaterialPicked(colors[currentlyAt].color);
                // Set the Material to the Player
                data.GetPlayerInScene().GetComponentInChildren<MeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();

            }
            else if (input.x < -0.5f)
            {
                int currentlyAt = data.GetCosmeticData().GetColorIndex();
                currentlyAt = GetNextAvailableIndexColor(currentlyAt);
                // Check if there is more colors available
                // Set the Material to Cosmetic Data
                data.GetCosmeticData().SetColorIndex(currentlyAt);
                data.GetCosmeticData().SetMaterialPicked(colors[currentlyAt].color);
                // Set the Material to the Player
                data.GetPlayerInScene().GetComponentInChildren<MeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
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
                data.GetCosmeticData().SetMaterialPicked(colors[i].color);
                data.GetPlayerInScene().GetComponentInChildren<MeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                return;
            }
        }

        // Default Color 0
        data.GetCosmeticData().SetColorIndex(0);
        data.GetCosmeticData().SetMaterialPicked(colors[0].color);
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

    // Enum for Selected Cosmetic
    public enum SelectedCosmetic
    {
        Colors
    }
}
