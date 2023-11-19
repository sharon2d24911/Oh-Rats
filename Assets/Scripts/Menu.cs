using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Animator animator;
    private string sceneToLoad;
    //public Texture2D basic;

    void Start()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        //Time.timeScale = 1f;
    }

    // Using animator to fade scene out to black
    public void FadeToScene(string sceneName)
    {
        Time.timeScale = 1f;
        sceneToLoad = sceneName;
        animator.SetTrigger("FadeOut");
        if(SceneManager.GetActiveScene().name == "TitleScene")
        {
            GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().PlaySFX("StartButton", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["StartButton"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["StartButton"][1]);
            StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeOut("TitleWop", "none", "none", "none", 1, 0));
        }
        //else if (SceneManager.GetActiveScene().name == "Game")
        //{
        //    StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeIn("TitleWop", "none", "none", "none", 1, 1, 1));
        //}
        else
        {
            StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeOut("BassyMain", "none", "none", "none", 1, 0)); // Fade out music
        }
        
    }

    // Animation event on the completion of fading out to call scene
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
