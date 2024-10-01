using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatformManager : MonoBehaviour
{
    [SerializeField] public List<MovingPlatform> movingParts = new List<MovingPlatform>();

    private void Update()
    {
        foreach (var part in movingParts)
        {
            part.UpdatePreviousPosition();
            part.UpdatePreviousRotation();
            part.Move();
        }
    }

    public void AddMovingPlatform(MovingPlatform platform)
    {
        movingParts.Add(platform);
    }
}
