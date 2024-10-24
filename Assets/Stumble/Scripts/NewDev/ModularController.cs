using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ModularGamemodes))]
public class ModularController : MonoBehaviour
{
    public int levelID;

    private ModularGamemodes gamemodes;

    private void Start()
    {
        gamemodes = GetComponent<ModularGamemodes>();
    }
}
