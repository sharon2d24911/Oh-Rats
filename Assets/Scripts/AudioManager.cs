using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] music, sfx;
    public AudioSource musicSource, sfxSource, musicSource2, musicSource3, musicSource4, musicSource5, musicSource6, musicSource7, musicSource8;
    [HideInInspector] public float playerVolume = 1;
    [HideInInspector] public float playerSfxVolume = 1;

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


    public void StopMusic()
    {
        AudioSource[] musicSources = { musicSource, musicSource2, musicSource3, musicSource4, musicSource5, musicSource6, musicSource7, musicSource8 };

        // Check which musicSources are currently playing
        for (int i = 0; i < musicSources.Length; i++)
        {
            if (musicSources[i].volume > 0)
            {
                musicSources[i].Stop();
                musicSources[i].volume = 0;
            }
        }

    }

    public IEnumerator FadeIn(string[] musicNames, float duration, float[] targetVolume, float[] speed)
    {
        AudioSource[] musicSources = { musicSource, musicSource2, musicSource3, musicSource4, musicSource5, musicSource6, musicSource7, musicSource8 };
        
        AudioSource[] availableSources = new AudioSource[8];
        int index = 0;

        // Check to see which musicSources are available to use
        for (int i = 0; i < musicSources.Length; i++)
        {
            if (musicSources[i].volume == 0 || musicSources[i].mute == true || musicSources[i].clip == null)
            {
                availableSources[index] = musicSources[i];
                Debug.Log("availablesource"+availableSources[index]);
                index++;
            }
        }

        // Initialize the sources with the clips
        int sourcesToInitialize = Math.Min(index, musicNames.Length);
        for (int i = 0; i < sourcesToInitialize; i++)
        {
            if (musicNames[i] != "none")
            {
                Debug.Log("musicFadeIn"+ musicNames[i]);
                Sound s = Array.Find(music, x => x.soundName == musicNames[i]);
                if (s != null)
                {
                    availableSources[i].clip = s.clip;
                    availableSources[i].pitch = speed[i];
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
                    if (targetVolume[i] == 0)
                    {
                        availableSources[i].mute = true;
                    }
                    else
                    {
                        availableSources[i].mute = false;
                        // If player has adjusted volume control
                        if (playerVolume != 1)
                        {
                            musicSources[i].volume = playerVolume;
                        }
                        else
                        {
                            // Update volume for each source
                            availableSources[i].volume = Mathf.Lerp(0, targetVolume[i], time / duration);
                        }
                    }
                }
            }

            yield return null;
        }
    }


    public IEnumerator FadeOut(string musicName1, string musicName2, string musicName3, string musicName4, float duration, float targetVolume)
    {
        AudioSource[] musicSources = { musicSource, musicSource2, musicSource3, musicSource4, musicSource5, musicSource6, musicSource7, musicSource8 };
        string[] musicNames = { musicName1, musicName2, musicName3, musicName4 };

        AudioSource[] playingSources = new AudioSource[8];
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
        for (int i = 0; i < sourcesToFadeOut; i++)
        {
            if (playingSources[i] != null)
            {
                playingSources[i].mute = true;
            }
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
            sfxSource.PlayOneShot(s.clip, volume); // Play sfx once
        }
    }

    public IEnumerator AdjustMusicVolume(string[] musicNames, float[] volume,float[] speed)
    {
        AudioSource[] musicSources = { musicSource, musicSource2, musicSource3, musicSource4, musicSource5, musicSource6, musicSource7, musicSource8 };
        for (int i = 0; i < musicSources.Length; i++)
        {
            for (int j = 0; j < musicNames.Length; j++)
            {
                // If the music source is currently playing
                if (musicNames[j] != "none")
                {
                    Sound matchingSound = Array.Find(music, x => x.soundName == musicNames[j]);

                    if (matchingSound != null && musicSources[i].clip == matchingSound.clip)
                    {
                        musicSources[i].pitch = speed[j];
                        float time = 0f;
                        if (volume[j] != 0)
                        {
                            musicSources[i].mute = false;
                        }
                        else
                        {
                            musicSources[i].mute = true;
                            musicSources[i].volume = 0;
                        }
                        // If player has adjusted volume control
                        if (playerVolume != 1 && volume[j] != 0)
                        {
                            musicSources[i].volume = playerVolume;
                        }
                        else
                        {
                            // Fade in
                            while (time < 3)
                            {
                                time += Time.deltaTime;
                                musicSources[i].volume = Mathf.Lerp(musicSources[i].volume, volume[j], time / 3);
                                yield return null;
                            }
                        }
                    }

                }
            }
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
                // If player adjusted volume
                if (playerVolume != 1 && volume == 1)
                {
                    musicSources[i].volume = playerVolume;
                }
                else
                {
                   musicSources[i].volume = volume;
                }
            }
        }
    }
    public void SFXVolume(float volume)
    {
        if (playerSfxVolume != 1)
        {
            return;
        }
        else
        {
            sfxSource.volume = volume;
        }
    }

    // Music volume control for player
    public void MusicVolumeControl()
    {
        AudioSource[] musicSources = { musicSource, musicSource2, musicSource3, musicSource4, musicSource5, musicSource6, musicSource7, musicSource8 };
        for (int i = 0; i < musicSources.Length; i++)
        {
            // If the music source is currently playing
            if (musicSources[i].mute == false)
            {
                musicSources[i].volume = GameObject.FindWithTag("GameHandler").GetComponent<PauseMenu>().musicSlider.value;
            }
        }
        playerVolume = GameObject.FindWithTag("GameHandler").GetComponent<PauseMenu>().musicSlider.value;
    }

    // Sfx volume control for player
    public void SfxVolumeControl()
    {
        sfxSource.volume = GameObject.FindWithTag("GameHandler").GetComponent<PauseMenu>().sfxSlider.value;
        playerSfxVolume = GameObject.FindWithTag("GameHandler").GetComponent<PauseMenu>().sfxSlider.value;
    }

}