using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbySlotUI : MonoBehaviour
{
    public RawImage camTexture;
    public Animator animator;

    public void PlayerJoined(){
        animator.SetTrigger("GoOut");
    }
    public void PlayerLeft() {
        animator.SetTrigger("GoIn");
    }
}
