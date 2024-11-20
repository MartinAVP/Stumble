using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private List<VFXplayer> effects = new List<VFXplayer>();
    private List<GameObject> activeEffects = new List<GameObject>();

    public static VFXManager Instance;

    [Header("Assets Import Path")]
    [SerializeField] private string folderPath = "Assets/Stumble/Audio/Assets";
    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    // Singleton
    private void OnEnable()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        PopulateModulesFromFile();
    }

    private void Start()
    {
        //LoadAssets();
        checkForDuplicates();
    }

    private void PopulateModulesFromFile()
    {
        effects = Resources.LoadAll("VFX", typeof(VFXplayer)).Cast<VFXplayer>().ToList();
    }

    /*    public void PlaySound(string soundName, Transform origin)
        {
            AudioHolder holder = GetAudioHolder(soundName);
            if (holder != null)
            {
                //Debug.Log("Found " + soundName);
                AudioSource audioSource = origin.AddComponent<AudioSource>();
                AudioClip clip;
                if (holder.audioClip.Count == 0) { Debug.LogError("The sound " + soundName + " is missing a sound asset"); return; }
                else if (holder.audioClip.Count == 1)
                {
                    clip = holder.audioClip[0];
                }
                else
                {
                    // Select a Random Audio within the variants
                    int id = Random.Range(0, holder.audioClip.Count);
                    clip = holder.audioClip[id];
                }
                // Set to SFX Group
                AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("SFX");
                audioSource.outputAudioMixerGroup = groups[0];

                audioSource.clip = clip;
                //audioSource.time = .03f;
                audioSource.loop = false;
                audioSource.volume = targetVolume + holder.volumeOffset;
                audioSource.playOnAwake = false;
                audioSource.Play();
                StartCoroutine(soundTimer(audioSource, clip.length + 0.1f));

                if (debugLogs)
                {
                    Debug.Log("Player SFX: " + soundName + " for " + origin.transform.name);
                }
            }
            else
            {
                Debug.LogError("The sound " + soundName + " does not exist in the SFX library");
            }
        }*/

    public void PlayVFX(string VFXname)
    {
        if(GetVFXPlayer(VFXname) == null) { Debug.LogError("[Error] There is no VFX in the VFX Library with the name " + VFXname); }
        VFXplayer currentFX = GetVFXPlayer(VFXname);
        GameObject individual = Instantiate(currentFX.player);
        individual.GetComponent<VFXIndividual>().player = currentFX;

        activeEffects.Add(individual);
    }

    public void RemoveActiveVFX(VFXplayer player)
    {
        if (GetVFXPlayer(player) == null) { Debug.LogError("[Error] There is no VFX in the active VFX with the name " + player.title); }

        foreach(var obj in activeEffects)
        {
            if(obj.GetComponent<VFXplayer>() == player)
            {
                activeEffects.Remove(obj);
                break;
            }
        }
    }

    private IEnumerator soundTimer(AudioSource source, float timer)
    {
        yield return new WaitForSeconds(timer);
        if (source != null)
        {
            // Remove the component once its finished
            Destroy(source);
        }
    }

    public VFXplayer GetVFXPlayer(string name)
    {
        foreach (VFXplayer player in effects)
        {
            if (player.title == name)
            {
                return player;
            }
        }
        return null;
    }

    public VFXplayer GetVFXPlayer(VFXplayer playerSeek)
    {
        foreach (VFXplayer player in effects)
        {
            if (player == playerSeek)
            {
                return player;
            }
        }
        return null;
    }

    /*    private void LoadAssets()
        {
            string[] guids = AssetDatabase.FindAssets("t:AudioHolder", new[] { folderPath });

            audios.Clear();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AudioHolder asset = AssetDatabase.LoadAssetAtPath<AudioHolder>(path);
                if (asset != null)
                {
                    audios.Add(asset);
                }
            }

            Debug.Log("Loaded " + audios.Count + " audios from assets folder");
        }*/

    private void checkForDuplicates()
    {
        HashSet<string> titles = new HashSet<string>();
        List<string> duplicates = new List<string>();

        foreach (VFXplayer holder in effects)
        {
            if (titles.Contains(holder.title))
            {
                duplicates.Add(holder.title);
            }
            else
            {
                titles.Add(holder.title);
            }
        }

        if (duplicates.Count > 0)
        {
            Debug.LogError("Duplicate audio titles found: " + string.Join(", ", duplicates));
        }
        else
        {
            if (debugLogs)
            {
                Debug.Log("No duplicate audio titles found.");
            }
        }
    }
}
