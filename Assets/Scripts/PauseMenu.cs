using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [HideInInspector] public bool paused = false;
    private bool prop = false;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        paused = true;
        if (Time.timeScale == 0f)
            prop = true;
        else
            prop = false;
        Time.timeScale = 0f;
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().PlaySFX("UIClick",GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][1]);
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().MusicVolume(0.25f);
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        paused = false;
        if (!prop)
            Time.timeScale = 1f;
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().PlaySFX("UIClick", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][1]);
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().MusicVolume(1f);
    }

}
