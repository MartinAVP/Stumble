using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
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

        Debug.Log(positions.Count + " Values");

        int index = 0;
        foreach (PlayerData player in positions.Values) {
            GameObject card = Instantiate(CardPrefab);
            Scoreboard board = card.GetComponent<Scoreboard>();

            if(index % 2 == 1)
            {
                board.InvertPanel(); // Panel Invert
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

        foreach (PlayerData player in PlayerDataHolder.Instance.GetPlayers())
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
