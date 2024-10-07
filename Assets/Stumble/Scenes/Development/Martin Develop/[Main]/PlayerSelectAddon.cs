using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelectAddon : MonoBehaviour
{
    private event Action<Vector2, InputDevice> OnSelectInput;
    public void Select(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 value = context.ReadValue<Vector2>();
            OnSelectInput?.Invoke(value, context.action.activeControl.device);

            if (CosmeticManager.Instance != null)
            {
                CosmeticManager.Instance.MoveCosmetic(value, this.GetComponent<PlayerInput>());
            }
        }
    }
}
