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
        Camera.main.GetComponent<AudioManager>().PlaySFX("UIClick");
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Camera.main.GetComponent<AudioManager>().PlaySFX("UIClick");
    }

    public void Home()
    {
        Time.timeScale = 1f;
        // StartCoroutine(Camera.main.GetComponent<AudioManager>().FadeTwo(false, "BassyMain", "BassyEvent", 0f, 0f)); // Fade out two music
        SceneManager.LoadScene("TitleScene");
        Camera.main.GetComponent<AudioManager>().StopMusic("BassyMain");
        Camera.main.GetComponent<AudioManager>().StopMusic("BassyEvent");
    }

}
