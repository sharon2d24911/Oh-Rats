using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] music, sfx;
    public AudioSource musicSource, sfxSource;
    private float minVolume = 0.0f;
    private IEnumerator fadeIn;
    private IEnumerator fadeOut;

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
        if (fadeOut != null)
        {
            StopCoroutine(fadeOut);
        }
        PlayMusic("BassyFingerBassDrums");
        fadeIn = FadeIn(musicSource,1,2f); 
        StartCoroutine(fadeIn);
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
    
    // Fade out function for future use
    public IEnumerator FadeOut(AudioSource audioSource,float maxVolume,float fadeoutDuration)
    {
        float timer = 0f;
        float currentVolume = audioSource.volume;
        float targetValue = Mathf.Clamp(0, minVolume, maxVolume);
        while (audioSource.volume > 0)
        {
            timer += Time.deltaTime;
            var newVolume = Mathf.Lerp(currentVolume, targetValue, timer / fadeoutDuration);
            audioSource.volume = newVolume;
            yield return null;
        }
    }

    // Fade in function for future use
    public IEnumerator FadeIn(AudioSource audioSource, float maxVolume,float fadeinDuration)
    {
        float timer = 0f;
        float currentVolume = audioSource.volume;
        float targetValue = Mathf.Clamp(1, minVolume, maxVolume);
        while (timer < fadeinDuration)
        {
            timer += Time.deltaTime;
            var newVolume = Mathf.Lerp(currentVolume, targetValue, timer / fadeinDuration);
            audioSource.volume = newVolume;
            yield return null;
        }
    }
         

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfx, x => x.soundName == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip); // Play sfx once
        }
    }
}