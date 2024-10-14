using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer gameMixer;

    [Header("Settings")]
    [SerializeField] private float maxVolume = 0.1f;

    public static OptionsManager Instance { get; private set; }

    private void Start()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetGeneralVolume(float value)
    {
        ChangeAudio("MainVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        ChangeAudio("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        ChangeAudio("SFXVolume", value);
    }

    public void SetTargetFPS(int value)
    {
        Application.targetFrameRate = value;
    }

    private void ChangeAudio(string group, float value)
    {
        if(value <= -35)
        {
            value = -80;
        }
        gameMixer.SetFloat(group, value);
    }
}
