using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;

public class ModularGamemodeDisplay : MonoBehaviour
{

    private ModularController controller;
    private ModularGamemodes gamemodes;

    [SerializeField] private GameObject MapPrefab;

    public static ModularGamemodeDisplay Instance { get; private set; }
    public bool initialized { get; private set; }

    // Singleton
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            //Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        Task Setup = setup();
    }

    private void Start()
    {
    }

    private async Task setup()
    {
        while (ModularController.Instance == null || ModularController.Instance.enabled == false)
        {
            await Task.Delay(1);
        }
        controller = ModularController.Instance;
        gamemodes = ModularController.Instance.gamesLib;

        // Once it finds it initialize the scene
        Initialize();
        Debug.Log("Modular Controller Found...         [Modular Gamemode Display]");
        initialized = true;

        return;
    }

    private void Initialize()
    {
        Debug.LogWarning(gamemodes.modules.Count);

        for (int i = 0; i < gamemodes.modules.Count; i++) {
            //Debug.Log("Spawning");
            GameObject map = Instantiate(MapPrefab, this.transform);
            MapCard mapCard = map.GetComponent<MapCard>();
            mapCard.gamemode = gamemodes.modules[i];
            mapCard.title.text = gamemodes.modules[i].name;
/*            if (gamemodes.modules[i].modulePreview != null)
            {
                mapCard.backgroundImage.sprite = gamemodes.modules[i].modulePreview;
            }
            else
            {
                Debug.Log("Null Image");
            }*/
            /*            if (gamemodes.modules[i].title != null)
                        {
                            mapCard.title.text = gamemodes.modules[i].title;
                        }
                        if (gamemodes.modules[i].modulePreview != null)
                        {
                            mapCard.backgroundImage.sprite = gamemodes.modules[i].modulePreview;
                        }*/
        }

        foreach (var gamemode in gamemodes.modules) {

        }
    }
}
