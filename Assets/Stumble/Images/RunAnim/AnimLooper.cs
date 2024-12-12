using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimLooper : MonoBehaviour
{
    public List<Sprite> sprites;
    private Image image;
    private int currentID = 0;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if(sprites.Count == 0) { return; }

    }


}
