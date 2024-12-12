using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer gameMixer;

    [Header("Settings")]
    [SerializeField] private float maxVolume = 0.1f;

    private Coroutine sfxTestSoundCoroutine;
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
        if (value < 0)
        {
            value *= 5;
        }
        value = Mathf.Clamp(value, -80, 0);
        ChangeAudio("MainVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        if (value < 0)
        {
            value *= 5;
        }
        value = Mathf.Clamp(value, -80, 0);
        ChangeAudio("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        if(value < 0)
        {
            value *= 5;
        }
        value = Mathf.Clamp(value, -80, 0);
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


        // Restart the timer
        if(group == "SFXVolume")
        {
            if (sfxTestSoundCoroutine != null)
            {
                StopCoroutine(sfxTestSoundCoroutine);
            }
            sfxTestSoundCoroutine = StartCoroutine(Timer());
        }
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(.5f);

        PlayTestSound();
    }
    private void PlayTestSound()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySound("Jump", this.transform);
        }
    }
}
