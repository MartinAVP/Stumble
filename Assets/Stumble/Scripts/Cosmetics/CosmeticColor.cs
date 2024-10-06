using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Color", menuName = "Cosmetics/Color")]
public class CosmeticColor : ScriptableObject
{
    public String Title;
    public Material colorMaterial;
}
