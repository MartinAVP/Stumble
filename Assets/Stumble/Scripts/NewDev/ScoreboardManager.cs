using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class ScoreboardManager : MonoBehaviour
{
    [SerializeField] private GameObject scoreboardPanel;
    [SerializeField] private Transform scoreboard;
    [SerializeField] private Animator animator;
    [Space]
    [SerializeField] private GameObject CardPrefab;

    public SortedDictionary<int, PlayerData> positions = new SortedDictionary<int, PlayerData>();
    private List<Scoreboard> boards = new List<Scoreboard>();

    public static ScoreboardManager Instance;

    private void Awake()
    {
        if(Instance == null) { Instance = this; }
        scoreboardPanel.SetActive(false);
    }
/*
    public void UpdatePositions(SortedDictionary<int, PlayerData> newPositions)
    {
        positions.Clear();
        positions = newPositions;
    }

    public void UpdatePositionsFromTime(SortedDictionary<float, PlayerData> newPositions)
    {
        int index = 0;
        foreach (PlayerData data in newPositions.Values) {
            index++;
            positions.Add(index, data);
        }
    }*/

    private Dictionary<PlayerData, int> finalPositions = new Dictionary<PlayerData, int>();

    /// <summary>
    /// Update the Positions based on the time players finished in.
    /// </summary>
    /// <param name="newPositions">The positions in order the players finished in</param>
    public void UpdatePositionsFromRace(SortedDictionary<float, PlayerData> newPositions)
    {
        positions.Clear(); // Clean the Positions in the Canvas

        finalPositions = CalculateRacePositions(newPositions);
/*        finalPositions.OrderBy(x => x.Value).ToList();*/
    }
    
    /// <summary>
    /// Update the Positions based on the order that the players were eliminated in.
    /// </summary>
    /// <param name="newPositions">The positions in order of elimination</param>
    public void UpdatePositionsFromArena(SortedDictionary<int, PlayerData> newPositions)
    {
        positions.Clear(); // Clean the Positions in the Canvas

        finalPositions = CalculateArenaPositions(newPositions);
    }

    /// <summary>
    /// Takes the positions from the Race manager and converts them in a Dictionary of Positions.
    /// </summary>
    /// <param name="incomingPositions">Incoming Sorted array of Time and PlayerData</param>
    /// <returns> The sorted players in point assignment order; first - last </returns>
    public static Dictionary<PlayerData, int> CalculateRacePositions(SortedDictionary<float, PlayerData> incomingPositions)
    {
        Dictionary<PlayerData, int> finalPositions = new Dictionary<PlayerData, int>();

        var sorted = incomingPositions.OrderBy(x => x.Key).ToList();
        //Debug.Log("====== SORTED ======");
        int index2 = 0;
        foreach (var sortID in sorted)
        {
            //Debug.Log(index2 + "# with time " + sortID.Key + " is Player #" + sortID.Value.id);
            finalPositions.Add(sortID.Value, index2);
            index2++;
        }

/*        int index = 0;
        foreach (PlayerData data in incomingPositions.Values)
        {
            finalPositions.Add(data, index);
            index++;
        }*/

        return finalPositions;
    }

    /// <summary>
    /// Takes the positions from the Arena managed and converst them in a Dictionary of Positions.
    /// Note: The order that is provided is inverted since, it is given in the order that players die in.
    /// </summary>
    /// <param name="incomingPositions">The Dictionary of player Death order</param>
    /// <returns>The sorted players in point assignment order; first - last </returns>
    public static Dictionary<PlayerData, int> CalculateArenaPositions(SortedDictionary<int, PlayerData> incomingPositions)
    {
        Dictionary<PlayerData, int> finalPositions = new Dictionary<PlayerData, int>();

        var sorted = incomingPositions.OrderBy(x => x.Key).Reverse().ToList();
        // Inverted loop allowing the correct position assignation
        //int index = incomingPositions.Count - 1;
        int index = 0;
        foreach (var sortID in sorted)
        {
            finalPositions.Add(sortID.Value, index);
            index++;
        }

        return finalPositions;
    }

    public void AddMatchPoints()
    {
        // Add Points Based on Positions
        int index = 1;
        foreach (PlayerData data in finalPositions.Keys)
        {
            SetPoints(data.id, index, 0);
            index++;
        }
    }

    public void SetPoints(int id, int place, int bonus)
    {
        switch (place)
        {
            case 1:     // 1st Place
                PlayerDataHolder.Instance.GetPlayerData(id).points += 10 + bonus;
                break;
            case 2:     // 2nd Place
                PlayerDataHolder.Instance.GetPlayerData(id).points += 7 + bonus;
                break;
            case 3:     // 3rd Place
                PlayerDataHolder.Instance.GetPlayerData(id).points += 5 + bonus;
                break;
            case 4:     // 4th Place
                PlayerDataHolder.Instance.GetPlayerData(id).points += 3 + bonus;
                break;
            case 0:     // 0 Is Special Case to Add Extra points. Just bonus no place considered.
                PlayerDataHolder.Instance.GetPlayerData(id).points += bonus;
                break;
            default:
                break;
        }
    }

    public void UpdateUIPoints()
    {
        foreach (Scoreboard board in boards) {
            int points = PlayerDataHolder.Instance.GetPlayerData(board.playerData.id).points;
            board.UpdateScore(points, 2);
        }
    }

    public void DisplayScoreboard()
    {
        scoreboardPanel.SetActive(true);
        List<UICameraView> views = new List<UICameraView>();
        views = GetCharacterImages();

        //Debug.Log(finalPositions.Count + " Values");

       // Debug.Log("====== FINAL POSITIONS ======");

        int index = 0;
        foreach (PlayerData player in finalPositions.Keys) {
            //Debug.Log(index + "# is Player #" + player.id);

            GameObject card = Instantiate(CardPrefab);
            Scoreboard board = card.GetComponent<Scoreboard>();
            card.gameObject.name = "Player #" +  index + "card";

            if(index % 2 == 1)
            {
                board.InvertPanel(); // Panel Invert One Left and on Right
            }

            board.playerData = player;
            card.transform.SetParent(scoreboard);

            boards.Add(board);
            views[index].SetTextureForRenderCam(board.characterImage);

            SetLayerForEachChild(player.input.transform.parent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer, player.input.transform);

            board.UpdateValues(player.points, index + 1);
            index++;
        }
        animator.SetTrigger("PlayIn");
    }

    private List<UICameraView> GetCharacterImages()
    {
        List<UICameraView> uiCamViews = new List<UICameraView>();

        foreach(PlayerData player in finalPositions.Keys)
        {
            uiCamViews.Add(player.playerInScene.GetComponentInChildren<UICameraView>());
        }

        // Initialize
        foreach (var camView in uiCamViews)
        {
            camView.EnableCharacterCam();
        }

        return uiCamViews;
    }

    private void SetLayerForEachChild(int layerID, Transform parent)
    {
        parent.gameObject.layer = layerID;

        foreach (Transform child in parent)
        {
            SetLayerForEachChild(layerID, child);
        }
    }
}
