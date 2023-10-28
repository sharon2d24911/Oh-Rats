using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().PlaySFX("UIClick");
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().musicSource.volume = (GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().musicSource.volume) / 4;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().PlaySFX("UIClick");
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().musicSource.volume = (GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().musicSource.volume) * 4;
    }

    public void Home()
    {
        Time.timeScale = 1f;
        // StartCoroutine(Camera.main.GetComponent<AudioManager>().FadeTwo(false, "BassyMain", "BassyEvent", 0f, 0f)); // Fade out two music
        SceneManager.LoadScene("TitleScene");
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().StopMusic("BassyMain");
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().StopMusic("BassyEvent");
    }

}
