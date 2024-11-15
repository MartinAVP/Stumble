using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeSelectScreenManager : MonoBehaviour
{
    public enum Direction { Left, Right, Up, Down }

    public static GamemodeSelectScreenManager Instance { get; private set; }

    [SerializeField] private float transitionDuration = 1f;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void InterpolateScreens(GameObject leftScreen, GameObject rightScreen, Direction direction)
    {
        Vector3 offScreenPosition = direction == Direction.Left
            ? new Vector3(-Screen.width, 0, 0)
            : new Vector3(Screen.width, 0, 0);

        // Start the transition
        StartCoroutine(Transition(leftScreen.transform, rightScreen.transform, direction));
    }

    private IEnumerator Transition(Transform outgoing, Transform incoming, Direction direction)
    {
        Vector3 initialIncomingPosition = direction == Direction.Left
            ? new Vector3(Screen.width, 0, 0) // Incoming starts off-screen on the right
            : new Vector3(-Screen.width, 0, 0); // Incoming starts off-screen on the left

        incoming.localPosition = initialIncomingPosition;
        incoming.gameObject.SetActive(true);

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            float smoothStep = Mathf.SmoothStep(0f, 1f, t);

            // Move both screens in the same direction
            outgoing.localPosition = Vector3.Lerp(Vector3.zero, direction == Direction.Left ? new Vector3(-Screen.width, 0, 0) : new Vector3(Screen.width, 0, 0), smoothStep);
            incoming.localPosition = Vector3.Lerp(initialIncomingPosition, Vector3.zero, smoothStep);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final positions are set
        outgoing.localPosition = direction == Direction.Left ? new Vector3(-Screen.width, 0, 0) : new Vector3(Screen.width, 0, 0);
        incoming.localPosition = Vector3.zero;

        // Deactivate outgoing screen
        outgoing.gameObject.SetActive(false);
    }
}
