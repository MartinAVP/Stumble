using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, AxisState.IInputAxisProvider
{
    [HideInInspector]
    public InputAction horizontal;
    [HideInInspector]
    public InputAction vertical;
    public bool lockView = false;

    public float GetAxisValue(int axis)
    {
        // Not update the view.
        if (lockView) return 0;

        switch (axis)
        {
            case 0: return horizontal.ReadValue<Vector2>().x;
            case 1: return horizontal.ReadValue<Vector2>().y;
            case 2: return vertical.ReadValue<float>();
        }

        return 0;
    }
}
