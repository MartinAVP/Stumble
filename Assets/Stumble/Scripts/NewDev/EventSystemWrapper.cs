using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EventSystemWrapper : MonoBehaviour
{
    void OnEnable()
    {
        // Get a reference to the root visual element
        var uiDocument = GetComponent<UIDocument>();
        var rootVisualElement = uiDocument.rootVisualElement;

        // Register for navigation events
        rootVisualElement.RegisterCallback<NavigationCancelEvent>(OnNavCancelEvent);
        rootVisualElement.RegisterCallback<NavigationMoveEvent>(OnNavMoveEvent);
        rootVisualElement.RegisterCallback<NavigationSubmitEvent>(OnNavSubmitEvent);
    }

    private void OnNavSubmitEvent(NavigationSubmitEvent evt)
    {
        Debug.Log($"OnNavSubmitEvent {evt.propagationPhase}");
    }

    private void OnNavMoveEvent(NavigationMoveEvent evt)
    {
        Debug.Log($"OnNavMoveEvent {evt.propagationPhase} - move {evt.move} - direction {evt.direction}");
    }

    private void OnNavCancelEvent(NavigationCancelEvent evt)
    {
        Debug.Log($"OnNavCancelEvent {evt.propagationPhase}");
    }
}
