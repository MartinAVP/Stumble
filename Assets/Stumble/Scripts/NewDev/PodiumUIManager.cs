using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PodiumUIManager : MonoBehaviour
{
    public static PodiumUIManager Instance;

    [SerializeField] private List<TextMeshPro> scenePoints;
    [SerializeField] private List<TextMeshPro> scenePlace;

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
        ResetAllPoints();
    }

    public void ChangePoints(int id, int points)
    {
        scenePoints[id].text = points.ToString() + " pts";
    }

    public void TogglePlace(int id, bool enabled)
    {
        if (enabled)
        {
            scenePlace[id].text = "#" + (id + 1);
        }
        else
        {
            scenePlace[id].text = "";
        }
    }

    public void ResetAllPoints()
    {
        for (int i = 0; i < scenePoints.Count; i++) { 
            scenePoints[i].text = "";
        }

        for (int i = 0; i < scenePlace.Count; i++) {
            scenePlace[i].text = "";
        }
    }
}