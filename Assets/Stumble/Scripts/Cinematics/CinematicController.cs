using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicController : MonoBehaviour
{
    [SerializeField] private List<CinematicPoint> points;

    public PlayableDirector timeline;

    public static CinematicController Instance { get; private set; }

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


    private void OnEnable()
    {
        timeline = GetComponentInChildren<PlayableDirector>();
    }

    private void Start()
    {
        foreach (var point in points)
        {
            point.camera.m_LookAt = point.focus;
        }
    }

    public void StartTimeline()
    {
        timeline.Play();
        StartCoroutine(TurnOffAllCameras());
    }

    private IEnumerator TurnOffAllCameras()
    {
        yield return new WaitForSeconds(GetTimelineLenght);
        foreach (var point in points)
        {
            point.camera.gameObject.SetActive(false);
        }
        
        this.GetComponentInChildren<Camera>().gameObject.SetActive(false);
    }

    public float GetTimelineLenght => (float)timeline.duration;
}
