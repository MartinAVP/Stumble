using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ModularGamemodes))]
public class ModularController : MonoBehaviour
{
    public int levelId;

    private ModularGamemodes gamesLib;
    [Tooltip("The amount of modes that party gamemode will have.")]
    [SerializeField] private int partyGameSize;


    public static ModularController Instance;
    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(this.gameObject); }
    }

    private void Start()
    {
        if(GetComponent<ModularGamemodes>() == null)
        {
            Debug.LogError("Modular Gamemodes Library Not Initialized");
            return; 
        }
        gamesLib = GetComponent<ModularGamemodes>();

        // Set the ID of Level to -1. Meaning Menu, no level
        levelId = -1;
    }
/*    
    private void OnGUI()
    {
        // Set the position and size of the button
        if (GUI.Button(new Rect(10, 10, 150, 30), "Initialize Party"))
        {
            InitializeParty();
            //Debug.Log("Select Random Gamemodes");
        }
    }*/

    /// <summary>
    ///  Initialize Party starts the main gamemode with preselected gamemode Quantity
    /// </summary>
    public void InitializeParty()
    {
        gamesLib.SelectGamemodes(partyGameSize);

        LoadingScreenManager.Instance.StartTransition(true);
        StartCoroutine(loadLevelDelay());

        if (MenuMusicController.Instance != null)
        {
            MenuMusicController.Instance.EndMusic(2.8f);
        }
    }

    private IEnumerator loadLevelDelay()
    {
        yield return new WaitForSeconds(4f);
        AdvanceLevels();
    }

    /// <summary>
    /// Advance Levels is what gets the player from scene one to scene two.
    /// It also manages when they've runned out of race modes to test.
    /// </summary>
    public void AdvanceLevels()
    {
        // Advance to the next levelId
        levelId++;

        // Check if last scene
        if(levelId >= gamesLib.activeGamemodes.Count)
        {
            // Load Podium Scene
            Debug.LogError("Loading Podium Scene");
            SceneManager.LoadScene("Podium");

            // Reset Level ID
            levelId = 0;
            return;
        }

        // Load The Next Scene
        Debug.Log("Loading New Active Gamemode " + gamesLib.activeGamemodes[levelId].scene + "   (" + (levelId + 1) + "/" + gamesLib.activeGamemodes.Count + ")");
        SceneManager.LoadScene(gamesLib.activeGamemodes[levelId].scene);
    }
}
