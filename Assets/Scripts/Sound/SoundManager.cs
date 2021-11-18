using UnityEngine.Audio;
using System;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{

    public SoundParam[] sounds;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        Play("main_theme", this.gameObject);
    }


    public void Play(string name, GameObject sourceObject)
    {
        try
        {
            if(GetComponent<AudioSource>() != null)
            {
                Destroy(GetComponent<AudioSource>());
            }

            SoundParam sp = Array.Find(sounds, sound => sound.name.Equals(name));

            //Si vide, on l'initialise (il va avoir la position de l'objet child)
            if (sp.audioSource == null)
            {

                sp.audioSource = sourceObject.AddComponent<AudioSource>();

                sp.audioSource.clip = sp.audioClip;
                sp.audioSource.volume = sp.volume;

                sp.audioSource.pitch = sp.pitch;
                sp.audioSource.loop = sp.loop;

                sp.audioSource.spatialBlend = sp.spatialBlend;

            }

            if (sp.loop == true)
            {
                sp.audioSource.Play();
            }
            {
                sp.audioSource.PlayOneShot(sp.audioClip);
            }
        }
        catch (NullReferenceException)
        {
            Debug.Log("Audio clip" + name + " non existant");
        }
    }
}
