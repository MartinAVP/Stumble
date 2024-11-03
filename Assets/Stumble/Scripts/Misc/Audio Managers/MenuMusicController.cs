using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    public static MenuMusicController Instance { get; private set; }

    [SerializeField] private float delay;
    [SerializeField] private AudioSource source;

    private float timeElapsed;
    private float lerpDuration;
    private float currentVolume;
    private bool lowerVolume = false;

    // Start is called before the first frame update
    void Start()
    {
        Singleton();
        StartCoroutine(StartMenuMusic());
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
            DontDestroyOnLoad(this);
        }
    }

    private IEnumerator StartMenuMusic()
    {
        yield return new WaitForSeconds(delay);
        source.Play();
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
