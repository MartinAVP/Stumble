using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ModularGamemodes : MonoBehaviour
{
    public List<Gamemode> modules = new List<Gamemode>();                  // List of Module Gamemodes 
    [Header("Selection")]
    public List<Gamemode> activeGamemodes = new List<Gamemode>();           // List of Chosen Module Gamemodes
    public bool allowDuplicates = false;

    private void OnDisable()
    {
        modules.Clear();
        activeGamemodes.Clear();
    }

    private void Awake()
    {
        // Load Assets from "Resources/Modules" folder
        PopulateModulesFromFile();
    }

    private void PopulateModulesFromFile()
    {
        modules = Resources.LoadAll("Modules", typeof(Gamemode)).Cast<Gamemode>().ToList();
    }

    public void SelectSpecificGamemode(Gamemode gamemode)
    {
        activeGamemodes.Clear();
        activeGamemodes.Add(gamemode);
    }

    public void SelectGamemodes(int quantity)
    {
        activeGamemodes.Clear();
        RecursiveSelect(quantity);
    }

    private void RecursiveSelect(int quantity)
    {
        if (activeGamemodes.Count >= quantity) { return; }
        if(activeGamemodes.Count == modules.Count) { Debug.LogWarning("Could not continue Recursion, missing asset quantity. Tried getting " + quantity + ", but there are only " + modules.Count + "."); return; } // Finish Recursion - Modules List is not filled enough.
        if(activeGamemodes.Count >= GetEnabledMapsCount()) { return; }

        int id = Random.Range(0, modules.Count);

        if (activeGamemodes.Contains(modules[id]) && !allowDuplicates)
        {
            RecursiveSelect(quantity);
            return;
        }

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