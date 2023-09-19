using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropBehaviourScript : MonoBehaviour
{
    private GameObject selectedObject;
    private Vector3 startingPosition;
    private List<GameObject> combining = new List<GameObject>();
    private List<GameObject> dragged = new List<GameObject>();
    public GameObject combinationZone;
    private readonly float sensitivity = 2.0f;
    private bool isIngredient;
    private GameObject grid;
    private GameObject tower;

    void Start()
    {
        grid = GameObject.Find("PlaceholderGrid");
    }
    // Update is called once per frame
    void Update()
    {

        // If left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            CheckHitObject();
        }

        // If left mouse button is held down
        if (Input.GetMouseButton(0) && selectedObject != null)
        {
            DragObject();
        }

        // If left mouse button is released
        if (Input.GetMouseButtonUp(0) && selectedObject != null)
        {
            DropObject();
        }

    }

    // Checks if there is an object that can be selected at the mouse position
    void CheckHitObject()
    {
        // Gets mouse coordinates and maps to where it is on the game screen
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Raycasting: did the mouse hit a collider?
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (hit.collider != null && !dragged.Contains(hit.collider.gameObject))
        {
            selectedObject = hit.collider.gameObject;
            startingPosition = selectedObject.transform.position;
            Debug.Log(selectedObject.tag);
            if (selectedObject.tag == "Ingredient")
            {
                isIngredient = true;
                GameObject clone = Instantiate(selectedObject);
            }
            else
            {
                isIngredient = false;
            }
        }
    }

    // Drags selected object if something is at mouse position
    void DragObject()
    {
        // Set transparency of object to 70%
        selectedObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .7f);

        // Change the position of the selected object based on the position of the mouse (accounting for offset)
        selectedObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 10.0f));
    }

    // Drops object that was being dragged by mouse
    void DropObject()
    {
        // Resets object transparency
        selectedObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        GridCreate gridScript = grid.GetComponent<GridCreate>();
        Vector3 nearestPos = selectedObject.transform.position;
        float nearestDistance = Vector3.Distance(grid.transform.position, selectedObject.transform.position);
        List<Vector3> gridPositions;
        gridPositions = gridScript.getPositions(); //grabs list of grid positions from the GridCreate script


        Debug.Log(nearestDistance);

        foreach (Vector3 p in gridPositions)
        {
            float newDistance = Vector3.Distance(p, selectedObject.transform.position);
            if (newDistance < nearestDistance)
            {
                nearestDistance = newDistance;
                nearestPos = p;
            }
        }

        Debug.Log(nearestDistance);
        Debug.Log(nearestPos);

        if (isIngredient)
        {
            // Checks if the selected object is close enough or on the object that's been designated as the combination area
            // Will only accept new object placements if the list of things to be combined is not yet at 2 (assuming 2 objects will be the limit, subject to chenge)
            if (Vector3.Distance(combinationZone.transform.position, selectedObject.transform.position) < sensitivity && combining.Count < 2)
            {
                // Snaps object into the same position
                selectedObject.transform.position = combinationZone.transform.position;
                dragged.Add(selectedObject);

                // Adds selected object to list
                combining.Add(selectedObject);

                if (combining.Count == 2)
                {
                    // Combines two objects
                    CombineObjects();
                }
            }
            else
            {
                // Destroy the object selected if not placed close enough
                Debug.Log("DESTROY");
                Destroy(selectedObject);
            }
        }
        else if (nearestDistance > sensitivity)
        {
            // If tower is not within distance, place back in original spot and destroy current instance
            GameObject clone = Instantiate(selectedObject);
            clone.transform.position = startingPosition;
            Destroy(selectedObject);
        }
        else
        {
            // If tower is within distance of a grid spot, snaps object into the same position
            selectedObject.transform.position = new Vector3(nearestPos.x, nearestPos.y, nearestPos.z - 1f);
            dragged.Add(selectedObject);
        }

        selectedObject = null;
    }

    void CombineObjects()
    {
        // Checks if the two objects in combining can be combined, and if so, instantiates the combined object.
        Debug.Log(combining[0] + ", " + combining[1]);
        // Should be something that would store all combinations later on rather than if statements
        if ((combining[0].name.Contains("butter") && combining[1].name.Contains("eggs")) || (combining[0].name.Contains("eggs") && combining[1].name.Contains("butter")))
        {
            tower = Instantiate(Resources.Load("bagel_object"), combinationZone.transform.position, Quaternion.identity) as GameObject; // Create the combination of the two objects
            // This sets the new combined tower to be the same size as the last selected object because right now it's too small
            // Will likely not be as necessary once we get real assets
            tower.transform.localScale = new Vector3(selectedObject.transform.localScale.x, selectedObject.transform.localScale.y, selectedObject.transform.localScale.z);
            tower.gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("COMBINED");
        }

        // Clear the list after combining or after figuring out there is no combination
        Destroy(combining[0]);
        Destroy(combining[1]);
        combining.Clear();
    }
}