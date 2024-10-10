using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerMovement", menuName = "PlayerMovement/PlayerMovementSettings", order = 1)]
public class PlayerMovement : ScriptableObject
{
    #region Horizontal Movement
    [Header("Movement")]
    public float accelerationSpeed = 10f;
    public float deccelerationSpeed = 4f;
    public float airDragMultiplier = 1.0f;
    public float maxSpeed = 10;
    #endregion

    #region Bumping
    [Header("Bumping")]
    public float bumpForce = 20f;
    [Range(0f, 5f)]public float bumpUpwardForce = .2f;
    #endregion

    #region Rotating
    [Header("Rotation")]
    public float turnSmoothTime = 0.1f;
    #endregion

    #region Jumping
    [Header("Jump & Gravity")]
    public float jumpPower = 10;
    public LayerMask jumpableLayers;
    [Tooltip("It has to be more half the height of the character. Recommended [0.2] more than half the height. [0.2] Allows jumping on a 45 degree surface")]
    public float minJumpDistance = .2f;
    #endregion

    #region Vertical Movement
    // Gravity Variables
    [Space]
    public float gravityMultiplier = 3.0f;
    #endregion

    #region Camera Controls
    [Header("Camera Controls")]
    // Note: These values will only update when starting the game, not while the scene is playing.
    [Tooltip("Default: 5")]
    public float baseVerticalViewSensitivity = 5.0f;
    public bool baseInvertVerticalInput = false;
    [Space]
    [Tooltip("Default: 300")]
    public float baseHorizontalViewSensitivity = 300.0f;
    public bool baseInvertHorizontalInput = false;
    #endregion

    #region Diving
    [Header("Diving")]
    public float diveForce = 2;
    public float diveDragMultiplier = 1f;
    public float diveAirDragMultiplier = 1f;
    #endregion
}
