using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangingAnnouncer : MonoBehaviour
{
    [SerializeField] ColorChangingMinigame colorMinigame;

    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        if (colorMinigame != null && objectRenderer != null)
        {
            Debug.Log(colorMinigame.SelectedColor);
            objectRenderer.material.color = colorMinigame.SelectedColor;
        }
    }
}
