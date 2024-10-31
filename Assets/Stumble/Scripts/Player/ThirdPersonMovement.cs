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

    public event Action OnJump;
    public event Action OnDive;

    #region Horizontal Movement
    [Header("Movement")]
    private float accelerationSpeed = 10f;
    private float deccelerationSpeed = 4f;
    private float airDragMultiplier = 1f;
    private float maxSpeed = 10;
    [HideInInspector] public Vector3 rawDirection;
    [HideInInspector] public float horizontalVelocity = 0;
    private bool _grounded = false;
    #endregion

    private Vector3 groundedVector = Vector3.zero;

    #region Bumping
    [Header("Bumping")]
    private float bumpForce = 20f;
    private float bumpUpwardForce = .2f;
    [HideInInspector] public Vector3 _bumpHorizontalVelocity = Vector3.zero;
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
    [HideInInspector] public float verticalVelocity = 0;
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
    [HideInInspector]public float diveDragMultiplier = 1f;
    private float diveAirDragMultiplier = 1f;
    [HideInInspector]public bool isProne = false;
    private float playerHeight = 2;
    private float playerRadius = 0.5f;
    private bool diveWasCanceled = false;
    #endregion

    #region Platform
    private MovingPlatformData currentPlatform;
    private Vector3 platformVelocity;
    #endregion

    [Header("Debug")]
    // Lock Movement is made for testing bumping
    // If left unchecked, 2 characters will move with the same input
    [SerializeField] private bool rotateModelonDive = true;
    public bool lockMovement = false;
    public bool lockVeritcalMovement = false;

    private void OnEnable()
    {
        // Michael 10/12/2024
        //Application.targetFrameRate = 30;

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
        if(camController != null)
        {
            freelookcam = camController.gameObject.GetComponent<CinemachineFreeLook>();
        }
        //this.GetComponent<PlayerInput>().camera = cam.GetComponent<Camera>();
    }

    private void Start()
    {
        // Movement needs to happen after platforms have been moved so that the character doesn't lag a frame behind platforms 
        // To achieve this a moving platform manager publishes events in order
        // To ensure that platforms always move, and the player always moves, both platforms and the players access then discard the manager instance to spawn a manger. - Michael
        MovingPlatformManager manager = MovingPlatformManager.Instance;
        MovingPlatformEventBus.Subscribe(MovingPlatformEvent.Final, Movement);

        // Update Sensitivity
        updateSensitivity(baseVerticalViewSensitivity, baseInvertVerticalInput, baseHorizontalViewSensitivity, baseInvertHorizontalInput);

        // Set the player Height
        controller.height = playerHeight;

        //Application.targetFrameRate = 20;

        controller.detectCollisions = false;

        // Remove Player from Calculating Physics with the world
        jumpableLayersMinusPlayer = jumpableLayers &= ~(1 << this.gameObject.layer);

        // Add look action to cam

        if (FindFirstObjectByType<ExperimentalPlayerManager>() == null) // No Player Experimental Controller
        {
            this.transform.parent.GetComponentInChildren<InputHandler>().horizontal = this.GetComponent<PlayerInput>().actions.FindAction("Look");
/*            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;*/
            return;
        }

        if (FindAnyObjectByType<ExperimentalPlayerManager>().GetCameraType() == SceneCameraType.ThirdPersonControl)
        {
            this.transform.parent.GetComponentInChildren<InputHandler>().horizontal = this.GetComponent<PlayerInput>().actions.FindAction("Look");
        }
        else if (FindAnyObjectByType<ExperimentalPlayerManager>().GetCameraType() == SceneCameraType.StaticCamera)
        {
            this.transform.GetComponent<PlayerInput>().camera = Camera.main;
            cam = Camera.main.transform;
        }

        //Debug.Log("I got here 2");
    }

    private Transform hasCamera()
    {
        foreach(Transform child in this.transform.parent)
        {
            if(child.GetComponent<CinemachineFreeLook>() != null)
            {
                return child;
            }
        }
        return null;
    }

    private void Update()
    {
        if (diveWasCanceled)
        {
            if (_grounded)
            {
                diveWasCanceled = false;
            }
        }

        isFloored = _grounded;
    }

    private void OnDestroy()
    {
        MovingPlatformEventBus.Unsubscribe(MovingPlatformEvent.Final, Movement);
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
        if (!_grounded) return;

        // Add to the Vertical Velocity Value
        verticalVelocity = 0;
        verticalVelocity += jumpPower;

        OnJump?.Invoke();
    }

    public void Dive(InputAction.CallbackContext context)
    {
        if(lockMovement) return;

        // Prevent Diving when on the ground
        //if (_grounded) return;

        //Debug.Log("Dive");
        // Change Model
        if (context.started)
        {
            // Prevent Proning when already in prone
            if (isProne) { return; }
            toggleProne(true);

            OnDive?.Invoke();
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
        _bumpHorizontalVelocity.y = 0;

        print("Bump velocity: " + _bumpHorizontalVelocity +
            "Horizontal Velocity: " + horizontalVelocity);

        ApplyGravity();
        ApplyVerticalMovement();
        isGrounded();

        // Check for proning state, prevents horizontal movement and character rotation
        if (isProne)
        {
            ProneMovementState();
        }
        else
        {
            WalkingMovementState();
        }

        // Move with base after all other movement seems to yield a slightly smoother result? - Michael
        MoveWithBase();

        print("player pos " + transform.position.y);

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
        if (_grounded)
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

        if (actualBraking > _bumpHorizontalVelocity.magnitude)
        {
            _bumpHorizontalVelocity = Vector3.zero;
        }
        else
            _bumpHorizontalVelocity -= actualBraking * _bumpHorizontalVelocity.normalized;

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
        if (_grounded)
        {
            actualBraking = (deccelerationSpeed * 2) * moveDir.magnitude * Time.deltaTime;
            //Debug.Log("Using Extra Ground Force");
        }
        else
        {
            actualBraking = (deccelerationSpeed * 2) * moveDir.magnitude * airDragMultiplier * Time.deltaTime;

            //print(_bumpHorizontalVelocity);
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
            print("Braking: " + actualBraking);
            horizontalVelocity -= actualBraking;

            if (horizontalVelocity <= 0.05f)
            {
                horizontalVelocity = 0;
            }
        }

        if(actualBraking > _bumpHorizontalVelocity.magnitude)
        {
            _bumpHorizontalVelocity = Vector3.zero;
        }
        else
            _bumpHorizontalVelocity -= actualBraking * _bumpHorizontalVelocity.normalized;

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



        if (_grounded && verticalVelocity < -5)
        {
            verticalVelocity = -5;  // Prevents the character from sinking into the ground
        }
        else
        {
            verticalVelocity += _gravity * gravityMultiplier * Time.deltaTime;
        }
    }

    /// <summary>
    /// Uses the verticalVelocity value and applies movement to the character vertically
    /// </summary>
    private void ApplyVerticalMovement()
    {
        if (lockVeritcalMovement) { return; }
        Vector3 fallVector = new Vector3(0, verticalVelocity, 0);
        controller.Move(fallVector * Time.deltaTime);
    }
    #endregion

    // Internal Functions
    // ===========================================================================================
    #region Internal Functions
    /// <summary>
    /// Checks if the player is Grounded depending on the minimum jump distance.
    /// </summary>
    private void isGrounded()
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

        // ==========================================================================================================================================================
        //                                                      Extra Ground Check
        // ==========================================================================================================================================================
        // Not Grounded with initial checks, add two more front checks
        /*
        if (!_grounded)
        {
            //float degree = 15f;
            //if(foundGroundCheck)
            float degree = extraGroundCheck();

            Quaternion rotate1 = Quaternion.Euler(0, degree, 0);
            Quaternion rotate2 = Quaternion.Euler(0, -degree, 0);

            extra1 = (rotate1 * this.transform.forward) * playerRadius;
            extra2 = (rotate2 * this.transform.forward) * playerRadius;

            extra1 += this.transform.position;
            extra2 += this.transform.position;

            //extra2 += this.transform.right;
            Debug.DrawRay(extra1, Vector3.down, Color.cyan);
            Debug.DrawRay(extra2, Vector3.down, Color.cyan);
            //extra2 -= this.transform.right;
            //Debug.DrawRay(this.transform.position, extra2 * 6, Color.blue);

            if (_grounded == false) { _grounded = Physics.Linecast(extra1, extra1 + delta, out hit, jumpableLayersMinusPlayer); groundedVector = extra1; };
            if (_grounded == false) { _grounded = Physics.Linecast(extra2, extra2 + delta, out hit, jumpableLayersMinusPlayer); groundedVector = extra2; };

            if(_grounded) { foundGroundCheck = true; }
            else { foundGroundCheck = false; }
        }
        else
        {
            foundGroundCheck = false;
        }
        */

        // Slidding Test - Not Efficient
        /*
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
                    Debug.DrawRay(hit.point, result * 5, Color.red);
                    _grounded = false;
                }
                //Debug.Log(surfaceAngle);
            }
        }
        */

        if (_grounded)
        {
            MovingPlatformData newBase = hit.transform.GetComponent<MovingPlatformData>();
            if(newBase != null)
            {
                bool cancelVelocity = currentPlatform == null;

                currentPlatform = newBase;

                if (cancelVelocity)
                {
                    Vector3 platformVelocity = currentPlatform.LinearVelocity;
                    
                    if(platformVelocity.magnitude > _bumpHorizontalVelocity.magnitude)
                        platformVelocity = platformVelocity.normalized * Vector3.Dot(_bumpHorizontalVelocity.normalized, platformVelocity);

                    _bumpHorizontalVelocity -= platformVelocity;
                }
            }
            else if(currentPlatform != null)
            {
                //Vector3 platformVelocity = currentPlatform.LinearVelocity;
                _bumpHorizontalVelocity += platformVelocity;
                currentPlatform = null;
            }

        }
        else
        {
            if(currentPlatform != null)
            {
                //Vector3 platformVelocity = currentPlatform.LinearVelocity;
                _bumpHorizontalVelocity += platformVelocity;
            }

            currentPlatform = null;
        }

        if (_grounded == false) { _grounded = controller.isGrounded; };

        print("Is player grounded? " + _grounded);

        //return _grounded;
    }

    // Extra Ground Functions
    private bool foundGroundCheck = false;
    private float foundGroundCheckAngle = 0f;

    private float extraGroundCheck()
    {
        float t = Time.time / .1f;

        float pingPongValue = Mathf.PingPong(t, 1f);
        float smoothT = Mathf.Sin(pingPongValue * Mathf.PI * 0.5f);
        float angle = Mathf.Lerp(0f, 90f, smoothT);

        return angle;
    }

    /// <summary>
    /// Stick the player to moving platforms.
    /// </summary>
    private void MoveWithBase()
    {
        if (currentPlatform == null) return;

        controller.enabled = false;
        Vector3 startPos = transform.position;

        transform.position += currentPlatform.ChangeInPosition;

        Quaternion orientation = transform.rotation;

        transform.RotateAround(currentPlatform.transform.position, Vector3.right, currentPlatform.ChangeInRotation.x);
        transform.RotateAround(currentPlatform.transform.position, Vector3.forward, currentPlatform.ChangeInRotation.z);

        transform.rotation = orientation;

        transform.RotateAround(currentPlatform.transform.position, Vector3.up, currentPlatform.ChangeInRotation.y);

        controller.enabled = true;

        platformVelocity = (transform.position - startPos) / currentPlatform.DeltaTime;
        //platformVelocity.y = 0;
    }


    private void updateSensitivity(float vertical, bool invertVertical, float horizontal, bool invertHorizontal)
    {
        if(camController != null)
        {
            freelookcam.m_YAxis.m_MaxSpeed = vertical;
            freelookcam.m_XAxis.m_MaxSpeed = horizontal;

            freelookcam.m_YAxis.m_InvertInput = invertVertical;
            freelookcam.m_XAxis.m_InvertInput = invertHorizontal;
        }
    }

    public void toggleProne(bool activate)
    {
        if (activate)
        {
            // Prevent Diving when Diving was canceled midair
            if (diveWasCanceled) { return; }

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
                if (!_grounded)
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
        if(bumpVelocity.y * verticalVelocity <= 0)
        {
            verticalVelocity = bumpVelocity.y;
        }
        else
        {
            verticalVelocity += bumpVelocity.y;
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
