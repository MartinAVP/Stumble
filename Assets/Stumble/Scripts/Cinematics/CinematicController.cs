using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicController : MonoBehaviour
{
    [SerializeField] private List<CinematicPoint> points;

    public PlayableDirector timeline;
    public Camera cinematicCamera;

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


/*    private void OnEnable()
    {
        timeline = GetComponentInChildren<PlayableDirector>();
    }*/

    private void Start()
    {
        foreach (var point in points)
        {
            point.camera.m_LookAt = point.focus;
        }
    }

    public void StartTimeline()
    {
        initializeCameras();

        timeline.Play();
        StartCoroutine(TurnOffAllCameras());

/*        foreach(PlayerData data in PlayerDataManager.Instance.GetPlayers())
        {
            data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().camController.transform.GetComponent<CinemachineFreeLook>().enabled = false;
        }*/
    }

    private void initializeCameras()
    {
        foreach (var point in points)
        {
            point.camera.gameObject.SetActive(true);
        }


        cinematicCamera.gameObject.SetActive(true);
        //this.transform.GetComponentInChildren<CinemachineBrain>().gameObject.SetActive(true);
    }

    private IEnumerator TurnOffAllCameras()
    {
        yield return new WaitForSeconds(GetTimelineLenght - 1);
        //timeline.Pause();
        Debug.Log("Cinematic Controller Ended Correctly                 [Cinematic Controller]");

        foreach (var point in points)
        {
            point.camera.gameObject.SetActive(false);
        }

        /*        for (int i = 0; i < points.Count - 1; i++)
                {
                    points[i].camera.enabled = false;
                }*/


        //this.GetComponentInChildren<CinemachineBrain>().gameObject.SetActive(false);
        foreach (Transform child in this.transform)
        {
            if (child.GetComponent<Camera>() != null)
            {
                child.gameObject.SetActive(false);
                break;
            }

            /*        foreach (PlayerData data in PlayerDataManager.Instance.GetPlayers())
                    {
                        data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().camController.transform.GetComponent<CinemachineFreeLook>().enabled = true;
                    }*/
        }
    }
    public float GetTimelineLenght => (float)timeline.duration;
}
