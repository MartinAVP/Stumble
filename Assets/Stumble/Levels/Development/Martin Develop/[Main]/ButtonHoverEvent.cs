using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonHoverEvent : MonoBehaviour, IPointerEnterHandler
{
    public UnityEvent onHover;
    // This method will be called when the pointer enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        onHover.Invoke();
    }
}
