using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] music, sfx;
    public AudioSource musicSource, sfxSource;
    public float maxVolume;
    private float minVolume = 0.0f;
    private float Volume;
    /// Attack is fade-in speed, decay is fade-out
    public float Attack;
    public float Decay;

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

    private void Start()
    {
        // Background music play when game starts
        PlayMusic("bassy");
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(music, x => x.soundName == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name,float duration, float targetVolume)
    {
        Sound s = Array.Find(sfx, x => x.soundName == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.volume += Mathf.Clamp(Volume + Attack * Time.deltaTime, minVolume, maxVolume); // Increase the volume gradually (fade in).
            sfxSource.PlayOneShot(s.clip); // Play sfx once
        }
    }
}