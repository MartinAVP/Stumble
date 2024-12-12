using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatformManager : MonoBehaviour
{
    private static MovingPlatformManager instance;

    public static MovingPlatformManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject newManagerObject = new GameObject();
                newManagerObject.name = "MovingPlatformManager";
                instance = newManagerObject.AddComponent<MovingPlatformManager>();
            }

            return instance;
        }
    }

    private void Update()
    {
        if(Time.timeScale == 0)
        {
            return;
        }
        MovingPlatformEventBus.Publish(MovingPlatformEvent.PreMove);
        MovingPlatformEventBus.Publish(MovingPlatformEvent.Move);
        MovingPlatformEventBus.Publish(MovingPlatformEvent.PostMove);
        MovingPlatformEventBus.Publish(MovingPlatformEvent.Final);
    }
}
