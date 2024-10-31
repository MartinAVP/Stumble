using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EmoteWheelController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    private string controlScheme;
    [SerializeField] private GameObject pointerCenter;
    [SerializeField] private Transform viewer;

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
    }

    public static float Angle(Vector2 vector2)
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
