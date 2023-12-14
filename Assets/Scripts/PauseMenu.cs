using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public bool isAlmanac = false;
    public GameObject notification;
    private bool prop = false;
    private GameObject props;

    private void Start()
    {
        props = FindObjectOfType<Prop>().gameObject;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        props.GetComponent<Prop>().paused = true;
        if (Time.timeScale == 0f)
            prop = true;
        else
            prop = false;
        Time.timeScale = 0f;
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().PlaySFX("UIClick",GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][1]);
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
        props.GetComponent<Prop>().paused = false;
        if (!prop)
            Time.timeScale = 1f;
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().PlaySFX("UIClick", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UIClick"][1]);
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().MusicVolume(1f);
    }

}
