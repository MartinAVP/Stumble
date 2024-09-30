using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenuManager : MonoBehaviour
{
    [SerializeField] private Button b_GrandPrixGameMode;
    [SerializeField] private MultiplayerEventSystem eventSystem;

    [SerializeField] private GameObject gamemodesPanel;
    [SerializeField] private string grandPrixLevel;
    [SerializeField] private string bumpArenaLevel;

    //public Vector3 targetPosition; // Set this in the inspector or dynamically
    public float duration = 1f; // Duration in seconds

    private void OnEnable()
    {
        b_GrandPrixGameMode.onClick.AddListener(GrandPrixGamemodeSelect);
    }
    private void OnDisable()
    {
        b_GrandPrixGameMode.onClick.RemoveListener(GrandPrixGamemodeSelect);
        
    }

    public void GrandPrixGamemodeSelect()
    {
        Debug.Log("Grand Prix Selected");
        SceneManager.LoadScene(grandPrixLevel);
    
        //StartCoroutine(InterpolatePosition(gamemodesPanel.transform, new Vector3(0, 3000, 0), duration));
        //eventSystem.currentSelectedGameObject = ;
    }

    public void BumpArenaGamemodeSelect()
    {
        Debug.Log("Bump Arena Selected");
        SceneManager.LoadScene(bumpArenaLevel);
    }

    private IEnumerator InterpolatePosition(Transform targetObject, Vector3 target, float duration)
    {
        Vector3 startPosition = transform.position; // Get the initial position
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; // Increase elapsed time
            float t = elapsedTime / duration; // Calculate the interpolation factor
            targetObject.transform.position = Vector3.Lerp(startPosition, target, t); // Interpolate position
            yield return null; // Wait for the next frame
        }

        targetObject.transform.position = target; // Ensure the final position is set
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
