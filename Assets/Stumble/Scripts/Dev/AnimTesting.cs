using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTesting : MonoBehaviour
{
    public Animator animator;

/*    private void Start()
    {
        this.GetComponent<Animator>();
    }*/

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            animator.SetTrigger("Hit");
        }
    }
}
