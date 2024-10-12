using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MovingPlatformEvent
{
    PreMove,
    Move,
    PostMove,
    Final
}

public static class MovingPlatformEventBus
{
    private static readonly IDictionary<MovingPlatformEvent, UnityEvent> events = new Dictionary<MovingPlatformEvent, UnityEvent>();

    public static void Subscribe(MovingPlatformEvent eventType, UnityAction listener)
    {
        UnityEvent newEvent;

        if(events.TryGetValue(eventType, out newEvent))
        {
            newEvent.AddListener(listener);
        }
        else
        {
            newEvent = new UnityEvent();
            newEvent.AddListener(listener);
            events.Add(eventType, newEvent);
        }
    }

    public static void Unsubscribe(MovingPlatformEvent eventType, UnityAction unlistener)
    {
        UnityEvent gameEvent;

        if(events.TryGetValue(eventType, out gameEvent))
        {
            gameEvent.RemoveListener(unlistener);
        }
    }

    public static void Publish(MovingPlatformEvent eventType)
    {
        UnityEvent gameEvent;

        if(events.TryGetValue(eventType,out gameEvent))
        {
            gameEvent.Invoke();
        }
    }
}
