using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    ThirdPersonMovement thirdPersonMovement;
    StaticPlayerMovement staticPlayerMovement;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        if(this.transform.parent.GetComponent<ThirdPersonMovement>() != null)
        {
            thirdPersonMovement = this.transform.parent.GetComponent<ThirdPersonMovement>();
            return;
        }
        else if(this.transform.parent.GetComponent<StaticPlayerMovement>() != null)
        {
            staticPlayerMovement = this.transform.parent.GetComponent<StaticPlayerMovement>();
            return;
        }
    }

    private void LateUpdate()
    {
        if (thirdPersonMovement != null)
        {
            animator.SetFloat("velocity", thirdPersonMovement.horizontalVelocity);
        }
        if(staticPlayerMovement != null)
        {
            animator.SetFloat("velocity", staticPlayerMovement.horizontalVelocity);
        }

    }
}
