using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXController : MonoBehaviour
{
    private ThirdPersonMovement thirdPersonMovement;
    private EmoteWheelController emoteWheel;

    [Header("Static Effects")]
    [SerializeField] private Transform slapVFX;
    [SerializeField] private Transform jumpVFX;
    [SerializeField] private Transform runningVFX;

    void Start()
    {
        thirdPersonMovement = this.transform.parent.GetComponent<ThirdPersonMovement>();
        emoteWheel = this.transform.parent.GetComponent<EmoteWheelController>();

        thirdPersonMovement.OnSlapPlayer.AddListener(Slap);
        thirdPersonMovement.OnJumpPlayer.AddListener(Jump);
        if (emoteWheel != null)
        {
            emoteWheel.PlayEmote += PlayEmote;
        }

        // Stop Running Effects
        InitiliazeEffects();
    }

    private void OnDestroy()
    {
        if (thirdPersonMovement != null)
        {
            //thirdPersonMovement.OnSlap -= Slap;
            thirdPersonMovement.OnSlapPlayer.RemoveAllListeners();
            thirdPersonMovement.OnJumpPlayer.RemoveAllListeners();
        }
    }

    private void InitiliazeEffects()
    {
        slapVFX.GetComponent<ParticleSystem>().Stop();
        jumpVFX.GetComponent<ParticleSystem>().Stop();
    }


    private bool isRunningState = false;
    private void FixedUpdate()
    {
        if(thirdPersonMovement.horizontalVelocity > .5f)
        {
            if(isRunningState)
            {
                return;
            }
            isRunningState = true;

            runningVFX.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            if(!isRunningState)
            {
                return;
            }

            isRunningState = false;
            runningVFX.GetComponent<ParticleSystem>().Stop();
        }
    }

    private void Slap()
    {
        TriggerVFX(0);
    }

    private void Jump()
    {
        TriggerVFX(1);
    }
    private void PlayEmote(int id)
    {

    }


    private bool isSlapping = false;
    private bool isJumping = false;

    public void TriggerVFX(int id)
    {
        switch (id)
        {
            // Slap
            case 0:
                if(slapVFX.GetComponent<ParticleSystem>() != null)
                {
                    if(isSlapping) { return; }
                    slapVFX.GetComponent<ParticleSystem>().Play();
                    isSlapping = true;
                    StartCoroutine(SlapDelay());
                }
                break;
            case 1:
                if(slapVFX.GetComponent<ParticleSystem>() != null)
                {
                    if(isJumping) { return; }
                    if(!thirdPersonMovement.isFloored) { return; }
                    jumpVFX.GetComponent<ParticleSystem>().Play();
                    isJumping = true;
                    StartCoroutine(JumpDelay());

                }
                break;
        }
    }

    private IEnumerator SlapDelay()
    {
        yield return new WaitForSeconds(.5f);
        slapVFX.GetComponent<ParticleSystem>().Stop();
        isSlapping = false;
    }

    private IEnumerator JumpDelay()
    {
        yield return new WaitForSeconds(1f);
        slapVFX.GetComponent<ParticleSystem>().Stop();
        isJumping = false;
        
    }
}
