using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Animator animator;
    private string sceneToLoad;

    void Start()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
    }

    // Using animator to fade scene out to black
    public void FadeToScene(string sceneName)
    {
        
        sceneToLoad = sceneName;
        animator.SetTrigger("FadeOut");
        Camera.main.GetComponent<AudioManager>().PlaySFX("StartButton");
        StartCoroutine(Camera.main.GetComponent<AudioManager>().Fade(true, "BassyMain", 1f, 1f)); // Fade in music
    }

    // Animation event on the completion of fading out to call scene
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    
}
