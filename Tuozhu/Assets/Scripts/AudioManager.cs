using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource = null;
    public List<AudioSource> soundSources = new List<AudioSource>();

    private float musicVolume = 1.0f;
    private float soundVolume = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSoundVolume(float volume)
    {
        soundVolume = volume;
        foreach (var source in soundSources)
        {
            if (source != null)
            {
                source.volume = soundVolume;
            }
        }
    }
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        if (musicSource.clip != musicClip)
        {
            musicSource.clip = musicClip;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    public void PlaySound(AudioClip soundClip)
    {
        //查找空闲的AudioSource
        AudioSource availableSource = null;
        foreach (var source in soundSources)
        {
            if (!source.isPlaying)
            {
                availableSource = source;
                break;
            }
        }

        //如果没有则创建一个新的
        if (availableSource == null)
        {
            availableSource = gameObject.AddComponent<AudioSource>();
            soundSources.Add(availableSource);
        }

        //播放音效
        availableSource.clip = soundClip;
        availableSource.volume = soundVolume;
        availableSource.Play();
    }
}
