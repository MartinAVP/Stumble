using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Users;

public class LocalMultiplayer : MonoBehaviour
{
    public GameObject playerPrefab; // The prefab for your player character
    public Transform[] spawnPoints; // Array of spawn points for players
    public InputActionAsset playerControls; // Assign your InputActionAsset here

    private List<PlayerInput> players = new List<PlayerInput>();
    private Dictionary<InputDevice, PlayerInput> deviceToPlayerMap = new Dictionary<InputDevice, PlayerInput>();

    private void OnEnable()
    {
        // Register to input device changes
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        // Unregister to avoid memory leaks
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added)
        {
            // Handle new device connection
            AssignDeviceToPlayer(device);
        }
    }

    private void AssignDeviceToPlayer(InputDevice device)
    {
        // Check if the device is already assigned
        if (deviceToPlayerMap.ContainsKey(device))
        {
            Debug.Log("Device already assigned.");
            return;
        }

        // Instantiate a new player object
        GameObject playerObject = Instantiate(playerPrefab);

        // Find a spawn point for the new player
        Transform spawnPoint = spawnPoints[players.Count % spawnPoints.Length];
        playerObject.transform.position = spawnPoint.position;

        // Create a PlayerInput component and assign the input actions
        PlayerInput playerInput = playerObject.AddComponent<PlayerInput>();
        playerInput.actions = playerControls;

        // Optionally, assign the device to the PlayerInput component
        playerInput.defaultControlScheme = "Gamepad"; // Adjust according to your control schemes

        // Add player to the list and map
        players.Add(playerInput);
        deviceToPlayerMap[device] = playerInput;
    }
}
