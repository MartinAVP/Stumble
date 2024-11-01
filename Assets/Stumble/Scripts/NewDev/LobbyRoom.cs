using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoom : MonoBehaviour
{
    public Camera roomCam;
    public Transform roomSpawn;

    public RenderTexture cameraTexture { get; set; }

    private void Start()
    {
        SetupCamera();
    }

    public void SetupCamera()
    {
        roomCam.gameObject.SetActive(true);
        roomCam.enabled = true;

        cameraTexture = new RenderTexture(1024, 1024, 16, RenderTextureFormat.ARGB32);
        cameraTexture.Create();

        roomCam.targetTexture = cameraTexture;

        cameraTexture.Release();
    }

    public void SetTextureForRenderCam(RawImage image)
    {
        image.texture = cameraTexture;
    }
}
