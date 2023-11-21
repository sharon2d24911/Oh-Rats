using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropExamineMore : MonoBehaviour
{
    public BoxCollider2D boxCollider;
    [Header("Wall")]
    public Vector3 smallPos;
    public Vector3 smallScale;
    public Sprite smallSprite;
    public Vector2 smallBC;
    [Header("First Click")]
    public Sprite bigSprite;
    public Vector3 smallHiddenPos;
    public Vector3 smallHiddenScale;
    public Sprite smallHiddenSprite;
    public Vector2 smallHiddenBC;
    [Header("Second Click")]
    public Vector3 bigHiddenPos;
    public Vector3 bigHiddenScale;
    public Sprite bigHiddenSprite;
    public Vector2 bigHiddenBC;
    [Header("General")]
    public GameObject[] captions;
    public GameObject topPropPrefab;
    
    private GameObject background;

    private Vector3 targetPos;
    private Vector3 targetScale;
    private Sprite targetImage;
    private Vector3 targetBCSize;

    private GameObject topProp = null;
    private GameObject props;
    private int i;

    private readonly float fadeDuration = 0.3f;
    //private readonly float speed = 2f;

    private void Start()
    {
        i = 0;
        transform.localPosition = smallPos;
        transform.localScale = smallScale;
        boxCollider.size = smallBC;
        targetPos = smallPos;
        targetScale = transform.localScale;
        targetBCSize = smallBC;
        GetComponent<SpriteRenderer>().sprite = smallSprite;
        targetImage = smallSprite;
        props = FindObjectOfType<Prop>().gameObject;
        background = props.GetComponent<Prop>().background;
        if (captions.Length == 2)
        {
            captions[0].SetActive(false);
            captions[1].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.timeScale != 0f)
        {
            // Toggle the target scale on mouse click
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            // Toggle switch
            if (i == 0 && hit.collider != null && hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite.name == smallSprite.name)
            {
                i += 1; // Expand on first click
            } else if (i == 1)
            {
                // Check if player clicked on big prop or little prop
                Debug.Log(hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite.name);
                if (hit.collider != null && hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite.name == smallHiddenSprite.name)
                    i = 2; // Expand smaller prop
                else if (hit.collider != null && topProp != null && hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite.name == topProp.GetComponent<SpriteRenderer>().sprite.name)
                    i = 0; // Retract both props
            } else if (i == 2 && hit.collider != null && hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite.name == bigHiddenSprite.name)
            {
                i = 0;
            }
                ToggleSize();
        }

        if (GetComponent<SpriteRenderer>().sprite != targetImage || transform.position != targetPos || transform.localScale != targetScale)
        {
            Debug.Log("i IS CURRENTLY: " + i);
            // Transform to the new position/rotation/scale based on i
            transform.position = targetPos; // Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            transform.localScale = targetScale; // Vector3.Lerp(transform.localScale, targetScale, speed * Time.deltaTime);
            GetComponent<SpriteRenderer>().sprite = targetImage;
            boxCollider.size = targetBCSize;

            // If on first click (props expand to show a smaller prop to click)
            if (i == 1)
            {
                GetComponent<SpriteRenderer>().sortingOrder = 50;
                background.GetComponent<SpriteRenderer>().sortingOrder = 49;
                background.SetActive(true);

                topProp = Instantiate(topPropPrefab);
                topProp.GetComponent<SpriteRenderer>().sortingOrder = 51;

                if (captions.Length > 0)
                {
                    captions[0].transform.position = new Vector3(topProp.transform.position.x, (topProp.transform.position.y - 8f), 1f);
                    captions[0].SetActive(true);
                }
            // If on second click (smaller prop expands)
            } else if (i == 2)
            {
                StartCoroutine(FadeTopOut());

                if (captions.Length > 0)
                {
                    captions[1].transform.position = new Vector3(transform.position.x, (transform.position.y - 7f), 1f);
                    captions[0].SetActive(false);
                    captions[1].SetActive(true);
                }
            }
            else if (i == 0)
            {
                if (topProp != null)
                {
                    Destroy(topProp);
                    topProp = null;
                }

                if (captions.Length > 0)
                {
                    captions[0].SetActive(false);
                    captions[1].SetActive(false);
                }
                GetComponent<SpriteRenderer>().sortingOrder = 0;
                background.GetComponent<SpriteRenderer>().sortingOrder = 0;
                background.SetActive(false);
            }
        }
    }

    private void ToggleSize()
    {
        Vector3 position;
        Vector3 scale;
        Sprite image;
        Vector2 BC;
        if (i == 1)
        {
            position = smallHiddenPos;
            scale = smallHiddenScale;
            image = smallHiddenSprite;
            BC = smallHiddenBC;
        } else if (i == 2)
        {
            position = bigHiddenPos;
            scale = bigHiddenScale;
            image = bigHiddenSprite;
            BC = bigHiddenBC;
        } else
        {
            position = smallPos;
            scale = smallScale;
            image = smallSprite;
            BC = smallBC;
        }
        // Set the target for next click
        targetPos = position;
        targetScale = scale;
        targetImage = image;
        targetBCSize = BC;
    }

    public IEnumerator FadeTopOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            topProp.transform.position = Vector3.Lerp(topProp.transform.position, new Vector3(0, -18, 3), elapsedTime / fadeDuration);
            yield return null;
        }
        Destroy(topProp);
        topProp = null;
    }
}
