using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameMusicController : MonoBehaviour
{
    public static GameMusicController Instance { get; private set; }

    [SerializeField] private float delay;
    [SerializeField] private float delayCountdown;
    [Space]
    [SerializeField] private AudioSource background;
    [SerializeField] private AudioSource countdown;
    [Space]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip countdownSound;

    private float timeElapsed;
    private float lerpDuration;
    private float currentVolume;
    private bool lowerVolume = false;

    public bool initialized { get; private set; } = true;

    // Start is called before the first frame update

    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Awake()
    {
        Singleton();
    }

    public void InitializeCountdown()
    {
        countdown.clip = countdownSound;
        StartCoroutine(StartCountdownSound());
    }

    public void InitializeGameMusic()
    {
        background.clip = backgroundMusic;
        StartCoroutine(StartGameMusic());
    }

    private IEnumerator StartCountdownSound()
    {
        yield return new WaitForSeconds(delayCountdown);
        countdown.Play();
    }

    private IEnumerator StartGameMusic()
    {
        yield return new WaitForSeconds(delay);
        background.Play();
    }

    public void EndMusic(float time)
    {
        currentVolume = background.volume;
        lerpDuration = time;
        lowerVolume = true;
    }

    void Update()
    {
        if (lowerVolume)
        {
            if (timeElapsed < lerpDuration)
            {
                background.volume = Mathf.Lerp(currentVolume, 0, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
            }
            else
            {
                background.volume = 0;
            }
        }
    }
}
