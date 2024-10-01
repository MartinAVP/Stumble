using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CosmeticData
{
    // Constructors
    public CosmeticData(int colorIndex, Material picked)
    {
        this.colorIndex = colorIndex;
        this.picked = picked;
    }

    public CosmeticData()
    {
        this.colorIndex = 0;
        this.picked = null;
    }

    private int colorIndex;
    private Material picked;

    public void SetColorIndex(int colorIndex)
    {
        this.colorIndex = colorIndex;
    }

    public void SetMaterialPicked(Material materialpicked)
    {
        this.picked = materialpicked;
    }

    public int GetColorIndex()
    {
        return this.colorIndex;
    }

    public Material GetMaterialPicked()
    {
        return this.picked;
    }
}

