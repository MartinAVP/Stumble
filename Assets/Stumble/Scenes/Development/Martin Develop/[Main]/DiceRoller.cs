using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    // Target face to show after rolling
    private int targetFace;
    private float interpolationSpeed = 5f; // Adjust for faster or slower interpolation
    private bool isRolling;

    // Method to initiate the roll
    public void RollDice(int face)
    {
        if (face < 1 || face > 6)
        {
            Debug.LogError("Face must be between 1 and 6.");
            return;
        }

        targetFace = face;
        isRolling = true;
    }

    private void Update()
    {
        if (isRolling)
        {
            // Interpolate towards the target rotation
            Quaternion targetRotation = GetTargetRotation(targetFace);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * interpolationSpeed);

            // Check if the rotation is close enough to the target
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                transform.rotation = targetRotation; // Snap to target to avoid drifting
                isRolling = false; // Stop rolling
            }
        }
    }

    private Quaternion GetTargetRotation(int face)
    {
        // Define the target rotations for each face (adjust as needed)
        switch (face)
        {
            case 1: return Quaternion.Euler(0, 0, 0);
            case 2: return Quaternion.Euler(0, 90, 0);
            case 3: return Quaternion.Euler(0, 180, 0);
            case 4: return Quaternion.Euler(0, 270, 0);
            case 5: return Quaternion.Euler(90, 0, 0);
            case 6: return Quaternion.Euler(-90, 0, 0);
            default: return Quaternion.identity;
        }
    }

    private void OnGUI()
    {
        // Set the button's position and size
        Rect buttonRect = new Rect(100, 100, 200, 50);

        // Create a button
        if (GUI.Button(buttonRect, "Click Me"))
        {
            RollDice(Random.Range(1, 7));
        }
    }
}
