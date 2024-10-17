using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackupKicker : MonoBehaviour
{
    [SerializeField] private bool kickBackup;
    [Range(1,4)]
    [SerializeField] private int playerFaker = 1;

    public static BackupKicker Instance { get; private set; }
    public bool initialized { get; private set; }

    public void start()
    {
        Debug.Log("Howdy Neighbour");
        initialize();
    }

    private void initialize()
    {
        if(PlayerDataManager.Instance == null)
        {
            GameObject controller = new GameObject("Player Data Manager Backup Kicker");
            PlayerDataManager data = controller.AddComponent<PlayerDataManager>();
            GameObject playerPrefab = FindAnyObjectByType<PlayerInputManager>().playerPrefab;

            GameObject[] players = new GameObject[playerFaker];
            for (int i = 0; i < playerFaker; i++)
            {
                GameObject fakePlayer = Instantiate(playerPrefab);
                data.AddPlayer(fakePlayer.GetComponentInChildren<PlayerInput>());
                players[i] = fakePlayer;
            }

            foreach(GameObject player in players)
            {
                Destroy(player.gameObject);
            }
        }

        if(GameController.Instance == null)
        {
            GameObject controller = new GameObject("Game Controller Backup Kicker");
            controller.AddComponent<GameController>();
        }

        Debug.LogWarning("Backup Kicked In Systems Starting...");
    }
}

[CustomEditor(typeof(BackupKicker))]
class BackupKickerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Initialize Backup Generator"))
        {
            if (Application.isPlaying)
            {
                target.GetComponent<BackupKicker>().start();
            }
            //Debug.Log("It's alive: " + target);

        }

    }
}
