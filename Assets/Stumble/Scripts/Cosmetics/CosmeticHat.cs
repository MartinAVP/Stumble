using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hat", menuName = "Cosmetics/Hat")]
public class CosmeticHat : ScriptableObject
{
    public String Title;
    public Vector3 offset;
    public GameObject hatPrefab;
    public Sprite iconTexture;
}

