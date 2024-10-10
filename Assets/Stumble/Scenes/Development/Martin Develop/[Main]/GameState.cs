using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    StartScreen, // State of Game just Started
    MainMenu, // Host Browsing Main Menu
    Lobby, // All Player Choose Cosmetics / Join Extra players
    ChoosingGameMode, // Host is Choosing the Gamemode

    Race, // Racing Gamemode
    Arena, // Arena Gamemode

    Podium, // All players are in podium, Host can go back to other states
}
