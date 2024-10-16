using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonMovement))]
public class PlayerSounds : MonoBehaviour
{
    private ThirdPersonMovement m_Movement;
    private void Awake()
    {
        m_Movement = this.GetComponent<ThirdPersonMovement>();
    }

    private void OnEnable()
    {
        m_Movement.OnJump += callJump;
        m_Movement.OnDive += callDive;
    }

    private void callJump()
    {
        if(SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySound("Jump", this.transform);
        }
    }

    private void callDive()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySound("Dive", this.transform);
        }
    }

    public bool isGrounded => m_Movement.isFloored;
}
