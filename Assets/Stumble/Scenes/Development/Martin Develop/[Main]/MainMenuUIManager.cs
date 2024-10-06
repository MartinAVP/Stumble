using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Tooltip("Panel For Fading")]
    [SerializeField] private GameObject unityScreenPanel;

    private void Start()
    {
        unityScreenPanel.SetActive(true);
        if(LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.StartTransition();
        }
    }
}
