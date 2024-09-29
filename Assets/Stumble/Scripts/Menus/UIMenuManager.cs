using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIMenuManager : MonoBehaviour
{
    [SerializeField] Button b_GrandPrixGameMode;

    private void OnEnable()
    {
        b_GrandPrixGameMode.onClick.AddListener(GrandPrixGamemodeSelect);
    }
    private void OnDisable()
    {
        b_GrandPrixGameMode.onClick.RemoveListener(GrandPrixGamemodeSelect);
        
    }

    private void GrandPrixGamemodeSelect()
    {

    }

/*    public void NavigateThroughUI(InputAction.CallbackContext context)
    {
        // Check for navigation input (e.g., using joystick or arrow keys)
        if (playerData.inputDevice.TryGetFeatureValue(CommonUsages.primaryAxis, out Vector2 axis))
        {
            if (axis.y > navigationThreshold)
            {
                // Navigate up
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp());
            }
            else if (axis.y < -navigationThreshold)
            {
                // Navigate down
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown());
            }
        }

        // Handle button press
        if (playerData.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonPressed) && primaryButtonPressed)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
            }
        }
    }*/

}
