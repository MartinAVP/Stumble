using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIReconnectManager : MonoBehaviour
{
    public GameObject ReconnectScreen;
    public TextMeshProUGUI disconnectedPlayersText;

    public List<int> disconnectedPlayers = new List<int>();

    private void OnEnable()
    {
        PlayerDataManager.Instance.onPlayerInputDeviceDisconnect += OnPlayerDisconnected;
        PlayerDataManager.Instance.onPlayerInputDeviceReconnect += OnPlayerReconnected;
    }

    private void Start()
    {
        ReconnectScreen.SetActive(false);
    }

    private void OnPlayerDisconnected(PlayerData data)
    {
        disconnectedPlayers.Add(data.id);

        disconnectedPlayersText.text = buildPlayersString();
        toggleScreen();
    }

    private void OnPlayerReconnected(PlayerData data)
    {
        disconnectedPlayers.Remove(data.id);

        disconnectedPlayersText.text = buildPlayersString();
        toggleScreen();
    }

    private string buildPlayersString()
    {
        string display = "";
        for (int i = 0; i < disconnectedPlayers.Count; i++)
        {
            display += "Player #" + disconnectedPlayers[i];
            if (disconnectedPlayers[i] != disconnectedPlayers[disconnectedPlayers.Count - 1])
            {
                display += " ,";
            }
        }

        return display;
    }

    private void toggleScreen()
    {
        if (disconnectedPlayers.Count > 0)
        {
            ReconnectScreen.SetActive(true);
        }
        else
        {
            ReconnectScreen.SetActive(false);
        }
    }
}
