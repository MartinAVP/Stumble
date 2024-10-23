using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModularGamemodes : MonoBehaviour
{
    public List<Gamemode> modules;                  // List of Module Gamemodes 
    [Header("Selection")]
    public List<Gamemode> chosenGamemodes = new List<Gamemode>();          // List of Chosen Module Gamemodes

    [SerializeField]private int targetSelection;
    public bool allowDuplicates = false;

    private void OnDisable()
    {
        chosenGamemodes.Clear();
    }

    private void OnGUI()
    {
        // Set the position and size of the button
        if (GUI.Button(new Rect(10, 10, 150, 30), "Select Gamemodes"))
        {
            chosenGamemodes.Clear();
            SelectGamemodes(targetSelection);
        }
    }

    private void SelectGamemodes(int quantity)
    {
        RecursiveSelect(quantity);
    }

    private void RecursiveSelect(int quantity)
    {
        if (chosenGamemodes.Count >= quantity) { return; } // Finished Doing Recursion
        if(chosenGamemodes.Count == modules.Count) { Debug.LogWarning("Could not continue Recursion, missing asset quantity. Tried getting " + quantity + ", but there are only " + modules.Count + "."); return; } // Finish Recursion - Modules List is not filled enough.
        if(chosenGamemodes.Count >= GetEnabledMapsCount()) { return; }  // Check if there is no more enabled maps available

        int id = Random.Range(0, modules.Count); // Get a random index from the modules list

        // Check for duplicates
        if (chosenGamemodes.Contains(modules[id]) && !allowDuplicates)
        {
            RecursiveSelect(quantity);
        }

        // Check if the Specific Module is disabled
        if (!modules[id].enabled)
        {
            RecursiveSelect(quantity);
        }

        else
        {
            chosenGamemodes.Add(modules[id]);
            //Debug.Log($"Added: {modules[id].name}, Current Count: {chosenGamemodes.Count}");
            RecursiveSelect(quantity);
        }

    }

    private int GetEnabledMapsCount()
    {
        int enabledMaps = 0;
        foreach (var module in modules)
        {
            if (module.enabled) { enabledMaps++; }
        }

        return enabledMaps;
    }
}