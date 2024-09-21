using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.VisualScripting;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour, IBumper
{
    // Requierements
    public CharacterController controller;
    public Transform camController;
    public Transform cam;

    #region Horizontal Movement
    [Header("Movement")]
    [SerializeField] private float accelerationSpeed = 10f;
    [SerializeField] private float deccelerationSpeed = 4f;
    [SerializeField] private float maxSpeed = 10;
    private Vector3 rawDirection;
    private float _horizontalVelocity = 0;
    #endregion

    #region Bumping
    [Header("Bumping")]
    [SerializeField] private float maxBumpSpeed = 2;
    [SerializeField] private float bumpDrag = .2f;
    [Space]
    [SerializeField] private float bumpForce = 20f;
    [Range(0f, 5f)][SerializeField] private float bumpUpwardForce = .2f;
    private Vector3 bumpDir = Vector3.zero;
    private float currentBumpSpeed = 0;
    #endregion

    #region Rotating
    [Header("Rotation")]
    [SerializeField] private float turnSmoothTime = 0.1f;
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
    [Space]
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float _gravity = -9.81f;
    private float _verticalVelocity = 0;
    #endregion

    #region Camera Controls
    [Header("Camera Controls")]
    // Note: These values will only update when starting the game, not while the scene is playing.
    [Tooltip("Default: 5")]
    [SerializeField] private float baseVerticalViewSensitivity = 5.0f;
    [SerializeField] private bool baseInvertVerticalInput = false;
    [Space]
    [Tooltip("Default: 300")]
    [SerializeField] private float baseHorizontalViewSensitivity = 300.0f;
    [SerializeField] private bool baseInvertHorizontalInput = false;
    private CinemachineFreeLook freelookcam;
    #endregion

    #region Diving
    [Header("Diving")]
    [SerializeField] private float diveForce = 2;
    [SerializeField] private float diveDragMultiplier = 1f;
    [HideInInspector]public bool isProne = false;
    private float playerHeight = 2;
    [SerializeField] private bool rotateModelonDive = true;
    #endregion

    [Header("Debug")]
    // Lock Movement is made for testing bumping
    // If left unchecked, 2 characters will move with the same input
    public bool lockMovement = false;

    private void Awake()
    {
        freelookcam = camController.gameObject.GetComponent<CinemachineFreeLook>();
    }

    private void Start()
    {
        // Update Sensitivity
        updateSensitivity(baseVerticalViewSensitivity, baseInvertVerticalInput, baseHorizontalViewSensitivity, baseInvertHorizontalInput);

        // Set the player Height
        controller.height = playerHeight;
    }

    private void Update()
    {
        ApplyGravity();
        ApplyVerticalMovement();
        Movement();
        Bumping();
    }

    // Input Actions Callback Functions
    // ===========================================================================================
    #region Player Input Functions
    public void Move(InputAction.CallbackContext context)
    {
        Vector2 raw;
        raw = context.ReadValue<Vector2>();
        rawDirection = new Vector3(raw.x, 0, raw.y).normalized;
        //Debug.Log(direction);
        if (lockMovement)
        {
            rawDirection = Vector3.zero;
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (lockMovement) return;

        // Check when key is pressed once
        if (!context.started) return;

        // Check if player is prone, unprone if proned;
        if (isProne) { toggleProne(false); return; }

        // Check if player is on the air
        if (!isGrounded()) return;

        // Add to the Vertical Velocity Value
        _verticalVelocity = 0;
        _verticalVelocity += jumpPower;
    }

    public void Dive(InputAction.CallbackContext context)
    {
        if(lockMovement) return;

        //Debug.Log("Dive");
        // Change Model
        if (context.started)
        {
            // Prevent Proning when already in prone
            if (isProne) { return; }
            toggleProne(true);
        }
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
        // Direction Vector
        Vector3 moveDir;

        // Check for proning state, prevents horizontal movement and character rotation
        if (isProne)
        {
            moveDir = transform.forward.normalized;
            Debug.DrawRay(this.transform.position, Quaternion.Euler(0, this.transform.rotation.z, 0) * transform.forward.normalized, Color.yellow);

            _horizontalVelocity -= (deccelerationSpeed * 2) * moveDir.magnitude * diveDragMultiplier * Time.deltaTime;
            if (_horizontalVelocity <= 0.05f)
            {
                _horizontalVelocity = 0;
            }

            controller.Move(moveDir.normalized * _horizontalVelocity * Time.deltaTime);
            return;
        }

        // Camera Angle Logic Calculation
        float targetAngle = Mathf.Atan2(rawDirection.x, rawDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        if (rawDirection.magnitude >= 0.1f)
        {
            moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            transform.rotation = Quaternion.Euler(0, angle, 0);

            if (_horizontalVelocity > maxSpeed)
            {
                _horizontalVelocity = maxSpeed;
            }

            _horizontalVelocity += accelerationSpeed * moveDir.magnitude * Time.deltaTime;

            controller.Move(moveDir.normalized * _horizontalVelocity * Time.deltaTime);
        }

        // No player input
        // Decellerate the player based on multiplier
        else
        {
            moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            _horizontalVelocity -= (deccelerationSpeed * 2) * moveDir.magnitude * Time.deltaTime;
            if (_horizontalVelocity <= 0.05f)
            {
                _horizontalVelocity = 0;
            }
            controller.Move(moveDir.normalized * _horizontalVelocity * Time.deltaTime);
        }
    }

    /// <summary>
    /// Apply Bumping to the player, not affected by player movement
    /// </summary>
    private void Bumping()
    {
        if(currentBumpSpeed <= 0) { return; }
        currentBumpSpeed -= bumpDrag * bumpDir.magnitude * Time.deltaTime;

        if (currentBumpSpeed > maxBumpSpeed + 0.1f)
        {
            currentBumpSpeed = maxBumpSpeed;
        }

        if (currentBumpSpeed <= 0.05f)
        {
            currentBumpSpeed = 0;
        }
        controller.Move(bumpDir * currentBumpSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Applies Gravity over time to the player, does not run the calculation if the player is grounded
    /// </summary>
    private void ApplyGravity()
    {
        // Prone Logic
        if (isProne)
        {
            if (!isGrounded())
            {
                _verticalVelocity += _gravity * gravityMultiplier * Time.deltaTime;
            }

            return;
        }

        if (isGrounded() && _verticalVelocity < -5f)
        {
            _verticalVelocity = -5.0f;  // Prevents the character from sinking into the ground
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
        // Is grounded Raycast changes the origin based on proning state
        if (!isProne)
        {
            //Debug.DrawRay(transform.position - new Vector3(0, playerHeight, 0), Vector3.down * 10, Color.white);
            return Physics.Raycast(transform.position, Vector3.down, out _, (playerHeight / 2) + .2f, jumpableLayers);
        }
        else
        {
            //Debug.DrawRay(transform.position - new Vector3(0, playerHeight, 0), Vector3.down * 10, Color.green);
            return Physics.Raycast(transform.position - new Vector3(0, playerHeight, 0), Vector3.down, out _, (playerHeight / 2) + .2f, jumpableLayers);
        }

    }

    private void updateSensitivity(float vertical, bool invertVertical, float horizontal, bool invertHorizontal)
    {
        freelookcam.m_YAxis.m_MaxSpeed = vertical;
        freelookcam.m_XAxis.m_MaxSpeed = horizontal;

        freelookcam.m_YAxis.m_InvertInput = invertVertical;
        freelookcam.m_XAxis.m_InvertInput = invertHorizontal;
    }

    private void toggleProne(bool activate)
    {
        if (activate)
        {
            _horizontalVelocity += diveForce;
            if(_horizontalVelocity > maxSpeed + diveForce)
            {
                _horizontalVelocity = maxSpeed + diveForce;
            }

            isProne = true;
            if (rotateModelonDive)
            {
                this.transform.GetChild(0).transform.Rotate(90, 0, 0);
                this.transform.GetChild(0).transform.position += new Vector3(0, -0.5f, 0);
            }
            this.GetComponent<CapsuleCollider>().center = new Vector3(0, -0.5f, 0);
            // Capsule Collider Direction Horizontal
            this.GetComponent<CapsuleCollider>().direction = 2;

            // Make Character controller to a ball
            controller.height = 1;
            controller.center = new Vector3(0, -0.5f, 0);
            //controller.height = 1;

            playerHeight = controller.height;
        }
        else
        {
            isProne = false;
            if (rotateModelonDive)
            {
                this.transform.GetChild(0).transform.Rotate(-90, 0, 0);
                this.transform.GetChild(0).transform.position += new Vector3(0, 0.5f, 0);
            }
            this.GetComponent<CapsuleCollider>().center = new Vector3(0, 0, 0);
            // Capsule Collider Vertical
            this.GetComponent<CapsuleCollider>().direction = 1;
            // Make Character controller to a ball
            controller.height = 2;
            controller.center = new Vector3(0, 0, 0);

            playerHeight = controller.height;
        }
    }
    #endregion

    // Interfaces
    // ===========================================================================================
    #region Interfaces
    /// <summary>
    /// Will update the values of the bumping
    /// Note: Bumping is a seperate player movement vector than speed. This is made so that the player can
    /// counter a bumper with their own speed.
    /// Note: Current player speed does not affect the magnitude of the bump 
    /// </summary>
    public void Bump(Vector3 direction, float magnitude)
    {
        _verticalVelocity = 0;
        _horizontalVelocity = 0;
        currentBumpSpeed = magnitude;
        bumpDir = direction.normalized * magnitude;
    }
    #endregion

    // Collisions
    // ===========================================================================================
    #region Collisions
    /// <summary>
    /// Check for the collision of the CharacterController with another one.
    /// </summary>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Player") {
            if (isProne)
            {
                Debug.Log("Bump!");
                IBumper targetBumper = hit.gameObject.GetComponent<IBumper>();
                if (targetBumper != null)
                {
                    Vector3 bumpDirection = transform.forward.normalized + new Vector3(0, bumpUpwardForce, 0);
                    float bumpMagnitude = bumpForce;

                    targetBumper.Bump(bumpDirection, bumpMagnitude);
                }
            }
        }
    }
    #endregion

}
