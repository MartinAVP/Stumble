using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICameraView : MonoBehaviour
{
    //[SerializeField] private RawImage imageTexture;
    [SerializeField] public Camera characterCam;
    public RenderTexture cameraTexture;

    // Start is called before the first frame update
/*    void Awake()
    {

    }

    private void OnGUI()
    {
        // Set the position and size of the button
        if (GUI.Button(new Rect(10, 10, 150, 30), "Enable Cam"))
        {
            EnableCharacterCam();
            //Debug.Log("Select Random Gamemodes");
        }
    }*/

    public void EnableCharacterCam()
    {
        characterCam.gameObject.SetActive(true);
        characterCam.enabled = true;

        float desiredHeight = 10.0f;
        characterCam.orthographicSize = desiredHeight / 2; 
        float desiredWidth = 10.0f;
        characterCam.orthographicSize = desiredWidth / (2 * characterCam.aspect);

        characterCam.clearFlags = CameraClearFlags.SolidColor;
        characterCam.backgroundColor = Color.clear;

        characterCam.orthographicSize = 1.29f;

        //characterCam = this.AddComponent<Camera>();

/*        characterCam = GetComponent<Camera>();*/
        cameraTexture = new RenderTexture(1024, 1024, 16, RenderTextureFormat.ARGB32);
        cameraTexture.Create();

        characterCam.targetTexture = cameraTexture;

        cameraTexture.Release();

        //imageTexture.texture = cameraTexture;

/*        this.gameObject.SetActive(false);
        characterCam.enabled = false;*/
    }

    public void SetTextureForRenderCam(RawImage image)
    {
        image.texture = cameraTexture;
    }
}
