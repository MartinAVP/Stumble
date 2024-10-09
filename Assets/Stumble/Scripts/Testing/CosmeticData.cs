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
        this.rightBootPrefab = null;
        this.leftBootPrefab = null;
    }

    // Hat
    public int hatIndex { private set; get; }
    public GameObject hatPrefab { private set; get; }
    // Color
    public int colorIndex { private set; get; }
    public Material colorPicked { private set; get; }
    // Boots
    public int bootsIndex { private set; get; }
    public GameObject rightBootPrefab { private set; get; }
    public GameObject leftBootPrefab { private set; get; }

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

    public void SetRightBootPrefab(GameObject bootPrefab)
    {
        this.rightBootPrefab = bootPrefab;
    }

    public void SetLeftBootPrefab(GameObject bootPrefab)
    {
        this.leftBootPrefab = bootPrefab;
    }
}

