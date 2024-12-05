using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDataHolder : MonoBehaviour
{
    // == The Master Player Holder ==
    // This class holds every player that is being managed in the game.
    // This class should not be wiped across scenes, it is what carries
    // the players over.
    //public List<PlayerData> players { get; private set; } = new List<PlayerData>();
    public List<PlayerData> players = new List<PlayerData>();

    // Instance of the Class
    public static PlayerDataHolder Instance { get; private set; }

    // Singleton
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Debug.Log("There were 2 or more instances of the PlayerDataHolder");
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Initialize Task to Start Data Management
        //Task Setup = setup();
    }

/*    private async Task setup()
    {
        // Wait for these values GameController needs to be 
        while (GameController.Instance == null || GameController.Instance.enabled == false || GameController.Instance.initialized == false)
        {
            await Task.Delay(1);
        }

        Debug.Log("Player Data Holder Found... Holds " + players.Count + " Players          [Player Data Holder]");
    }*/

    // Get All Players
    public List<PlayerData> GetPlayers()
    {
        return players;
    }

    // Add a Player
    public void AddPlayer(PlayerInput input)
    {
        // The Reason behind setting the player as the parent of the
        // gameobject is because of the player prefab layout.
        //Debug.Log("Adding Player");
        GameObject playerGameObject = input.transform.parent.gameObject;
        bool isFirstPlayer = false;

        PlayerData tempPlayerData;
        if(players.Count == 0)
        {
            isFirstPlayer = true;
        }

        tempPlayerData = new PlayerData(players.Count, playerGameObject, input, input.devices[0], isFirstPlayer, new CosmeticData());
        players.Add(tempPlayerData);
    }

    // Remove a Player
    public void RemovePlayer(PlayerInput input)
    {
/*        GameState state = GameController.Instance.gameState;
        if (state != GameState.Lobby)
        {
            return;
        }*/

        int playerID = FindPlayer(input);

        if (playerID != -1)
        {
            //Destroy(players[playerID].GetPlayerInScene());
            //players.RemoveAt(playerID);
            players.Remove(players[playerID]);
            //Debug.Log("Remove player: " + playerID + " player remaining: " + players.Count);
        }
        else
        {
            Debug.LogError("The player# " + playerID + " was not found and it could not be removed");
        }

        // Redefine Ids and Set Host to Player 0
        for (int i = 0; i < players.Count; i++)
        {
            players[i].id = i;
            players[i].isHost = false;
        }

        // Check if the list is empty
        if(players.Count > 0)
        {
            players[0].isHost = true;
        }

    }
    public void RemovePlayer(InputDevice device)
    {
        int playerID = FindPlayer(device);
        if (playerID != -1)
        {
            Destroy(players[playerID].GetPlayerInScene());
            players.RemoveAt(playerID);
            Debug.Log("Remove player: " + playerID);
        }

        // Reasign the IDs
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetID(i);
        }
    }

    // Find Player
    private int FindPlayer(PlayerInput input)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetInput() == input)
            {
                return i;
            }
        }
        return -1;
    }
    private int FindPlayer(InputDevice device)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetDevice() == device)
            {
                return i;
            }
        }
        return -1;
    }

    // Get Player Data
    public PlayerData GetPlayerData(PlayerInput input)
    {
        int playerID = FindPlayer(input);
        if (playerID != -1)
        {
            return players[playerID];
        }
        return null;
    }
    public PlayerData GetPlayerData(int id)
    {
        //Debug.Log("Current Players PlayerDataMAnager: " + players.Count);
        if (id <= players.Count)
        {
            return players[id];
        }
        return null;
    }
    public PlayerData GetData(int id)
    {
        if (id <= players.Count)
        {
            return players[id];
        }
        return null;
    }
    public PlayerData GetPlayerData(InputDevice device)
    {
        int playerID = FindPlayer(device);
        if (playerID != -1)
        {
            return players[playerID];
        }
        return null;
    }

    public void ClearAllButHost(bool resetCosmetics)
    {
        // Find Host
        PlayerData hostPlayer = new PlayerData(-1, null, null, null, false, null);

        Debug.LogWarning(players.Count);
        foreach (var player in players)
        {
            if (player.isHost)
            {
                hostPlayer = player;
                break;
            }
        }

        players.Clear();
        players.Add(hostPlayer);

        if (resetCosmetics)
        {
            players[0].cosmeticData = new CosmeticData();
        }
    }

/*    public int GetPlayersWithInGameCharacter()
    {
        int count = 0;
        foreach (PlayerData player in players)
        {
            if (player.GetPlayerInScene() != null)
            {
                count++;
            }
        }

        return count;
    }*/
}
