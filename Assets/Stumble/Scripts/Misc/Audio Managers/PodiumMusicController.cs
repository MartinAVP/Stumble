using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PodiumMusicController : MonoBehaviour
{
    public static PodiumMusicController Instance { get; private set; }

    [SerializeField] private float delay;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip winning;
    [SerializeField] private AudioClip winningLoop;

    private float timeElapsed;
    private float lerpDuration;
    private float currentVolume;
    private bool lowerVolume = false;

    // Start is called before the first frame update
    void Start()
    {
        Singleton();
        StartCoroutine(StartPodiumMusic());
    }

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

    private IEnumerator StartPodiumMusic()
    {
        yield return new WaitForSeconds(delay);
        source.clip = winning;
        source.Play();
        yield return new WaitForSeconds(winning.length - .5f);
        AudioSource newSource = this.AddComponent<AudioSource>();
        newSource.clip = winningLoop;
        newSource.Play();
        yield return new WaitForSeconds(.5f);
        source.Pause();
    }

    public void EndMusic(float time)
    {
        currentVolume = source.volume;
        lerpDuration = time;
        lowerVolume = true;
    }

    void Update()
    {
        if (lowerVolume)
        {
            if (timeElapsed < lerpDuration)
            {
                source.volume = Mathf.Lerp(currentVolume, 0, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
            }
            else
            {
                source.volume = 0;
            }
        }
    }
}
