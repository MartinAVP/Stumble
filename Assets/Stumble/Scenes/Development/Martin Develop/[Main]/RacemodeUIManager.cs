using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacemodeUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance.StartTransition(false);
        }   
    }
}
