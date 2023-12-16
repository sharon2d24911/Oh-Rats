using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropExamineRat : MonoBehaviour
{
    public BoxCollider2D shelfBoxCollider;
    
    private Texture2D interactCursor;
    private Texture2D defaultCursor;
    
    private GameObject props;
    private GameObject gameHandler;
    private GameObject currentProp;

    private readonly float fadeDuration = 1f;

    private void Start()
    {
        props = FindObjectOfType<Prop>().gameObject;
        interactCursor = props.GetComponent<Prop>().interactCursor;
        defaultCursor = props.GetComponent<Prop>().defaultCursor;
        gameHandler = FindObjectOfType<GameHandler>().gameObject;
        shelfBoxCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        currentProp = props.GetComponent<Prop>().inspecting;
        if (Input.GetMouseButtonDown(0) && Time.timeScale != 0f && currentProp == null)
        {
            // Toggle the target scale on mouse click
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                StartCoroutine(FadeTop());
            }
        }

    }

    void OnMouseEnter()
    {
        if (Time.timeScale != 0f && !gameHandler.GetComponent<DragCombination>().trashMode && currentProp == null)
            Cursor.SetCursor(interactCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    void OnMouseExit()
    {
        if (!gameHandler.GetComponent<DragCombination>().trashMode)
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }
    
    public IEnumerator FadeTop()
    {
        float elapsedTime = 0f;
        Color fullAlpha = new Color(1, 1, 1, 1f);
        Color transparent = new Color(1, 1, 1, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            GetComponent<SpriteRenderer>().color = Color.Lerp(fullAlpha, transparent, elapsedTime / fadeDuration);
            yield return null;
        }

        shelfBoxCollider.enabled = true;
        gameObject.SetActive(false);
    }
}
