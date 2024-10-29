using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BorderAnimation : MonoBehaviour
{
    //public Sprite sprites;
    public image Border;

    public List<Sprite> Border_Frames;
    public float frameDelay = .1f;

    private bool available = true;

    void Start()
    {
        for (int i = 0; i < Border_Frames.Count; i++)
        {
            image.sprite = Border_Frames[i]; 
        }
        Border_Frames[0].SetActive(true);

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
            if (i == 0)
            {
                Border_Frames[Border_Frames.count].SetActive(true);
            }
            { }

            Border_Frames[i-1].SetActive(false);
            Border_Frames[i].SetActive(true);
            yield return new WaitForSecondsRealtime(frameDelay);
        } 
        available = true;
    }

}

