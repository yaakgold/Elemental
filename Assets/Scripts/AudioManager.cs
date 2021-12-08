using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    private static AudioManager _instance;

    public static AudioManager Instance { get { return _instance; } }

    #endregion

    public Sound[] sounds;
    public GameObject soundPref;

    private void Awake()
    {
        if (_instance != this && _instance != null)
            Destroy(gameObject);
        _instance = this;

        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            GameObject go = Instantiate(soundPref, transform);
            go.name = sound.name;
            sound.source = go.GetComponent<AudioSource>();
            sound.Update();
        }
    }

    public void UpdateSoundValues()
    {
        foreach (Sound sound in sounds)
        {
            sound.Update();
        }
    }

    public void Play(string name, Vector3 audioPosition)
    {
        if (!Array.Exists(sounds, x => x.name == name))
        {
            print(name);
            return;
        }

        Sound sound = Array.Find(sounds, x => x.name == name);
        if(sound.name == name)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).name == name)
                {
                    transform.GetChild(i).transform.position = audioPosition;
                }
            }

            if(!sound.source.isPlaying)
                sound.source.Play();
        }
    }

    private void OnValidate()
    {
        foreach (Sound sound in sounds)
        {
            sound.name = sound.clip.name;
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float vol = 1;
    [Range(.1f, 3)]
    public float pitch = 1;
    public bool loop;

    public bool isSFX = true;

    [HideInInspector]
    public AudioSource source;

    public void Update()
    {
        source.clip = clip;
        source.volume = vol;
        source.pitch = pitch;
        source.loop = loop;
    }
}