using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Announcer : MonoBehaviour
{
    public void DebugValue(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.transform.position += Vector3.up;
        }
        else if (context.canceled)
        {
            this.transform.position -= Vector3.up;
        }
    }
}
