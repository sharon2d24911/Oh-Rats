using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] music, sfx, layer;
    public AudioSource musicSource, sfxSource, musicSource2;
    public bool fadingOut1 = false;

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
        }
    }

    public IEnumerator FadeIn(string musicName, float duration, float targetVolume, float speed)
    {
        Sound s = Array.Find(music, x => x.soundName == musicName);
        AudioSource activeSource;
        Debug.Log("vol: " + musicSource.volume);
        // Music source 1 currently not in use, fade it in
        if (musicSource.volume == 0 || fadingOut1 == false)
        {
            activeSource = musicSource;
        }
        else
        {
            activeSource = musicSource2;
        }
        activeSource.clip = s.clip;

        float time = 0f;
        float startVol;

        startVol = 0;
        activeSource.pitch = speed;
        activeSource.Play();
 
        
        Debug.Log("Current source fadein: " + activeSource);

        while (time < duration)
        {
            time += Time.deltaTime;
            Debug.Log("vol: " + musicSource.volume);
            // Fade from the start volume to the target volume
            activeSource.volume = Mathf.Lerp(startVol, targetVolume, time / duration);
            yield return null;
        }
        yield break;
    }

    public IEnumerator FadeOut(string musicName, float duration, float targetVolume, float speed)
    {
        Sound s = Array.Find(music, x => x.soundName == musicName);
        AudioSource activeSource;
        // Music source 1 currently in use, fade it out
        if (musicSource.volume > 0)
        {
            fadingOut1 = true;
            activeSource = musicSource;
        }
        else
        {
            activeSource = musicSource2;
        }
        activeSource.clip = s.clip;
        // If fading out, calculate the duration based on the length of the audio clip

        float time = 0f;
        float startVol;


        startVol = activeSource.volume;
        targetVolume = 0;

        Debug.Log("Current source fadeout: " + activeSource);
        while (time < duration)
        {
            time += Time.deltaTime;
            // Fade from the start volume to the target volume
            activeSource.volume = Mathf.Lerp(startVol, targetVolume, time / duration);
            yield return null;
        }
        yield break;
    }

    public IEnumerator FadeTwo(bool fadeIn, string musicName, string layerName, float duration, float targetVolume)
    {
        Sound s1 = Array.Find(layer, x => x.soundName == layerName);
        Sound s2 = Array.Find(music, x => x.soundName == musicName);
        //layerSource.clip = s1.clip;
        musicSource.clip = s2.clip;
        // If fading out, calculate the duration based on the length of the audio clip
        if (!fadeIn)
        {
            double lengthOfSource1 = (double)musicSource.clip.samples / musicSource.clip.frequency; // Calculate clip's length
            yield return new WaitForSecondsRealtime((float)(lengthOfSource1 - duration));
        }
        float time = 0f;
        //float startVol1 = layerSource.volume;
        float startVol2 = musicSource.volume;
        while (time < duration)
        {
            time += Time.deltaTime;
            // Fade from the start volume to the target volume
            musicSource.Play();
            //layerSource.Play();
            //layerSource.volume = Mathf.Lerp(startVol1, targetVolume, time / duration);
            musicSource.volume = Mathf.Lerp(startVol2, targetVolume, time / duration);
            yield return null;
        }
        yield break;
    }


    public void PlaySFX(string name,float volume,float pan)
    {
        Sound s = Array.Find(sfx, x => x.soundName == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.panStereo = pan;
            sfxSource.PlayOneShot(s.clip,volume); // Play sfx once
        }
    }
}