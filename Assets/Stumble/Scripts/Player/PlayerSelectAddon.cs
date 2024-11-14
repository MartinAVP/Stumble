using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerSelectAddon : MonoBehaviour
{
    [HideInInspector]public UnityEvent<Vector2, PlayerInput> OnSelectInput;
    public void Select(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 value = context.ReadValue<Vector2>();
            Debug.Log(value); 
            OnSelectInput?.Invoke(value, this.GetComponent<PlayerInput>());

            if (CosmeticManager.Instance != null)
            {
                CosmeticManager.Instance.MoveCosmetic(value, this.GetComponent<PlayerInput>());
            }
        }
    }
}
