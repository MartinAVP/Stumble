using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicPlayerMovement : MonoBehaviour
{
    private Vector2 direction;

    public void Move(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
        Debug.Log(direction);
    }

    public void FixedUpdate()
    {
        PlayerMovement(direction);
    }

    public void PlayerMovement(Vector2 direction)
    {
        // Convert Vector 2 to Vector 3
        this.transform.position += new Vector3(direction.x, 0, direction.y);
    }
}
