using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class VisualCosmetic
{
    [SerializeField] public Transform cosmeticPanel;
    [Space]
    [SerializeField] public RectTransform hatContainer;
    [SerializeField] public RectTransform colorContainer;
    [SerializeField] public RectTransform bootsContainer;
    [Space]
    public Image currentHatImage;
    public Image currentColorImage;
    public Image currentBootsImage;
}
