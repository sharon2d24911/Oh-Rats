using UnityEngine;
using UnityEngine.UI;

public class PropExamine : MonoBehaviour
{
    public BoxCollider2D boxCollider;
    [Header("Display size")]
    public Sprite small;
    public Sprite big;
    public Vector3 smallPos;
    public Vector3 smallRot;
    public Vector3 smallScale;
    public Vector2 smallBC;
    [Header("Expanded size")]
    public Vector3 bigPos;
    public Vector3 bigRot;
    public Vector3 bigScale;
    public Vector2 bigBC;
    [Header("Cursor")]
    public Texture2D interactCursor;
    public Texture2D defaultCursor;
    //private float speed = 6f;

    private Vector3 targetPos;
    private Vector3 targetRot;
    private Vector3 targetScale;
    private Sprite targetImage;
    private Vector2 targetBCSize;
    private bool isBig;

    private GameObject gameHandler;

    private void Start()
    {
        transform.localPosition = smallPos;
        transform.eulerAngles = smallRot;
        transform.localScale = smallScale;
        targetScale = transform.localScale;
        GetComponent<SpriteRenderer>().sprite = small;
        isBig = false;
        gameHandler = FindObjectOfType<GameHandler>().gameObject;
    }

    private void Update()
    {
        
    }

    private void OnMouseDown()
    {
        // Toggle the target scale on mouse click
        if (Time.timeScale != 0f)
            ToggleSize();

        if (transform.localScale != targetScale)
        {
            // Transform to the new position/scale over a certain time, set rotation (Lerp causes it to spin weirdly)
            transform.position = targetPos; //Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            transform.eulerAngles = targetRot;
            transform.localScale = targetScale; //Vector3.Lerp(transform.localScale, targetScale, speed * Time.deltaTime);
            GetComponent<SpriteRenderer>().sprite = targetImage;
            boxCollider.size = targetBCSize;
        }
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

    private void ToggleSize()
    {
        // Toggle the "isBig" bool
        isBig = !isBig;
        // If isBig, first condition (big pos/rot/scale), if !isBig, second condition (small pos/rot/scale)
        Vector3 position = isBig ? bigPos : smallPos;
        Vector3 rotation = isBig ? bigRot : smallRot;
        Vector3 scale = isBig ? bigScale : smallScale;
        Sprite image = isBig ? big : small;
        Vector2 BC = isBig ? bigBC : smallBC;
        // Set the target for next click
        targetPos = position;
        targetRot = rotation;
        targetScale = scale;
        targetImage = image;
        targetBCSize = BC;
        Sound(); // play sfx
    }
    
    private void Sound()
    {
        if (isBig && (this.name == "S1 - Framed photo" || this.name == "S1 - Award" || this.name == "S1 - Framed bakery" || this.name == "S1 - Framed photo"))
        {
            AudioManager.Instance.PlaySFX("FrameOn");
        }
        else if (!isBig && (this.name == "S1 - Framed photo" || this.name == "S1 - Award" || this.name == "S1 - Framed bakery" || this.name == "S1 - Framed photo"))
        {
            AudioManager.Instance.PlaySFX("FrameOff");
        }
        else if (isBig && (this.name == "S1 - Folded newspaper" || this.name == "S1 - Work Schedule"))
        {
            AudioManager.Instance.PlaySFX("PaperOn");
        }
        else if (!isBig && (this.name == "S1 - Folded newspaper" || this.name == "S1 - Work Schedule"))
        {
            AudioManager.Instance.PlaySFX("PaperOff");
        }
    }
}
