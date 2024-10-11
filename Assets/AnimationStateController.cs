using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    ThirdPersonMovement thirdPersonMovement;
    StaticPlayerMovement staticPlayerMovement;

    // Updating Variables
    public float horizontalSpeed;
    public float verticalSpeed;
    public bool isGrounded;
    public bool isProne;
    public bool isFalling;
    public bool isSliding;

    // Static Variables
    public float slideFactor = 3;

    [Header("Settings")]
    [SerializeField] private float slideTreshold;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();

        thirdPersonMovement = this.transform.parent.GetComponent<ThirdPersonMovement>();
        //return;
        /*        if (this.transform.parent.GetComponent<ThirdPersonMovement>() != null)
                {

                }*/
        /*        else if(this.transform.parent.GetComponent<StaticPlayerMovement>() != null)
                {
                    staticPlayerMovement = this.transform.parent.GetComponent<StaticPlayerMovement>();
                    return;
                }*/

        slideFactor = thirdPersonMovement.diveDragMultiplier;
        animator.SetFloat("slideFactor", slideFactor);
    }

    private void FixedUpdate()
    {
        horizontalSpeed = thirdPersonMovement.horizontalVelocity;
        verticalSpeed = thirdPersonMovement.verticalVelocity;
        isGrounded = thirdPersonMovement.isFloored;
        isProne = thirdPersonMovement.isProne;

        CalculateExtraFactors();

        animator.SetFloat("horizontalVelocity", horizontalSpeed);
        animator.SetFloat("verticalVelocity", verticalSpeed);
        animator.SetBool("proning", isProne); 
        animator.SetBool("grounded", isGrounded);

        animator.SetBool("falling", isFalling); 
        animator.SetBool("sliding", isSliding); 
/*        if (thirdPersonMovement != null)
        {
            
        }*/
/*        if(staticPlayerMovement != null)
        {
            animator.SetFloat("velocity", staticPlayerMovement.horizontalVelocity);
        }*/

    }

    private void CalculateExtraFactors()
    {
        bool tempFall = false;
        if (!isGrounded)
        {
            if (verticalSpeed < 0)
            {
                tempFall = true;
            }
        }
        isFalling = tempFall;

        bool tempSliding = false;
        if (isGrounded)
        {
            if (isProne)
            {
                if(horizontalSpeed > slideTreshold)
                {
                    tempSliding = true;
                }
            }
        }
        isSliding = tempSliding;
    }
}
