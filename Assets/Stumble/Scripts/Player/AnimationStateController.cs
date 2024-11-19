using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    ThirdPersonMovement thirdPersonMovement;
    EmoteWheelController emoteWheel;

    // Updating Variables
    public float horizontalSpeed;
    public float verticalSpeed;
    public Vector3 rawHorizontal;

    public bool isMoving;
    public bool isGrounded;
    public bool isProne;
    public bool isFalling;
    public bool isAscending;
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
        emoteWheel = this.transform.parent.GetComponent<EmoteWheelController>();
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

        thirdPersonMovement.OnSlap += Slap;
        if(emoteWheel != null)
        {
            emoteWheel.PlayEmote += PlayEmote;
        }
        animator.SetFloat("slideFactor", slideFactor);
    }

    private void OnDestroy()
    {
        if(thirdPersonMovement != null)
        {
            thirdPersonMovement.OnSlap -= Slap;
        }
    }

    private void Update()
    {
        horizontalSpeed = thirdPersonMovement.horizontalVelocity;
        verticalSpeed = thirdPersonMovement.verticalVelocity;
        isGrounded = thirdPersonMovement.isFloored;
        isProne = thirdPersonMovement.isProne;

        rawHorizontal = thirdPersonMovement.rawDirection;

        CalculateExtraFactors();

        animator.SetFloat("horizontalVelocity", horizontalSpeed);
        animator.SetFloat("verticalVelocity", verticalSpeed);
        animator.SetBool("moving", isMoving);
        animator.SetBool("proning", isProne); 
        animator.SetBool("grounded", isGrounded);

        animator.SetBool("falling", isFalling); 
        animator.SetBool("sliding", isSliding); 
        animator.SetBool("ascending", isAscending); 
/*        if (thirdPersonMovement != null)
        {
            
        }*/
/*        if(staticPlayerMovement != null)
        {
            animator.SetFloat("velocity", staticPlayerMovement.horizontalVelocity);
        }*/

    }

    private void Slap()
    {
        animator.SetTrigger("Hit");
    }

    private void PlayEmote(int id)
    {
        if(!isMoving && !isProne)
        {
            animator.SetInteger("EmoteID", id);
            animator.SetTrigger("ExecuteEmote");
        }

    }

    private void CalculateExtraFactors()
    {
        bool tempMoving = false;
        if (rawHorizontal != Vector3.zero)
        {
            tempMoving = true;
        }
        isMoving = tempMoving;

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

        bool tempAscending = false;
        if (!isGrounded) 
        {
            if (verticalSpeed > 0)
            {
                tempAscending = true;
            }
        }
        isAscending = tempAscending;
    }
}
