using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXController : MonoBehaviour
{
    private ThirdPersonMovement thirdPersonMovement;
    private EmoteWheelController emoteWheel;

    void Start()
    {
        thirdPersonMovement = this.transform.parent.GetComponent<ThirdPersonMovement>();
        emoteWheel = this.transform.parent.GetComponent<EmoteWheelController>();

        thirdPersonMovement.OnSlapPlayer.AddListener(Slap);
        if (emoteWheel != null)
        {
            emoteWheel.PlayEmote += PlayEmote;
        }
    }

    private void OnDestroy()
    {
        if (thirdPersonMovement != null)
        {
            //thirdPersonMovement.OnSlap -= Slap;
            thirdPersonMovement.OnSlapPlayer.RemoveAllListeners();
        }
    }

    private void Slap()
    {

    }

    private void PlayEmote(int id)
    {

    }
}
