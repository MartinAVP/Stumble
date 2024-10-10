using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boots", menuName = "Cosmetics/Boots")]
public class CosmeticBoots : ScriptableObject
{
    public String Title;

    public Vector3 leftOffset;
    public Vector3 rightOffset;

    public GameObject leftBootPrefab;
    public GameObject rightBootPrefab;

    public Sprite iconTexture;
}
