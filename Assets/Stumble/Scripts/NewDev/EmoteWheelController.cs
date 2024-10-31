﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EmoteWheelController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    private string controlScheme;
    [SerializeField] private GameObject pointerCenter;
    [SerializeField] private Transform viewer;
    [SerializeField] private Transform[] wheelPieces;

    private bool isActive = false;

    private void Start()
    {
        controlScheme = this.GetComponent<PlayerInput>().currentControlScheme;

        if (controlScheme == "Keyboard")
        {
            Debug.Log("Keyboard");
        }
        else if(controlScheme == "Controller")
        {
            Debug.Log("Controller");
        }
    }

    private void LateUpdate()
    {
        //Debug.Log(playerCamera.ScreenToViewportPoint(Input.mousePosition));
        Debug.DrawRay(pointerCenter.transform.GetComponent<RectTransform>().anchoredPosition, playerCamera.ScreenToViewportPoint(Input.mousePosition), Color.red);

        Vector3 targetDir = (playerCamera.ScreenToViewportPoint(Input.mousePosition) - new Vector3(.5f, .5f, 0f)) * 2;

        Vector3 cross = Vector3.Cross(targetDir, playerCamera.transform.forward);
        viewer.GetComponent<RectTransform>().anchoredPosition = cross;
        //Debug.Log(targetDir);
        //float Angle = Vector2.Angle(pointerCenter.transform.position, targetDir);
        //Debug.Log(Angle(targetDir));

        Quaternion quaternion = Quaternion.identity;
        quaternion.eulerAngles = new Vector3(0, 0, Angle(targetDir) * -1);

        pointerCenter.transform.rotation = quaternion;

        foreach (Transform t in wheelPieces)
        {
            t.GetComponent<Image>().color = Color.white;
        }

        //wheelPieces[GetSelectedID(Angle(targetDir))].GetComponent<Image>().color = Color.red;
        wheelPieces[GetSelected(Angle(targetDir), wheelPieces.Length)].GetComponent<Image>().color = Color.red;
    }

    public void EmoteWheelKeybind(InputAction.CallbackContext callback)
    {
        if (callback.performed)
        {

        }

        if (callback.canceled)
        {

        }
    }


    // Hard Coded
    private int GetSelectedID(float angle)
    {
        float offSet = (360 / wheelPieces.Length) / 2;

        if((angle > 0f && angle <= 45f - offSet) || (angle > 360 - offSet && angle <= 360))
        {
            return 0;
        }
        else if (angle > 45f - offSet && angle <= 90f - offSet)
        {
            return 1;
        }
        else if (angle > 90f - offSet && angle <= 135f - offSet)
        {
            return 2;
        }
        else if (angle > 135f - offSet && angle <= 180f - offSet)
        {
            return 3;
        }
        else if (angle > 180f - offSet && angle <= 225f - offSet)
        {
            return 4;
        }
        else if (angle > 225f - offSet && angle <= 270f - offSet)
        {
            return 5;
        }
        else if (angle > 270f - offSet && angle <= 315f - offSet)
        {
            return 6;
        }
        else if (angle > 315f - offSet && angle <= 360f - offSet)
        {
            return 7;
        }

        return -1;
    }

    // Modular (*￣3￣)╭
    private int GetSelected(float angle, int pieces)
    {
        float pieceTreshold = 360 / pieces;
        float offSet = pieceTreshold / 2;

        if(pieces > 2)
        {
            // Special Case 0
            if ((angle > 0f && angle <= pieceTreshold - offSet) || (angle > 360 - offSet && angle <= 360))
            {
                return 0;
            }

            for (int i = 1; i < pieces; i++)
            {
                if (angle > (pieceTreshold * i) - offSet && angle <= pieceTreshold * (i + 1) - offSet)
                {
                    return i;
                }
            }
        }

        return -1;

    }

    private static float Angle(Vector2 vector2)
    {
        if (vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
        }
    }
}
