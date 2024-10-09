using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class VisualCosmetic
{
    [SerializeField] public Transform cosmeticPanel;

    [SerializeField] public RectTransform hatContainer;
    [SerializeField] public RectTransform colorContainer;
    [SerializeField] public RectTransform bootsContainer;

    public Image currentHatImage;
    public Image currentColorImage;
    public Image currentBootsImage;
}
