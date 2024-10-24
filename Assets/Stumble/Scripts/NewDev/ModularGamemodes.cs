using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModularGamemodes : MonoBehaviour
{
    public List<Gamemode> modules;                                          // List of Module Gamemodes 
    [Header("Selection")]
    public List<Gamemode> activeGamemodes = new List<Gamemode>();           // List of Chosen Module Gamemodes

    [SerializeField]private int targetSelection;
    public bool allowDuplicates = false;

    private void OnDisable()
    {
        activeGamemodes.Clear();
    }

    private void Start()
    {

    }

/*    private void OnGUI()
    {
        // Set the position and size of the button
        if (GUI.Button(new Rect(10, 10, 150, 30), "Select Gamemodes"))
        {
            activeGamemodes.Clear();
            SelectGamemodes(targetSelection);
            //Debug.Log("Select Random Gamemodes");
        }
    }*/

    public void SelectGamemodes(int quantity)
    {
        RecursiveSelect(quantity);
    }

    private void RecursiveSelect(int quantity)
    {
        if (activeGamemodes.Count >= quantity) { return; } // Finished Doing Recursion
        if(activeGamemodes.Count == modules.Count) { Debug.LogWarning("Could not continue Recursion, missing asset quantity. Tried getting " + quantity + ", but there are only " + modules.Count + "."); return; } // Finish Recursion - Modules List is not filled enough.
        if(activeGamemodes.Count >= GetEnabledMapsCount()) { return; }  // Check if there is no more enabled maps available

        int id = Random.Range(0, modules.Count); // Get a random index from the modules list

        // Check for duplicates
        if (activeGamemodes.Contains(modules[id]) && !allowDuplicates)
        {
            RecursiveSelect(quantity);
            return;
        }

        // Check if the Specific Module is disabled
        if (modules[id].disabled)
        {
            RecursiveSelect(quantity);
            return;
        }

        else
        {
            activeGamemodes.Add(modules[id]);
            //Debug.Log($"Added: {modules[id].name}, Current Count: {chosenGamemodes.Count}");
            RecursiveSelect(quantity);
        }

    }

    private int GetEnabledMapsCount()
    {
        int enabledMaps = 0;
        foreach (var module in modules)
        {
            if (!module.disabled) { enabledMaps++; }
        }

        return enabledMaps;
    }
}