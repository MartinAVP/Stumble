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
    public PlayerMovement playerMovementSettings;

    #region Horizontal Movement
    [Header("Movement")]
    private float accelerationSpeed = 10f;
    private float deccelerationSpeed = 4f;
    private float airDragMultiplier = 1f;
    private float maxSpeed = 10;
    private Vector3 rawDirection;
    [HideInInspector]public float horizontalVelocity = 0;
    private bool _grounded = false;
    #endregion

    #region Bumping
    [Header("Bumping")]
    private float bumpForce = 20f;
    private float bumpUpwardForce = .2f;
    private Vector3 _bumpHorizontalVelocity = Vector3.zero;
    #endregion

    #region Rotating
    [Header("Rotation")]
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    #endregion

    #region Jumping
    [Header("Jump & Gravity")]
    private float jumpPower = 10;
    private LayerMask jumpableLayers;
    private float minJumpDistance = .2f;
    private LayerMask jumpableLayersMinusPlayer;  // < Important for making the physics ignore the player this is attached to.
    #endregion

    #region Vertical Movement
    // Gravity Variables
    [Space]
    private float gravityMultiplier = 3.0f;
    private float _gravity = -9.81f;
    private float _verticalVelocity = 0;
    #endregion

    #region Camera Controls
    [Header("Camera Controls")]
    // Note: These values will only update when starting the game, not while the scene is playing.
/*    [Tooltip("Default: 5")]*/
    private float baseVerticalViewSensitivity = 5.0f;
    private bool baseInvertVerticalInput = false;
    [Space]
/*    [Tooltip("Default: 300")]*/
    private float baseHorizontalViewSensitivity = 300.0f;
    private bool baseInvertHorizontalInput = false;

    private CinemachineFreeLook freelookcam;
    #endregion

    #region Diving
    [Header("Diving")]
    private float diveForce = 2;
    private float diveDragMultiplier = 1f;
    private float diveAirDragMultiplier = 1f;
    [HideInInspector]public bool isProne = false;
    private float playerHeight = 2;
    private float playerRadius = 0.5f;
    private bool diveWasCanceled = false;
    #endregion

    #region Base
    private MovingPlatformData currentBase;
    #endregion

    [Header("Debug")]
    // Lock Movement is made for testing bumping
    // If left unchecked, 2 characters will move with the same input
    [SerializeField] private bool rotateModelonDive = true;
    public bool lockMovement = false;
    public bool lockVeritcalMovement = false;

    private void OnEnable()
    {
        if(playerMovementSettings == null)
        {
            Debug.LogError("All the variables were changed to default due to the third person controller not having a player card attached");
        }

        // Horizontal Movement
        accelerationSpeed = playerMovementSettings.accelerationSpeed;
        deccelerationSpeed = playerMovementSettings.deccelerationSpeed;
        airDragMultiplier = playerMovementSettings.airDragMultiplier;
        maxSpeed = playerMovementSettings.maxSpeed;

        // Bumping
        bumpForce = playerMovementSettings.bumpForce;
        bumpUpwardForce = playerMovementSettings.bumpUpwardForce;

        // Roatation
        turnSmoothTime = playerMovementSettings.turnSmoothTime;

        // Jumping
        jumpPower = playerMovementSettings.jumpPower;
        jumpableLayers = playerMovementSettings.jumpableLayers;
        minJumpDistance = playerMovementSettings.minJumpDistance;

        // Vertical Movement
        gravityMultiplier = playerMovementSettings.gravityMultiplier;

        // Camera Controls
        baseVerticalViewSensitivity = playerMovementSettings.baseVerticalViewSensitivity;
        baseInvertVerticalInput = playerMovementSettings.baseInvertVerticalInput;

        baseHorizontalViewSensitivity = playerMovementSettings.baseHorizontalViewSensitivity;
        baseInvertHorizontalInput = playerMovementSettings.baseInvertHorizontalInput;

        // Diving
        diveForce = playerMovementSettings.diveForce;
        diveDragMultiplier = playerMovementSettings.diveDragMultiplier;
        diveAirDragMultiplier = playerMovementSettings.diveAirDragMultiplier;

    }

    private void Awake()
    {
        freelookcam = camController.gameObject.GetComponent<CinemachineFreeLook>();
        //this.GetComponent<PlayerInput>().camera = cam.GetComponent<Camera>();
    }

    private void Start()
    {
        // Update Sensitivity
        updateSensitivity(baseVerticalViewSensitivity, baseInvertVerticalInput, baseHorizontalViewSensitivity, baseInvertHorizontalInput);

        // Set the player Height
        controller.height = playerHeight;

        //Application.targetFrameRate = 20;

        controller.detectCollisions = false;

        // Remove Player from Calculating Physics with the world
        jumpableLayersMinusPlayer = jumpableLayers &= ~(1 << this.gameObject.layer);
    }


    private void Update()
    {
        Movement();
        ApplyGravity();
        ApplyVerticalMovement();


        MoveWithBase();
    }

    private void FixedUpdate()
    {
        isGrounded();
        if (diveWasCanceled)
        {
            if (isGrounded())
            {
                diveWasCanceled = false;
            }
        }

        isFloored = isGrounded();
    }

    public bool isFloored;

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
        if (isProne) {
            //Debug.Log("Un Toggle Prone");
            toggleProne(false);
            return; 
        }

        // Check if player is on the air
        if (!isGrounded()) return;

        // Add to the Vertical Velocity Value
        _verticalVelocity = 0;
        _verticalVelocity += jumpPower;
    }

    public void Dive(InputAction.CallbackContext context)
    {
        if(lockMovement) return;

        // Prevent Diving when on the ground
        //if (isGrounded()) return;

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
    /// Selects and updates the current movement state.
    /// </summary>
    private void Movement()
    {
        // Check for proning state, prevents horizontal movement and character rotation
        if (isProne)
        {
            ProneMovementState();
        }
        else
        {
            WalkingMovementState();
        }

        // Slide Prototype Logic - Not Working.
/*        Vector3 slideVector = groundedVector;
        // Not on a surface
        if (slideVector == Vector3.zero)
        {

            slideVelocity -= (accelerationSpeed / 2) * slideVector.normalized * Time.deltaTime;
            Debug.Log(slideVelocity);
        }
        else // Is not on a slanted surface
        {
            slideVelocity += (accelerationSpeed / 2) * slideVector.normalized * Time.deltaTime;
        }
        slideVector = slideVector * currentSlideVelocity;

        controller.Move(slideVelocity * Time.deltaTime);*/
    }

    /// <summary>
    /// Applies movement fixed to the direction the player dived in.
    /// </summary>
    private void ProneMovementState()
    {
        // Direction Vector
        Vector3 moveDir = transform.forward.normalized;

        // Composite movement
        Vector3 inputVelocity;
        Vector3 finalVelocity;

        // Braking 
        float actualBraking;

        moveDir = transform.forward.normalized;
        Debug.DrawRay(this.transform.position, Quaternion.Euler(0, this.transform.rotation.z, 0) * transform.forward.normalized * 2, Color.yellow);

        // Apply Extra Drag Multiplier if the player is grounded
        if (isGrounded())
        {
            actualBraking = (deccelerationSpeed * 2) * diveDragMultiplier * Time.deltaTime;
            //Debug.Log("Extra Dive Drag");
        }
        else
        {
            actualBraking = (deccelerationSpeed * 2) * diveDragMultiplier * diveAirDragMultiplier * Time.deltaTime;
            //Debug.Log("Normal Dive Drag");
        }

        horizontalVelocity -= actualBraking;
        _bumpHorizontalVelocity -= actualBraking * _bumpHorizontalVelocity.normalized;

        if(_bumpHorizontalVelocity.magnitude < 0.05f)
        {
            _bumpHorizontalVelocity = Vector3.zero;
        }

        if (horizontalVelocity <= 0.05f)
        {
            horizontalVelocity = 0;
        }

        inputVelocity = moveDir * horizontalVelocity;
        finalVelocity = inputVelocity + _bumpHorizontalVelocity;

        controller.Move(finalVelocity * Time.deltaTime);

    }

    /// <summary>
    /// Applies movement to the player horizontally in relation to the camera orientation.
    /// </summary>
    private void WalkingMovementState()
    {
        // Direction Vector
        Vector3 moveDir = transform.forward.normalized;

        // Composite movement
        Vector3 inputVelocity;
        Vector3 finalVelocity;

        // Braking 
        float actualBraking = 0;

        // Camera Angle Logic Calculation
        float targetAngle = Mathf.Atan2(rawDirection.x, rawDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

        // Extra Drag Force if the player is Grounded
        if (isGrounded())
        {
            actualBraking = (deccelerationSpeed * 2) * moveDir.magnitude * Time.deltaTime;
            //Debug.Log("Using Extra Ground Force");
        }
        else
        {
            actualBraking = (deccelerationSpeed * 2) * moveDir.magnitude * airDragMultiplier * Time.deltaTime;
        }

        // Player input detected, move player
        if (rawDirection.magnitude >= 0.1f)
        {
            moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            transform.rotation = Quaternion.Euler(0, angle, 0);

            horizontalVelocity += accelerationSpeed * moveDir.magnitude * Time.deltaTime;

            if (horizontalVelocity > maxSpeed)
            {
                horizontalVelocity = maxSpeed;
            }
        }

        // No player input
        // Decellerate the player based on multiplier
        else
        {
            horizontalVelocity -= actualBraking;

            if (horizontalVelocity <= 0.05f)
            {
                horizontalVelocity = 0;
            }
        }

        _bumpHorizontalVelocity -= actualBraking * _bumpHorizontalVelocity.normalized;

        if (_bumpHorizontalVelocity.magnitude < 0.05f)
        {
            _bumpHorizontalVelocity = Vector3.zero;
        }

        inputVelocity = moveDir * horizontalVelocity;
        finalVelocity = inputVelocity + _bumpHorizontalVelocity;

        controller.Move(finalVelocity * Time.deltaTime);
    }

/*    private Vector3 slideVelocity = Vector3.zero;
    private float currentSlideVelocity = 1;
    private float maxSlideVel;*/

/*    private Vector3 Sliding()
    {
        // Check for Surface Sliding
        float surfaceAngle = 0;
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 5, jumpableLayers))
        {
            surfaceAngle = Vector3.Angle(hit.normal, Vector3.up);
            //Debug.Log(surfaceAngle);
        }
        Vector3 thisObj = this.transform.position;
        Vector3 dir = Vector3.Reflect(Vector3.down, hit.normal);
        dir = Vector3.ProjectOnPlane(dir, hit.normal);
        Debug.DrawRay(this.transform.position, dir * 5, Color.magenta);
        if(surfaceAngle < 25) { return Vector3.zero; }

        return dir;
    }*/

    /// <summary>
    /// Applies Gravity over time to the player, does not run the calculation if the player is grounded
    /// </summary>
    private void ApplyGravity()
    {
        if (lockVeritcalMovement) { return; }

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
        if (lockVeritcalMovement) { return; }
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
        Vector3 start1, start2, start3, start4 = Vector3.zero;
        //Vector3 start5, start6, start7, start8 = Vector3.zero;

        // Player not proning
        if (!isProne)
        {
            start1 = transform.position + this.transform.forward * playerRadius;
            start2 = transform.position - this.transform.forward * playerRadius;
            start3 = transform.position + this.transform.right * playerRadius;
            start4 = transform.position - this.transform.right * playerRadius;
        }
        // Player in Prone
        else
        {
            float offset = 1.1f;

            // Capsule Collider on Prone
/*            start1 = transform.position + (Vector3.down * .5f) + this.transform.forward * playerRadius * 2;
            start2 = transform.position + (Vector3.down * .5f) - this.transform.forward * playerRadius * 2;*/
            // Sphere Collider on Prone
            start1 = transform.position + (Vector3.down * .6f) + this.transform.forward * playerRadius * (offset + .2f);
            start2 = transform.position + (Vector3.down * .6f) - this.transform.forward * playerRadius * offset;

            start3 = transform.position + (Vector3.down * .6f) + this.transform.right * playerRadius * offset;
            start4 = transform.position + (Vector3.down * .6f) - this.transform.right * playerRadius * offset;
            // Note: Don't go under .5f

            // 
        }

        Vector3 delta = Vector3.down * ((0.5f * playerHeight) + .2f);

        Debug.DrawLine(start1, start1 + delta, Color.red);
        Debug.DrawLine(start2, start2 + delta, Color.blue);
        Debug.DrawLine(start3, start3 + delta, Color.green);
        Debug.DrawLine(start4, start4 + delta, Color.magenta);

        _grounded = false;
/*        Vector3 groundedVector = start1;*/
/*        float surfaceAngle = 0;*/
        RaycastHit hit = new RaycastHit();

        // Check if the character is grounded using a raycast
        // Is grounded Raycast changes the origin based on proning state
        _grounded = Physics.Linecast(start1, start1 + delta, out hit, jumpableLayersMinusPlayer); groundedVector = start1;
        if (_grounded == false) { _grounded = Physics.Linecast(start2, start2 + delta, out hit, jumpableLayersMinusPlayer); groundedVector = start2; };
        if (_grounded == false) { _grounded = Physics.Linecast(start3, start3 + delta, out hit, jumpableLayersMinusPlayer); groundedVector = start3; };
        if (_grounded == false) { _grounded = Physics.Linecast(start4, start4 + delta, out hit, jumpableLayersMinusPlayer); groundedVector = start4; };

        // Slidding Test - Not Efficient
        if (_grounded)
        {
            if (Physics.Raycast(groundedVector, delta, out hit, delta.magnitude, jumpableLayers))
            {
                float surfaceAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (surfaceAngle > 60)
                {
/*                    Vector3 result = new Vector3(hit.normal.x, 0, hit.normal.z);
                    this.GetComponent<CharacterController>().Move(result * Time.deltaTime * slantedSurfacePushbackMultiplier);
                    groundedVector = Vector3.zero;
                    Debug.DrawRay(hit.point, result * 5, Color.red);*/
                    _grounded = false;
                }
                //Debug.Log(surfaceAngle);
            }
        }

        if (_grounded)
        {
            MovingPlatformData newBase = hit.transform.GetComponent<MovingPlatformData>();
            if(newBase != null)
                currentBase = newBase;
        }
        else
        {
            currentBase = null;
        }
        
        return _grounded;
    }

    private Vector3 groundedVector = Vector3.zero;
    public float slantedSurfacePushbackMultiplier = 1;

    /// <summary>
    /// Stick the player to moving platforms.
    /// </summary>
    private void MoveWithBase()
    {
        if (currentBase == null) return;

        controller.enabled = false;

        transform.position += currentBase.parent.ChangeInPosition;

        Quaternion orientation = transform.rotation;

        transform.RotateAround(currentBase.parent.transform.position, Vector3.right, currentBase.parent.ChangeInRotation.x);
        transform.RotateAround(currentBase.parent.transform.position, Vector3.forward, currentBase.parent.ChangeInRotation.z);

        transform.rotation = orientation;

        transform.RotateAround(currentBase.parent.transform.position, Vector3.up, currentBase.parent.ChangeInRotation.y);

        controller.enabled = true;
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
            // Prevent Diving when Diving was canceled midair
            if (diveWasCanceled) { return; }

            //horizontalVelocity += diveForce;
            //if(horizontalVelocity > maxSpeed + diveForce)
            //{
            //    horizontalVelocity = maxSpeed + diveForce;
            //}

            _bumpHorizontalVelocity += diveForce * transform.forward;

            isProne = true;
            if (rotateModelonDive)
            {
                this.transform.GetChild(0).transform.Rotate(90, 0, 0);
                this.transform.GetChild(0).transform.position += new Vector3(0, -0.5f, 0);
            }
            this.GetComponent<CapsuleCollider>().center = new Vector3(0, -0.5f, 0);
            // Capsule Collider Direction Horizontal
            //this.GetComponent<CapsuleCollider>().direction = 2;
            this.GetComponent<CapsuleCollider>().height = 1;

            // Make Character controller to a ball
            controller.height = 1;
            controller.center = new Vector3(0, -0.5f, 0);
            //controller.height = 1;

            playerHeight = controller.height;
        }
        else // Stand Up
        {
            _bumpHorizontalVelocity = Vector3.zero;
            horizontalVelocity = 0;

            // Check if the player is already Diving and Prevent from Diving Again;
            if (isProne)
            {
                if (!isGrounded())
                {
                    diveWasCanceled = true;
                }
            }

            isProne = false;
            if (rotateModelonDive)
            {
                this.transform.GetChild(0).transform.Rotate(-90, 0, 0);
                this.transform.GetChild(0).transform.position += new Vector3(0, 0.5f, 0);
            }
            this.GetComponent<CapsuleCollider>().center = new Vector3(0, 0, 0);
            // Capsule Collider Vertical
            //this.GetComponent<CapsuleCollider>().direction = 1;
            // Make Character controller to a ball
            this.GetComponent<CapsuleCollider>().height = 2;
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
        Vector3 bumpVelocity = direction * magnitude;

        _bumpHorizontalVelocity += new Vector3(bumpVelocity.x, 0, bumpVelocity.z);

        // If vertical velocity of the bumper is acting against this, then cancel this vertical velocity. Otherwise sum the velocities.
        if(bumpVelocity.y * _verticalVelocity <= 0)
        {
            _verticalVelocity = bumpVelocity.y;
        }
        else
        {
            _verticalVelocity += bumpVelocity.y;
        }
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
