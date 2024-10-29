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

    private bool available = true;

    void Start()
    {
        StartCoroutine(Animate());
    }


    void FixedUpdate()
    {
        if (available)
        {
            StartCoroutine(Animate());

        }
    }

    private IEnumerator Animate()
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

