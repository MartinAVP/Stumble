using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CinematicController : MonoBehaviour
{
    [SerializeField] private List<CinematicPoint> points;

    public PlayableDirector timeline;
    public Camera cinematicCamera;
    [Space]

    [Header("UI")]
    [SerializeField] private GameObject cinematicPanel;
    [SerializeField] private Image backgroundImg;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;


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
        ShowCinematicUI();

/*        foreach(PlayerData data in PlayerDataManager.Instance.GetPlayers())
        {
            data.GetPlayerInScene().GetComponent<ThirdPersonMovement>().camController.transform.GetComponent<CinemachineFreeLook>().enabled = false;
        }*/
    }

    private void ShowCinematicUI()
    {
        // Enable Panel
        if (cinematicPanel != null)
        {
            cinematicPanel.SetActive(true);
        }

        // Reset Color
        if(backgroundImg != null)
        {
            Color background = backgroundImg.color;
            background.a = 1f;
            backgroundImg.color = background;
        }

        // Set the Text
        if(ModularController.Instance != null)
        {
            Gamemode mode = ModularController.Instance.GetCurrentGamemode();
            if (title != null)
            {
                title.text = mode.title;
            }
            if(description != null)
            {
                description.text = mode.description;
            }
        }
    }

    private void initializeCameras()
    {
        foreach (var point in points)
        {
            point.camera.gameObject.SetActive(true);
        }


        cinematicCamera.gameObject.SetActive(true);
        cinematicCamera.farClipPlane = 5000.0f;
        //this.transform.GetComponentInChildren<CinemachineBrain>().gameObject.SetActive(true);
    }

    private IEnumerator TurnOffAllCameras()
    {
        yield return new WaitForSeconds(GetTimelineLenght - 1);
        //timeline.Pause();
        Debug.Log("Cinematic Controller Ended Correctly                 [Cinematic Controller]");

        StartCoroutine(FadeOut());

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

    private IEnumerator FadeOut()
    {
        // loop over 1 second backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            if(title != null)
            {
                title.color = new Color(1, 1, 1, i);
            }
            if(description != null)
            {
                description.color = new Color(1, 1, 1, i);
            }
            if(backgroundImg != null)
            {
                Color background = backgroundImg.color;
                background.a = i;
                backgroundImg.color = background;
            }

            yield return null;
        }

        cinematicPanel.SetActive(false);
    }

    public float GetTimelineLenght => (float)timeline.duration;
}
