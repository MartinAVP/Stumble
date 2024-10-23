using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Gamemodes", menuName = "Gamemodes/Gamemode")]
public class Gamemode : ScriptableObject
{
    public string title;
    public string description;
    public GamemodeType type;
    public Texture2D modulePreview;
    public SceneAsset scene;
    [HideInInspector] public bool enabled;
}

public enum GamemodeType
{
    Race,
    LastManStanding
}
