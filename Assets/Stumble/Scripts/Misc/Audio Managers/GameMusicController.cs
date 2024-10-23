using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameMusicController : MonoBehaviour
{
    public static GameMusicController Instance { get; private set; }

    [SerializeField] private float delay;
    [SerializeField] private float delayCountdown;
    [SerializeField] private AudioSource background;
    [SerializeField] private AudioSource countdown;

    private float timeElapsed;
    private float lerpDuration;
    private float currentVolume;
    private bool lowerVolume = false;

    public bool initialized { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {

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

    private void Awake()
    {
        Singleton();
        Task Setup = setup();
    }

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (RacemodeManager.Instance == null || RacemodeManager.Instance.enabled == false || RacemodeManager.Instance.initialized == false)
        {
            // Await 5 ms and try finding it again.
            // It is made 5 seconds because it is
            // a core gameplay mechanic.
            await Task.Delay(1);
        }

        // Once it finds it initialize the scene
        Debug.Log("Initializing Game Music Manager...         [Game Music Manager]");
        //GameController.Instance.startSystems += LateStart;

        RacemodeManager.Instance.onRaceStart += InitializeGameMusic;
        RacemodeManager.Instance.onCountdownStart += InitializeCountdown;
        initialized = true;
        return;
    }

    public void OnDisable()
    {
        if (initialized)
        {
            RacemodeManager.Instance.onRaceStart -= InitializeGameMusic;
            RacemodeManager.Instance.onCountdownStart -= InitializeCountdown;
        }
    }

    private void InitializeCountdown()
    {
        //countdown.Play();
        StartCoroutine(StartCountdownSound());
    }

    private void InitializeGameMusic()
    {
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
