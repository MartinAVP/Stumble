using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    private Rigidbody rigidbody;

    public float speed = 6;
    public float moveAcceleration;

    [Header("New Movement")]
    public float currentSpeed = 0;
    public float accelerationSpeed = .2f;
    public float deccelerationSpeed = .8f;
    public float maxSpeed = 6;

    private Vector3 direction;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;

    // New input system

    private void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 raw;
        raw = context.ReadValue<Vector2>();
        direction = new Vector3(raw.x, 0, raw.y).normalized;
        //Debug.Log(direction);
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

            if(currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }

            currentSpeed += accelerationSpeed * moveDir.magnitude * Time.deltaTime;

            Debug.Log(direction.magnitude);
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }
        else
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0, angle, 0);
            //Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            Vector3 moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            currentSpeed -= (deccelerationSpeed * 2) * moveDir.magnitude * Time.deltaTime;
            if(currentSpeed <= 0.05f)
            {
                currentSpeed = 0;
            }
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
            //currentSpeed = 0;
        }
       
        //velocity = rigidbody.velocity.magnitude;
    }


}
