using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float speed = 6;

    private Vector3 direction;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;

    // New input system

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 raw;
        raw = context.ReadValue<Vector2>();
        direction = new Vector3(raw.x, 0, raw.y).normalized;
        Debug.Log(direction);
    }

    private void Update()
    {
        // Minimum Input for detection
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
       
    }
}
