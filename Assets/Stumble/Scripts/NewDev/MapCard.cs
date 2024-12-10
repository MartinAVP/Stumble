using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapCard : MonoBehaviour
{
    public Gamemode gamemode;
    [Space]
    public TextMeshProUGUI title;
    public Image backgroundImage;
    public ModularGamemodeDisplay modularGamemode;


    public void SetImageSprite(Sprite sprite)
    {
        if (sprite == null) return;
        if (backgroundImage == null) return;

        backgroundImage.sprite = sprite;
    }

    public void LoadCurrentLevel()
    {
        modularGamemode.LoadLevel(gamemode);
    }

/*    public void SetImageTexture(Texture2D texture)
    {
        if (texture == null) return;

        backgroundImage.sprite.texture = texture;
    }*/
}
