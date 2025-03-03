using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.VisualScripting;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    // Requierements
    public CharacterController controller;
    public Transform camController;
    public Transform cam;
    public PlayerMovement playerMovementSettings;

    public event Action OnJump;
    public event Action OnDive;
    public event Action OnSlap;

    [HideInInspector] public UnityEvent OnSlapPlayer;
    [HideInInspector] public UnityEvent OnJumpPlayer;

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

    #region Slaping
    private float slapForce = 10f;
    private float slapUpWardForce = 1.0f;
    private float slapDistance = 1.5f;
    private float slapCooldown = 1.0f;

    private bool canSlap = true;
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
    [HideInInspector] public InputHandler camInputHandler;
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
    private bool canDive = false;
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

        // Slaping
        slapForce = playerMovementSettings.slapForce;
        slapUpWardForce = playerMovementSettings.slapUpWardForce;
        slapDistance = playerMovementSettings.slapDistance;
        slapCooldown = playerMovementSettings.slapCooldown;

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

        // Dive Cooldown
        StartCoroutine(DiveCooldown(2f));

        // Update Sensitivity
        updateSensitivity(baseVerticalViewSensitivity, baseInvertVerticalInput, baseHorizontalViewSensitivity, baseInvertHorizontalInput);

        // Set the player Height
        controller.height = playerHeight;

        //Application.targetFrameRate = 20;

        controller.detectCollisions = false;

        // Remove Player from Calculating Physics with the world
        jumpableLayersMinusPlayer = jumpableLayers &= ~(1 << this.gameObject.layer);

        // Add look action to cam
        camInputHandler = this.transform.parent.GetComponentInChildren<InputHandler>();

        if (FindAnyObjectByType<PlayerManager>().sceneCameraType == SceneCameraType.StaticCamera)
        {
            this.transform.GetComponent<PlayerInput>().camera = Camera.main;
            cam = Camera.main.transform;
        }

        if (FindFirstObjectByType<ExperimentalPlayerManager>() == null) // No Player Experimental Controller
        {
            camInputHandler.horizontal = this.GetComponent<PlayerInput>().actions.FindAction("Look");
/*            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;*/
            return;
        }

        if (FindAnyObjectByType<ExperimentalPlayerManager>().GetCameraType() == SceneCameraType.ThirdPersonControl)
        {
            camInputHandler.horizontal = this.GetComponent<PlayerInput>().actions.FindAction("Look");
        }
        //Debug.Log("I got here 2");
    }

    private IEnumerator DiveCooldown(float time)
    {
        canDive = false;
        yield return new WaitForSeconds(time);
        canDive = true;
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

    private void LateUpdate()
    {
        if(this.transform.position.y < -600)
        {
            controller.enabled = false;
            this.transform.position = new Vector3(0, 20, 0);
            verticalVelocity = 0;
            horizontalVelocity = 0;
            CancelVelocity();
            toggleProne(false);
            controller.enabled = true;
        }
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

        //RaycastHit hit;
        Vector3 delta = Vector3.up * 1.1f * (controller.height / 2);

        if(!isProne &&
            Physics.Linecast(transform.position, transform.position + delta))
        {
            return;
        }

        // Check if player is prone, unprone if proned;
        if (isProne) {
            //Debug.Log("Un Toggle Prone");
            // Cancel velocity when standing up
            CancelVelocity();
            toggleProne(false);
            return; 
        }

        // Check if player is on the air
        if (!_grounded) return;

        // Add to the Vertical Velocity Value
        verticalVelocity = 0;
        verticalVelocity += jumpPower;

        OnJump?.Invoke();
        OnJumpPlayer?.Invoke();
    }

    public void Dive(InputAction.CallbackContext context)
    {
        if(lockMovement) return;

        // Prevent Diving when on the ground
        if (_grounded) return;

        //Debug.Log(canDive);
        if (!canDive) { return; }

        //Debug.Log("Dive");
        // Change Model
        if (context.started)
        {
            // Prevent Proning when already in prone
            if (isProne) { return; }
            toggleProne(true);


            //verticalVelocity = 1;
            OnDive?.Invoke();
        }
    }

    public void Slap(InputAction.CallbackContext context)
    {
        if (lockMovement) return;

        if (context.started)
        {
            OnSlap?.Invoke();
            SlapPlayer();
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
        if(Time.timeScale == 0)
        {
            return;
        }
        _bumpHorizontalVelocity.y = 0;

/*        print("Bump velocity: " + _bumpHorizontalVelocity +
            "Horizontal Velocity: " + horizontalVelocity);*/

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

        //print("player pos " + transform.position.y);

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
            //print("Braking: " + actualBraking);
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
                    float platformVerticalVelocity = platformVelocity.y;
                    
                    if(platformVelocity.magnitude > _bumpHorizontalVelocity.magnitude)
                        platformVelocity = platformVelocity.normalized * Vector3.Dot(_bumpHorizontalVelocity.normalized, platformVelocity);

                    _bumpHorizontalVelocity -= platformVelocity;
                    verticalVelocity -= platformVerticalVelocity;
                }
            }
            else if(currentPlatform != null)
            {
                _bumpHorizontalVelocity += platformVelocity;
                verticalVelocity += platformVelocity.y;
                currentPlatform = null;
            }

        }
        else
        {
            if(currentPlatform != null)
            {
                _bumpHorizontalVelocity += platformVelocity;
                verticalVelocity += platformVelocity.y;
            }

            currentPlatform = null;
        }

        if (_grounded == false) { _grounded = controller.isGrounded; };

        //print("Is player grounded? " + _grounded);

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

    private void CancelVelocity()
    {
        _bumpHorizontalVelocity = Vector3.zero;
        horizontalVelocity = 0;
    }

    // Slap Force
    // Slap Cooldown
    // Slap Distance

/*      slapForce = playerMovementSettings.slapForce;
        slapUpWardForce = playerMovementSettings.slapUpWardForce;
        slapDistance = playerMovementSettings.slapForce;
        slapCooldown = playerMovementSettings.slapForce;*/

    public List<GameObject> hitObjs = new List<GameObject>();
    private void SlapPlayer()
    {
        if (!canSlap) { return; }
        canSlap = false;
        /*        RaycastHit hit;
                if(Physics.Raycast(this.transform.position, this.transform.forward, out hit, slapDistance))
                {
                    if (hit.transform.GetComponent<IBumper>() != null)
                    {
                        hit.transform.GetComponent<IBumper>().Bump(this.transform.forward + new Vector3(0, slapUpWardForce, 0), slapForce, BumpSource.StaticBumper);
                        Debug.DrawRay(hit.point, hit.normal, Color.cyan, 5f);
                    }
                }*/

        float offset = 1.1f;
        Vector3 slapDir = this.transform.position + (this.transform.forward * offset);
        Debug.DrawLine(this.transform.position, slapDir, Color.green, 20f);

        //RaycastHit[] hits = Physics.OverlapSphere(slapDir, slapDistance);
        hitObjs.Clear();
        Collider[] hits = Physics.OverlapSphere(slapDir, slapDistance);
        foreach (Collider hit in hits)
        {
            hitObjs.Add(hit.gameObject);
        }


        //CapsuleCollider collider = this.transform.GetComponent<CapsuleCollider>();
        foreach(var singleHit in hits)
        {
            //if(singleHit.transform == this.transform) { break; }
            //Debug.Log("Hit: " + singleHit.gameObject.name);
            //Debug.DrawLine(slapDir, singleHit.point, Color.red, 20f);
            //Debug.DrawRay(slapDir, this.transform.forward - this.transform.position, Color.yellow, 20f);
            //Debug.DrawRay(slapDir, singleHit.point, Color.cyan, 20f);
            Debug.DrawLine(slapDir, singleHit.transform.position, Color.red, 20f);
            GameObject obj = singleHit.gameObject;
            if (obj.GetComponent<IBumper>() != null)
            {
                Debug.DrawLine(singleHit.transform.position, singleHit.transform.position + new Vector3(0, 5f, 0), Color.green, 5f);
                Debug.DrawLine(slapDir, singleHit.transform.position, Color.cyan, 20f);
                singleHit.gameObject.transform.GetComponent<IBumper>().Bump(this.transform.forward + new Vector3(0, slapUpWardForce, 0), slapForce, BumpSource.StaticBumper);
                //Debug.DrawRay(hit.point, hit.normal, Color.cyan, 5f);
                //Debug.DrawLine(this.transform.position, singleHit.point, Color.cyan);
            }
            else if (singleHit.GetComponent<ThirdPersonMovement>() != null)
            {
                if(singleHit.GetComponent<ThirdPersonMovement>() != this){                
                    Debug.DrawLine(singleHit.transform.position, singleHit.transform.position + new Vector3(0, 5f, 0), Color.green, 5f);
                    Debug.DrawLine(slapDir, singleHit.transform.position, Color.cyan, 20f);
                    singleHit.GetComponent<ThirdPersonMovement>().Bump(this.transform.forward + new Vector3(0, slapUpWardForce, 0), slapForce);

                    OnSlapPlayer?.Invoke();

                    if(SFXManager.Instance != null) 
                        SFXManager.Instance.PlaySound("Punch", transform);
                } // Prevent Auto Bumping
            }
            else
            {
                Debug.DrawLine(singleHit.transform.position, singleHit.transform.position + new Vector3(0, 5f, 0), Color.red, 5f);
            }
        }

        StartCoroutine(SlapCooldown(slapCooldown));
    }

    private Physics physics;

    private IEnumerator SlapCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        canSlap = true;

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
        //print("Bump player: " + direction + " " + magnitude);

        if (magnitude < .01f || direction.magnitude < .01f) return;

        if (isProne)
        {
            toggleProne(false);
        }
        diveWasCanceled = false;
        Vector3 bumpVelocity = direction * magnitude;

        _bumpHorizontalVelocity += new Vector3(bumpVelocity.x, 0, bumpVelocity.z);

        // If vertical velocity of the bumper is acting against this, then cancel this vertical velocity. Otherwise sum the velocities.
        if (bumpVelocity.y * verticalVelocity <= 0)
        {
            verticalVelocity = bumpVelocity.y;
        }
        else
        {
            verticalVelocity += bumpVelocity.y;
        }
    }

    private void BumpBack(Vector3 direction, Vector3 position, IBumper source)
    {
        Debug.DrawLine(transform.position, transform.position + CompositeVelocity, Color.yellow, 5f);

        if (source.GetBumpSource() != BumpSource.StaticBumper &&
            Vector3.Dot(direction, CompositeVelocity.normalized) < 0)
        {
            float impulseMagnitude = CompositeVelocity.magnitude;
            if (isProne) impulseMagnitude *= bumpForce;

            source.Bump(-direction, position, impulseMagnitude, BumpSource.StaticBumper);
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
        if(hit.point.y > (transform.position.y + (playerHeight / 2) * .9f) &&
            Vector3.Dot(hit.normal, Vector3.up) < 0 &&
            verticalVelocity > 0)
        {
            verticalVelocity = 0;
        }

        if (hit.transform.tag == "Player") {
            if (isProne)
            {
                //Debug.Log("Bump!");
                ThirdPersonMovement targetPlayer = hit.gameObject.GetComponent<ThirdPersonMovement>();
                if (targetPlayer != null)
                {
                    Vector3 bumpDirection = transform.forward.normalized + new Vector3(0, bumpUpwardForce, 0);
                    float bumpMagnitude = bumpForce;

                    targetPlayer.Bump(bumpDirection, bumpMagnitude);

                    SFXManager.Instance.PlaySound("Punch", transform);
                }
            }
        }

        IBumper bumper = hit.gameObject.GetComponent<IBumper>();
        if (bumper != null)
        {
            if(hit.point.y > (transform.position + controller.center).y - (controller.height / 2) - .05f)
            {
                BumpBack(bumper.GetBumpDirection(gameObject), hit.point, bumper);
            }
            Bump(bumper.GetBumpDirection(gameObject), bumper.GetBumpMagnitude());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IBumper bumper = other.gameObject.GetComponent<IBumper>();
        if (bumper != null)
        {
            Bump(bumper.GetBumpDirection(gameObject), bumper.GetBumpMagnitude());
        }
    }
    #endregion

    #region Properties
    public Vector3 CompositeVelocity { get { return (horizontalVelocity * transform.forward) + _bumpHorizontalVelocity + (verticalVelocity * Vector3.up); } }
    #endregion

}
