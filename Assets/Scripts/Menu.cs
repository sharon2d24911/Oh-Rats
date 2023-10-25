using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Animator animator;
    private string sceneToLoad;
    
    // Using animator to fade scene out to black
    public void FadeToScene(string sceneName)
    {
        
        sceneToLoad = sceneName;
        animator.SetTrigger("FadeOut");
        Camera.main.GetComponent<AudioManager>().PlaySFX("StartButton");
    }

    // Animation event on the completion of fading out to call scene
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    
}
