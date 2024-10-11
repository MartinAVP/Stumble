using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatformManager : MonoBehaviour
{
    private void Update()
    {
        MovingPlatformEventBus.Publish(MovingPlatformEvent.PreMove);
        MovingPlatformEventBus.Publish(MovingPlatformEvent.Move);
        MovingPlatformEventBus.Publish(MovingPlatformEvent.PostMove);
        MovingPlatformEventBus.Publish(MovingPlatformEvent.Final);
    }
}
