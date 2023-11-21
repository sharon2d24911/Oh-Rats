using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursors : MonoBehaviour
{
    public Texture2D interactCursor;
    public Texture2D defaultCursor;
    private GameObject gameHandler;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
        gameHandler = FindObjectOfType<GameHandler>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseEnter()
    {
        if (Time.timeScale != 0f && !gameHandler.GetComponent<DragCombination>().trashMode)
            Cursor.SetCursor(interactCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    void OnMouseExit()
    {
        if (!gameHandler.GetComponent<DragCombination>().trashMode)
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }
}
