using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public GameObject props;
    public bool isAlmanac = false;
    public GameObject notification;
    private bool prop = false;

    private void Start()
    {

    }
    public Slider musicSlider;
    public Slider sfxSlider;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        if (props != null)
            props.GetComponent<Prop>().paused = true;
        if (Time.timeScale == 0f)
            prop = true;
        else
            prop = false;
        Time.timeScale = 0f;
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().PlaySFX("UIClick", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][1]);
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().sfxSource.Pause();
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().MusicVolume(0.25f);
        if (isAlmanac)
        {
            notification.SetActive(false);
            GameObject.FindWithTag("GameHandler").GetComponent<DragCombination>().newDonutsNum = 0;
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        if (props != null)
            props.GetComponent<Prop>().paused = false;
        if (!prop)
            Time.timeScale = 1f;
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().PlaySFX("UIClick", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][1]);
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().sfxSource.Play();
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().MusicVolume(1f);
    }

    public void Home()
    {
        Time.timeScale = 1f;
        // StartCoroutine(Camera.main.GetComponent<AudioManager>().FadeTwo(false, "BassyMain", "BassyEvent", 0f, 0f)); // Fade out two music
        SceneManager.LoadScene("TitleScene");
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().StopMusic();
    }
}
