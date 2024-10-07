using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GamemodeSelectionUIManager : MonoBehaviour
{
    [SerializeField] private Button GrandprixButton;
    [SerializeField] private Button ArenaButton;

    [Header("Video Clips")]
    [SerializeField] private VideoClip grandPrixVideo;
    [SerializeField] private VideoClip arenaVideo;
    [SerializeField] private VideoPlayer backgroundVideo;

    private void OnEnable()
    {
        GrandprixButton.GetComponent<ButtonHoverEvent>().onHover.AddListener(HoverOverGrandPrix);
        ArenaButton.GetComponent<ButtonHoverEvent>().onHover.AddListener(HoverOverArena);
    }


    private GamemodeSelected currentlySelected;

    void Start()
    {
        LoadingScreenManager.Instance.StartTransition(false);
    }

    private void HoverOverGrandPrix()
    {
        backgroundVideo.clip = grandPrixVideo;
        backgroundVideo.Play();
    }

    private void HoverOverArena()
    {
        backgroundVideo.clip = arenaVideo;
        backgroundVideo.Play();
    }

    private enum GamemodeSelected
    {
        GrandPrix,
        Arena,
    }
}
