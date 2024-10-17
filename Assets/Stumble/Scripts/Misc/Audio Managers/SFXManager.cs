using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private List<AudioHolder> audios = new List<AudioHolder>();
    [SerializeField] private AudioMixer audioMixer;

    public static SFXManager Instance;

    public float targetVolume = 0.3f;

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
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        //LoadAssets();
        checkForDuplicates();
    }

    public void PlaySound(string soundName, Transform origin)
    {
        AudioHolder holder = GetAudioHolder(soundName);
        if (holder != null)
        {
            Debug.Log("Found " + soundName);
            AudioSource audioSource = origin.AddComponent<AudioSource>();
            AudioClip clip;
            if(holder.audioClip.Count == 0){ Debug.LogError("The sound " + soundName + " is missing a sound asset"); return; }
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

    public AudioHolder GetAudioHolder(string name)
    {
        foreach (AudioHolder holder in audios)
        {
            if (holder.title == name)
            {
                return holder;
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

        foreach (AudioHolder holder in audios)
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
