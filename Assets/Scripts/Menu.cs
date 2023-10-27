using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Animator animator;
    private string sceneToLoad;
    public Texture2D basic;

    void Start()
    {
        Cursor.SetCursor(basic, Vector2.zero, CursorMode.ForceSoftware);
    }

    // Using animator to fade scene out to black
    public void FadeToScene(string sceneName)
    {
        Time.timeScale = 1f;
        sceneToLoad = sceneName;
        animator.SetTrigger("FadeOut");
    }

    public void LoadMusic()
    {
        Camera.main.GetComponent<AudioManager>().PlaySFX("StartButton");
        StartCoroutine(Camera.main.GetComponent<AudioManager>().Fade(true, "BassyMain", 1f, 1f)); // Fade in music
    }

    // Animation event on the completion of fading out to call scene
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
