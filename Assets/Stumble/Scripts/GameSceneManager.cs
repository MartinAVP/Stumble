using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private bool isLobby = false;

    public static GameSceneManager Instance { get; private set; }

    // Singleton
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PlayerDataManager.Instance.isLobby = isLobby;
    }

    public bool GetLobby()
    {
        return isLobby;
    }
}
