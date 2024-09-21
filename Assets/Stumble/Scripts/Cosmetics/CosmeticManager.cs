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

            // Handle Selection in Category
            if(input.x > 0.5f)
            {
                //data.GetCosmeticData().SetColorIndex(0);
                // Get The current Selected Color
                int currentlyAt = data.GetCosmeticData().GetColorIndex();
                // Check if there is more colors available
                if (colors.Count - 1 == currentlyAt)
                {
                    // Set the Index at 0 - First Color
                    data.GetCosmeticData().SetColorIndex(0);
                    currentlyAt = 0;
                }
                // 
                else
                {
                    data.GetCosmeticData().SetColorIndex(currentlyAt + 1);
                    currentlyAt += 1;
                }
                // Change the Material to the One Chosen
                // Set the Material to Cosmetic Data
                data.GetCosmeticData().SetMaterialPicked(colors[currentlyAt].color);
                // Set the Material to the Player
                data.GetPlayerInScene().GetComponentInChildren<MeshRenderer>().material = data.GetCosmeticData().GetMaterialPicked();
                Debug.Log("Right by " + data.id + "///" + data.GetCosmeticData().GetColorIndex());

            }
            else if (input.x < -0.5f)
            {
                Debug.Log("Left by " + data.id);
            }
        }
    }

    public void setDefaultCosmetic(PlayerData data)
    {
        // Set Color
        data.GetCosmeticData().SetColorIndex(0);
        data.GetCosmeticData().SetMaterialPicked(colors[0].color);
    }

/*    private void AddIfNotExist(PlayerData data)
    {
        // Add the Player to the Dictionary
        // Check if the player already Exists in Colors
        if (!cosmeticColors.ContainsKey(data.id))
        {

        }

        foreach (var SelectedCosmetic in selectedCosmetic.Values)
        {
            
        }
    }*/

    public enum SelectedCosmetic
    {
        Colors
    }
}
