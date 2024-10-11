using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    ThirdPersonMovement thirdPersonMovement;
    StaticPlayerMovement staticPlayerMovement;

    public float horizontalSpeed;
    public float verticalSpeed;
    public bool isGrounded;
    public bool isProne;

    public float slideFactor;

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
    }

    private void FixedUpdate()
    {
        horizontalSpeed = thirdPersonMovement.horizontalVelocity;
        verticalSpeed = thirdPersonMovement.verticalVelocity;
        isGrounded = thirdPersonMovement.isFloored;
        isProne = thirdPersonMovement.isProne;

        animator.SetFloat("horizontalVelocity", horizontalSpeed);
        animator.SetFloat("verticalVelocity", verticalSpeed);
        animator.SetBool("proning", isProne); 
        animator.SetBool("grounded", isGrounded); 
/*        if (thirdPersonMovement != null)
        {
            
        }*/
/*        if(staticPlayerMovement != null)
        {
            animator.SetFloat("velocity", staticPlayerMovement.horizontalVelocity);
        }*/

    }
}
