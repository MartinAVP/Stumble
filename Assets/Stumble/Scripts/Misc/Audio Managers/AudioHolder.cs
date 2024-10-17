using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Audios/Audio")]
public class AudioHolder : ScriptableObject
{
    public string title;
    [SerializeField]public List<AudioClip> audioClip = new List<AudioClip>(1);
    [Tooltip("an override for the volume.")]
    [Space]
    public float volumeOffset;
}