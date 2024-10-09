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
        this.colorPicked = picked;
    }

    public CosmeticData()
    {
        // Hat
        this.hatIndex = -1;
        this.hatPrefab = null;

        // Color
        this.colorIndex = -1;
        this.colorPicked = null;

        // Boots
        this.bootsIndex = -1;
        this.bootsPrefab = null;
    }

    // Hat
    public int hatIndex { private set; get; }
    public GameObject hatPrefab { private set; get; }
    // Color
    public int colorIndex { private set; get; }
    public Material colorPicked { private set; get; }
    // Boots
    public int bootsIndex { private set; get; }
    public GameObject bootsPrefab { private set; get; }

    public void SetHatIndex(int hatIndex)
    {
        this.hatIndex = hatIndex;
    }

    public void SetHatPrefab(GameObject hatPrefab)
    {
        this.hatPrefab = hatPrefab;
    }

    public void SetColorIndex(int colorIndex)
    {
        this.colorIndex = colorIndex;
    }

    public void SetMaterialPicked(Material materialpicked)
    {
        this.colorPicked = materialpicked;
    }

    public void SetBootsIndex(int bootsIndex)
    {
        this.bootsIndex = bootsIndex;
    }

    public void SetBootsPrefab(GameObject bootsPrefab)
    {
        this.bootsPrefab = bootsPrefab;
    }
}

