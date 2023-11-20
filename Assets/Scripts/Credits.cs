using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{

    public GameObject creditsPopUp;
    private readonly float fadeDuration = .3f;
    public Texture2D defaultCursor;

    // Start is called before the first frame update
    void Start()
    {
        creditsPopUp.SetActive(false);
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openCredits ()
    {
        StartCoroutine(FadeIn());
    }

    public void closeCredits()
    {
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn()
    {
        creditsPopUp.SetActive(true);
        creditsPopUp.GetComponent<CanvasGroup>().alpha = 0;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            creditsPopUp.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            creditsPopUp.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0, elapsedTime / fadeDuration);
            yield return null;
        }
        creditsPopUp.SetActive(false);
    }
}
