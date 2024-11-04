using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JacknBox_Button : MonoBehaviour
{
    [SerializeField] Animator animator;

    public GameObject Squish;

    private void OnTriggerEnter(Collider other)
    {
        print("Jack box triggered");
        if (other.gameObject.tag == "Player")
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("JackNDaBox")) return;

            animator.Play("JackNDaBox");
        }
    }

}
