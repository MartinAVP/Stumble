using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputOverhaul : MonoBehaviour
{
    public UnityEvent OnSouthPressed;

    public void SouthButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Press Send");
            OnSouthPressed.Invoke();
        }
    }
}
