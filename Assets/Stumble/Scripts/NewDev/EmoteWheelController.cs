using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class EmoteWheelController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    private string controlScheme;
    [SerializeField] private GameObject pointerCenter;
    [SerializeField] private Transform[] wheelPieces;
    [SerializeField] private GameObject emoteWheelPanel;

    public event Action<int> PlayEmote;

    private scheme currentScheme;
    private Vector2 controllerV2 = Vector2.zero;

    private bool isActive = false;
    public int currentEmoteID = 0;

    private void Start()
    {
        emoteWheelPanel.SetActive(false);
        controlScheme = this.GetComponent<PlayerInput>().currentControlScheme; //

        if (controlScheme == "Keyboard")
        {
            Debug.Log("Keyboard");
            currentScheme = scheme.Keyboard;
        }
        else if(controlScheme == "Controller")
        {
            Debug.Log("Controller");
            currentScheme = scheme.Controller;
        }
    }

    private void LateUpdate()
    {
        if(!isActive) { return; }
        //Debug.Log(playerCamera.ScreenToViewportPoint(Input.mousePosition));
        Debug.DrawRay(pointerCenter.transform.GetComponent<RectTransform>().anchoredPosition, playerCamera.ScreenToViewportPoint(Input.mousePosition), Color.red);

        Vector3 targetDir = (playerCamera.ScreenToViewportPoint(Input.mousePosition) - new Vector3(.5f, .5f, 0f)) * 2;

        Quaternion quaternion = Quaternion.identity;

        if(currentScheme == scheme.Controller)
        {
            //targetDir = controllerV2.normalized;
            targetDir = new Vector3(controllerV2.x, controllerV2.y, 0f);
        }

        quaternion.eulerAngles = new Vector3(0, 0, Angle(targetDir) * -1);

        //pointerCenter.transform.rotation = quaternion;
        pointerCenter.GetComponent<RectTransform>().localRotation = quaternion;

        foreach (Transform t in wheelPieces)
        {
            t.GetComponent<Image>().color = Color.white;
        }

        //wheelPieces[GetSelectedID(Angle(targetDir))].GetComponent<Image>().color = Color.red;
        int id = GetSelected(Angle(targetDir), wheelPieces.Length);

        Debug.Log(id);
        if(id == -1) { return; }
        wheelPieces[id].GetComponent<Image>().color = Color.yellow;
        currentEmoteID = id;

        //Debug.Log(id);
    }

    public void EmoteWheelKeybind(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            emoteWheelPanel.SetActive(true);
            isActive = true;
        }

        if (context.performed)
        {
            //Debug.Log("The Scheme is " + currentScheme);
            if(currentScheme == scheme.Controller)
            {
                Vector2 raw = context.ReadValue<Vector2>();
                controllerV2 = raw.normalized;
                //Debug.Log("INPUT LOG: X:" + raw.x + " Y:" + raw.y);
            }
        }

        if (context.canceled)
        {
            isActive = false;
            emoteWheelPanel.SetActive(false);
            PlayEmote?.Invoke(currentEmoteID);
        }
    }

    public void EmoteWheelQuickAccess(InputAction.CallbackContext context)
    {

        Vector2 raw = context.ReadValue<Vector2>();
/*        Debug.Log(raw);

        Debug.Log(raw.x);
        Debug.Log(raw.y);*/

        if (context.performed)
        {
            // X - Values
            if (raw.x >= 0.5f && raw.y == 0)
            {
                PlayEmote?.Invoke(2);
            }
            if(raw.x <= -0.5f && raw.y == 0)
            {
                PlayEmote?.Invoke(6);
            }

            // Y - Values
            if (raw.y >= 0.5f && raw.x == 0)
            {
                PlayEmote?.Invoke(0);
            }
            if (raw.y <= -0.5f && raw.x == 0)
            {
                PlayEmote?.Invoke(4);
            }
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

    private enum scheme
    {
        Keyboard,
        Controller
    }
}
