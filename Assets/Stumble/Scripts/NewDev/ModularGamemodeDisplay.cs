using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ModularGamemodeDisplay : MonoBehaviour
{

    private ModularController controller;
    private ModularGamemodes gamemodes;

    [SerializeField] private GameObject MapPrefab;
    [SerializeField] private Transform parentTransform;

    [HideInInspector] public List<GameObject> cards;

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
        //Initialize();
        StartCoroutine(DelayedStart());

        Debug.Log("Modular Controller Found...         [Modular Gamemode Display]");
        initialized = true;

        return;
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1f);
        Initialize();
    }

    private void Initialize()
    {
        Debug.LogWarning(gamemodes.modules.Count);

        for (int i = 0; i < gamemodes.modules.Count; i++) {
            //Debug.Log("Spawning");
            GameObject map = Instantiate(MapPrefab, parentTransform.transform);
            cards.Add(map);

            MapCard mapCard = map.GetComponent<MapCard>();
            mapCard.gamemode = gamemodes.modules[i];
            mapCard.title.text = gamemodes.modules[i].title;
            mapCard.modularGamemode = this;

            //Texture2D texture = gamemodes.modules[i].modulePreview.texture;
            Sprite ModeSprite = gamemodes.modules[i].modulePreview;

            if(ModeSprite == null)
            {
                Debug.Log("Empty #" + i);
            }
            else
            {
                Debug.Log("Not Empty #" + i);

                try
                {
                    mapCard.SetImageSprite(ModeSprite);
                }
                catch (Exception ex)
                {
                    //Debug.LogError("Exception in SetImageSprite for sprite #" + i + ": " + ex.Message);
                }
                //mapCard.SetImageSprite(ModeSprite);
            }
        }
    }

    public void LoadLevel(Gamemode gamemode)
    {
        if(GamemodeSelectionManager.Instance != null)
        {
            GamemodeSelectionManager.Instance.LoadLevel(gamemode);
        }
    }
}
