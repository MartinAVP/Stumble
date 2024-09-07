using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour, IBumper
{
    public CharacterController controller;
    public Transform cam;
    private Rigidbody rigidbody;

    [Header("Movement")]
    public float accelerationSpeed = .2f;
    public float deccelerationSpeed = .8f;
    public float maxSpeed = 6;
    private Vector3 rawDirection;
    [SerializeField] float currentSpeed = 0;

    [Header("Bumping")]
    public float maxBumpSpeed = 8;
    public float bumpDrag = 5f;
    private Vector3 bumpDir;
    [SerializeField] private float currentBumpSpeed = 0;

    [Header("Rotation")]
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    // Gravity Variables
    private float _gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float _velocity;

    [SerializeField] private float jumpPower = 10;

    // New input system

    private void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    private bool IsGrounded() => controller.isGrounded;

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 raw;
        raw = context.ReadValue<Vector2>();
        rawDirection = new Vector3(raw.x, 0, raw.y).normalized;
        //Debug.Log(direction);
    }
    public void Jump(InputAction.CallbackContext context)
    {
        // Check when key is down
        if (!context.started) return;
        Debug.Log("Jump1");
/*        if(!IsGrounded()) return;
        Debug.Log("Jump");*/

        _velocity += jumpPower / 3;
    }

    private void Update()
    {
        Jumping();
        ApplyGravity();
        Movement();
        Bumping();
    }

    // IBumper Interface Implementation
    public void Bump(Vector3 direction, float magnitude)
    {
        currentBumpSpeed = magnitude;
        bumpDir = direction.normalized * magnitude;

        float targetAngle = Mathf.Atan2(bumpDir.x, bumpDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        bumpDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

        Debug.Log("PushBumper Triggered!");
    }
    private void Movement()
    {
        float targetAngle = Mathf.Atan2(rawDirection.x, rawDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        // Minimum Input for detection
        if (rawDirection.magnitude >= 0.1f)
        {
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            transform.rotation = Quaternion.Euler(0, angle, 0);

            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }

            currentSpeed += accelerationSpeed * moveDir.magnitude * Time.deltaTime;

            //Debug.Log(direction.magnitude);
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            //transform.rotation = Quaternion.Euler(0, targetAngle, 0);

            currentSpeed -= (deccelerationSpeed * 2) * moveDir.magnitude * Time.deltaTime;
            if (currentSpeed <= 0.05f)
            {
                currentSpeed = 0;
            }
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
            //currentSpeed = 0;
        }
    }
    private void Bumping()
    {

        //transform.rotation = Quaternion.Euler(0, angle, 0);
        currentBumpSpeed -= (bumpDrag * 2) * bumpDir.magnitude * Time.deltaTime;

        if (currentBumpSpeed > maxBumpSpeed)
        {
            currentBumpSpeed = maxSpeed;
        }

        if (currentBumpSpeed <= 0.05f)
        {
            currentBumpSpeed = 0;
        }
        controller.Move((bumpDir.normalized) * currentBumpSpeed * Time.deltaTime);
    }
    private void ApplyGravity()
    {
        if (IsGrounded())
        {
            if (_velocity < 0.0f)
            {
                _velocity = 0.0f;  // Prevents the character from sinking into the ground
            }
        }
        else
        {
            _velocity += _gravity * gravityMultiplier * Time.deltaTime;
        }

        Vector3 gravityMovement = new Vector3(0, _velocity, 0);
        controller.Move(gravityMovement * Time.deltaTime);
    }

    private void Jumping()
    {
        Vector3 jumpMovement = new Vector3(0, _velocity, 0);
        controller.Move(jumpMovement * Time.deltaTime);
    }

/*    private void Func()
    {
        Vector3 collide = Vector3.one; // (1, 1, 1);
        Transform potato;
        potato.position = new Vector3(x, y, z);
        float math = collide.x + collide.y;

        List<Vector3> corners = new List<Vector3>();


    }*/

}
