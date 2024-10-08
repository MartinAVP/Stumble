using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    ThirdPersonMovement movement;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();  
        movement = this.transform.parent.GetComponent<ThirdPersonMovement>();
    }

    private void LateUpdate()
    {
        animator.SetFloat("velocity", movement.horizontalVelocity);
    }
}
