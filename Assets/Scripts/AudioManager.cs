using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] music, sfx;
    public AudioSource musicSource, sfxSource, musicSource2, musicSource3, musicSource4, musicSource5, musicSource6, musicSource7, musicSource8;
    //private bool fadingOut1 = false;

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

    public void StopMusic(string musicName1, string musicName2, string musicName3, string musicName4)
    {
        AudioSource[] musicSources = { musicSource, musicSource2, musicSource3, musicSource4, musicSource5, musicSource6, musicSource7, musicSource8 };
        string[] musicNames = { musicName1, musicName2, musicName3, musicName4 };

        AudioSource[] playingSources = new AudioSource[4];
        int index = 0;

        // Check which musicSources are currently playing
        for (int i = 0; i < musicSources.Length; i++)
        {
            if (musicSources[i].volume > 0)
            {
                playingSources[index] = musicSources[i];
                index++;
            }
        }

        // Adjusting the number of iterations to the number of sources actually playing
        int sourcesToFadeOut = Math.Min(index, musicNames.Length);
        for (int i = 0; i < sourcesToFadeOut; i++)
        {
            if (musicNames[i] != "none" && playingSources[i] != null)
            {
                playingSources[i].clip = Array.Find(music, x => x.soundName == musicNames[i]).clip;
            }
        }
        for (int i = 0; i < playingSources.Length; i++)
        {
            if (playingSources[i] != null)
            {
                playingSources[i].Stop();
            }
        }
    }

    public IEnumerator FadeIn(string musicName1, string musicName2, string musicName3, string musicName4, float duration, float targetVolume, float speed)
    {
        AudioSource[] musicSources = { musicSource, musicSource2, musicSource3, musicSource4, musicSource5, musicSource6, musicSource7, musicSource8 };
        string[] musicNames = { musicName1, musicName2, musicName3, musicName4 };

        AudioSource[] availableSources = new AudioSource[8];
        int index = 0;

        // Check to see which musicSources are available to use
        for (int i = 0; i < musicSources.Length; i++)
        {
            if (musicSources[i].volume == 0)
            {
                availableSources[index] = musicSources[i];
                index++;
            }
        }

        // Initialize the sources with the clips
        int sourcesToInitialize = Math.Min(index, musicNames.Length);
        for (int i = 0; i < sourcesToInitialize; i++)
        {
            if (musicNames[i] != "none")
            {
                Sound s = Array.Find(music, x => x.soundName == musicNames[i]);
                if (s != null)
                {
                    availableSources[i].clip = s.clip;
                    availableSources[i].pitch = speed;
                    availableSources[i].Play();
                }
            }
        }

        float time = 0f;

        // Fade in all music sources together
        while (time < duration)
        {
            time += Time.deltaTime;

            for (int i = 0; i < sourcesToInitialize; i++)
            {
                if (musicNames[i] != "none")
                {
                    // Update volume for each source
                    availableSources[i].volume = Mathf.Lerp(0, targetVolume, time / duration);
                }
            }

            yield return null;
        }
    }


    public IEnumerator FadeOut(string musicName1, string musicName2, string musicName3, string musicName4, float duration, float targetVolume)
    {
        AudioSource[] musicSources = { musicSource, musicSource2, musicSource3, musicSource4, musicSource5, musicSource6, musicSource7, musicSource8 };
        string[] musicNames = { musicName1, musicName2, musicName3, musicName4 };

        AudioSource[] playingSources = new AudioSource[4];
        int index = 0;

        // Check which musicSources are currently playing
        for (int i = 0; i < musicSources.Length; i++)
        {
            if (musicSources[i].volume > 0)
            {
                playingSources[index] = musicSources[i];
                index++;
            }
        }

        // Adjusting the number of iterations to the number of sources actually playing
        int sourcesToFadeOut = Math.Min(index, musicNames.Length);
        for (int i = 0; i < sourcesToFadeOut; i++)
        {
            if (musicNames[i] != "none" && playingSources[i] != null)
            {
                playingSources[i].clip = Array.Find(music, x => x.soundName == musicNames[i]).clip;
            }
        }

        float time = 0f;
        float[] startVolumes = new float[sourcesToFadeOut];

        // Store the start volumes of each source
        for (int i = 0; i < sourcesToFadeOut; i++)
        {
            if (musicNames[i] != "none" && playingSources[i] != null)
            {
                startVolumes[i] = playingSources[i].volume;
            }
        }

        // Fade out all music sources together
        while (time < duration)
        {
            time += Time.deltaTime;

            for (int i = 0; i < sourcesToFadeOut; i++)
            {
                if (musicNames[i] != "none" && playingSources[i] != null)
                {
                    // Update volume for each source
                    playingSources[i].volume = Mathf.Lerp(startVolumes[i], targetVolume, time / duration);
                }
            }

            yield return null;
        }
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

    public void MusicVolume(float volume)
    {
        AudioSource[] musicSources = { musicSource, musicSource2, musicSource3, musicSource4, musicSource5, musicSource6, musicSource7, musicSource8 };
        for (int i = 0; i < musicSources.Length; i++)
        {
            // If the music source is currently playing
            if (musicSources[i].volume > 0)
            {
                musicSources[i].volume = volume;
            }
        }
    }
    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}