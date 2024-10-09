using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boots", menuName = "Cosmetics/Boots")]
public class CosmeticBoots : ScriptableObject
{
    public String Title;
    public Vector3 offset;
    public GameObject hatPrefab;
    public Sprite iconTexture;
}
