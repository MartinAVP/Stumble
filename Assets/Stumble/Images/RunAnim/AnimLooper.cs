using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimLooper : MonoBehaviour
{
    public List<Sprite> sprites;
    public Image image;
    public float frameTime;

    private int currentID = 0;
    private float elapsedTime = 0;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if(sprites.Count == 0) { return; }

        elapsedTime += Time.deltaTime;

        if(elapsedTime >= frameTime)
        {
            currentID++;
            if (currentID >= sprites.Count)
            {
                currentID = 0;
            }

            image.sprite = sprites[currentID];
            elapsedTime = 0;
        }
    }
}
