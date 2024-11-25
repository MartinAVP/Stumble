using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlScheme : MonoBehaviour
{
    public static ControlScheme Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI controlScheme;
    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;

    private int navID = 0;
    private bool transitioning;

    [SerializeField] List<Scheme> schemes;

    // Singleton
    private void Awake()
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

    public void OpenControls()
    {
        navID = 0;

        foreach (Scheme scheme in schemes)
        {
            scheme.panel.gameObject.SetActive(false);
        }

        if(schemes.Count > 0)
        {
            controlScheme.text = schemes[0].name;
            schemes[0].panel.localPosition = Vector3.zero;
            schemes[0].panel.gameObject.SetActive(true);
        }
    }

    public void NavRight()
    {
        if (transitioning) { return; }
        transitioning = true;

        int originID = navID;
        navID++;
        if (navID >= schemes.Count)
        {
            navID = 0;
        }

        controlScheme.text = schemes[navID].name;
        GamemodeSelectScreenManager.Instance.InterpolateScreens(schemes[originID].panel.gameObject, schemes[navID].panel.gameObject, GamemodeSelectScreenManager.Direction.Left);
        StartCoroutine(delayPress());
    }

    public void NavLeft()
    {
        if (transitioning) { return; }
        transitioning = true;

        int originID = navID;
        navID--;
        if (navID < 0)
        {
            navID = schemes.Count - 1;
        }

        controlScheme.text = schemes[navID].name;
        GamemodeSelectScreenManager.Instance.InterpolateScreens(schemes[originID].panel.gameObject, schemes[navID].panel.gameObject, GamemodeSelectScreenManager.Direction.Right);
        StartCoroutine(delayPress());
    }

    public IEnumerator delayPress()
    {
        yield return new WaitForSeconds(1.1f);
        transitioning = false;
    }

    public void CloseControls()
    {
        navID = 0;
    }
}

[System.Serializable]
public struct Scheme
{
    [SerializeField] public Transform panel;
    [SerializeField] public string name;
}