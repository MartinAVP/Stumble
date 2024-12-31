using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    private bool istransitioning = false;

    public void GoBackToMenu()
    {
        if (istransitioning) { return; }
        istransitioning = true;

        StartCoroutine(GoBack());
    }

    private IEnumerator GoBack()
    {
        if (LoadingScreenManager.Instance != null) { LoadingScreenManager.Instance.StartTransition(true); }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Menu");
    }
}
