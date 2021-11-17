using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class SoundParam
{
    public string name;

    public AudioClip audioClip;

    [HideInInspector]
    public AudioSource audioSource;

    [Range(0, 1)]
    public float volume;

    [Range(0, 1)]
    public float pitch = 1.0f;

    public bool loop;

    [Range(0, 1)]
    public float spatialBlend = 1.0f;
}