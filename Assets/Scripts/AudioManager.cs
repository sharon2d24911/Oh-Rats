using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] music, sfx, layer;
    public AudioSource musicSource, sfxSource, layerSource;

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
        //StartCoroutine(Fade(true,"BassyMain",1f,1f)); // Fade in
        //StartCoroutine(Fade(false, "BassyMain", 2f, 0f)); // Fade out
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
            layerSource.Play();
        }
    }

    public void StopMusic(string name)
    {
        Sound s = Array.Find(music, x => x.soundName == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Stop();
            layerSource.Stop();
        }
    }

    // Fade out function for future use
    public IEnumerator Fade(bool fadeIn, string musicName, float duration, float targetVolume)
    {
        Sound s = Array.Find(music, x => x.soundName == musicName);
        musicSource.clip = s.clip;
        // If fading out, calculate the duration based on the length of the audio clip

        float time = 0f;
        float startVol;
        if (fadeIn)
        {
           startVol = 0;
           musicSource.Play();
        }
        else
        {
            startVol = musicSource.volume;
            targetVolume = 0;
        }
        
        Debug.Log("Start Volume: " + startVol);
        Debug.Log("Target Volume: " + targetVolume);
        Debug.Log("Time: " + time);
        Debug.Log("Duration: " + duration);
        while (time < duration)
        {
            time += Time.deltaTime;
            // Fade from the start volume to the target volume
            musicSource.volume = Mathf.Lerp(startVol, targetVolume, time / duration);
            yield return null;
        }
        yield break;
    }

    public IEnumerator FadeTwo(bool fadeIn, string musicName, string layerName, float duration, float targetVolume)
    {
        Sound s1 = Array.Find(layer, x => x.soundName == layerName);
        Sound s2 = Array.Find(music, x => x.soundName == musicName);
        layerSource.clip = s1.clip;
        musicSource.clip = s2.clip;
        // If fading out, calculate the duration based on the length of the audio clip
        if (!fadeIn)
        {
            double lengthOfSource1 = (double)musicSource.clip.samples / musicSource.clip.frequency; // Calculate clip's length
            yield return new WaitForSecondsRealtime((float)(lengthOfSource1 - duration));
        }
        float time = 0f;
        float startVol1 = layerSource.volume;
        float startVol2 = musicSource.volume;
        while (time < duration)
        {
            time += Time.deltaTime;
            // Fade from the start volume to the target volume
            musicSource.Play();
            layerSource.Play();
            layerSource.volume = Mathf.Lerp(startVol1, targetVolume, time / duration);
            musicSource.volume = Mathf.Lerp(startVol2, targetVolume, time / duration);
            yield return null;
        }
        yield break;
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