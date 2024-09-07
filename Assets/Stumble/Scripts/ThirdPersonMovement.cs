using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour, IBumper
{
    // Requierements
    public CharacterController controller;
    public Transform cam;

    #region Horizontal Movement
    [Header("Movement")]
    public float accelerationSpeed = .2f;
    public float deccelerationSpeed = .8f;
    public float maxSpeed = 6;
    private Vector3 rawDirection;
    private float currentSpeed = 0;
    #endregion

    #region Bumping
    [Header("Bumping")]
    public float maxBumpSpeed = 8;
    public float bumpDrag = 5f;
    private Vector3 bumpDir;
    private float currentBumpSpeed = 0;
    #endregion

    #region Rotating
    [Header("Rotation")]
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    #endregion

    #region Jumping
    [Header("Jump & Gravity")]
    [SerializeField] private float jumpPower = 10;
    [SerializeField] private LayerMask jumpableLayers;
    [Tooltip("It has to be more half the height of the character. Recommended [0.2] more than half the height. [0.2] Allows jumping on a 45 degree surface")]
    [SerializeField] private float minJumpDistance = .2f;
    #endregion

    #region Vertical Movement
    // Gravity Variables
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float _gravity = -9.81f;
    private float _verticalVelocity = 0;
    #endregion

    private void Update()
    {
        ApplyGravity();
        Movement();
        //Bumping();
        ApplyVerticalMovement();

    }

    // Input Actions Callback Functions
    // ===========================================================================================
    #region Callback Context Functions
    public void Move(InputAction.CallbackContext context)
    {
        Vector2 raw;
        raw = context.ReadValue<Vector2>();
        rawDirection = new Vector3(raw.x, 0, raw.y).normalized;
        //Debug.Log(direction);
    }
    public void Jump(InputAction.CallbackContext context)
    {
        // Check when key is pressed once
        if (!context.started) return;

        // Check if player is on the air
        if(!isGrounded()) return;

        // Add to the Vertical Velocity Value
        _verticalVelocity += jumpPower;
    }
    #endregion

    // Updating Functions
    // ===========================================================================================
    #region Constantly Updating Functions
    /// <summary>
    /// Applies movement to the player horizontally in relation to the camera orientation.
    /// </summary>
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
    
    /// <summary>
    /// W.I.P
    /// </summary>
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

    /// <summary>
    /// Applies Gravity over time to the player, does not run the calculation if the player is grounded
    /// </summary>
    private void ApplyGravity()
    {
        if (isGrounded() && _verticalVelocity < 0f)
        {
            _verticalVelocity = 0.0f;  // Prevents the character from sinking into the ground
        }
        else
        {
            _verticalVelocity += _gravity * gravityMultiplier * Time.deltaTime;
        }
    }

    /// <summary>
    /// Uses the verticalVelocity value and applies movement to the character vertically
    /// </summary>
    private void ApplyVerticalMovement()
    {
        Vector3 fallVector = new Vector3(0, _verticalVelocity, 0);
        controller.Move(fallVector * Time.deltaTime);
    }
    #endregion

    // Internal Functions
    // ===========================================================================================
    #region Internal Functions
    /// <summary>
    /// Checks if the player is Grounded depending on the minimum jump distance.
    /// </summary>
    private bool isGrounded()
    {
        // Check if the character is grounded using a raycast
        return Physics.Raycast(transform.position, Vector3.down, out _, minJumpDistance, jumpableLayers);
    }
    #endregion

    // Interfaces
    // ===========================================================================================
    #region Interfaces
    /// <summary>
    /// W.I.P
    /// </summary>
    public void Bump(Vector3 direction, float magnitude)
    {
        currentBumpSpeed = magnitude;
        bumpDir = direction.normalized * magnitude;

        float targetAngle = Mathf.Atan2(bumpDir.x, bumpDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        bumpDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

        Debug.Log("PushBumper Triggered!");
    }
    #endregion
}
