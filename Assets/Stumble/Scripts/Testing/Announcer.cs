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

    public void MoveCosmetic(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        //Debug.Log(value);
        PlayerInput playerInput = this.GetComponent<PlayerInput>();
        PlayerData data = PlayerDataManager.Instance.GetPlayerData(playerInput);
        CosmeticManager.Instance.ChangeCosmetic(value, data);
    }

    private enum SelectedChange
    {
        Color,
    }
}
