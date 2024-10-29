using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BorderAnimation : MonoBehaviour
{
    //public Sprite sprites;
    public Image Border;

    public List<Sprite> Border_Frames;
    public float frameDelay = .1f;

    public bool available = true;

    void Start()
    {
        Border.enabled = false;
    }


    void FixedUpdate()
    {       
        if (available)
        {
            StartCoroutine(Animate());

        }
    }


    void IPointerEnterHandler()
    {
        Border.enabled = true;  
    }

    void IPointerExitHandler()
    {
        Border.enabled = false;
    }

    public IEnumerator Animate()
    {
        available = false;
        for (int i = 0;i < Border_Frames.Count; i++)
        {
            Border.sprite = Border_Frames[i];
            yield return new WaitForSecondsRealtime(frameDelay);
        } 
        available = true;
    }

}

