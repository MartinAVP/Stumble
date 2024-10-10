using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackupKicker : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(waitFrames());
    }

    private IEnumerator waitFrames()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (GameController.Instance == null)
        {
            new GameObject("BackupGameController").AddComponent<GameController>();
        }

        if (PlayerDataManager.Instance == null)
        {
            new GameObject("BackupDataManager").AddComponent<PlayerDataManager>();
        }
    }
}
