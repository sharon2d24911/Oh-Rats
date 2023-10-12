using System.Collections.Generic;
using UnityEngine;

public class DragCombination : MonoBehaviour
{
    private GameObject selectedObject;
    private GameObject baseObject;
    private Vector3 startingPosition;
    private List<GameObject> combining = new List<GameObject>();
    private List<GameObject> dragged = new List<GameObject>();
    public GameObject combinationZone;
    private readonly float sensitivity = 2.0f;
    private bool isIngredient;
    private GameObject grid;
    public GameObject unit;
    private GameObject newUnit;

    void Start()
    {
        grid = GameObject.Find("PlaceholderGrid");
    }

    // Update is called once per frame
    void Update()
    {
        // When left mouse is pressed
        if (Input.GetMouseButtonDown(0))
            CheckHitObject();

        // If left mouse button is held down
        if (Input.GetMouseButton(0) && selectedObject != null)
            DragObject();

        // If left mouse button is released
        if (Input.GetMouseButtonUp(0) && selectedObject != null)
            DropObject();

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

            // Separates behaviour depending on selected object type
            if (selectedObject.tag == "Ingredient" && selectedObject.GetComponent<Ingredient>().remaining > 0)
            {
                isIngredient = true;
                GameObject ingredient = Instantiate(selectedObject.GetComponent<Ingredient>().singularIngredient);
                baseObject = selectedObject;
                selectedObject = ingredient;
                // Set parent to be the ingredient
                selectedObject.transform.SetParent(baseObject.transform, true);
            }
            else if (selectedObject.tag == "Unit")
                isIngredient = false;
            else // Nothing else can be dragged, but add condition for trash if needed
                selectedObject = null;
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


        foreach (Vector3 p in gridPositions)
        {
            float newDistance = Vector3.Distance(p, selectedObject.transform.position);
            if (newDistance < nearestDistance)
            {
                nearestDistance = newDistance;
                nearestPos = p;
            }
        }
        

        if (isIngredient)
        {
            float xDifference = Mathf.Abs(combinationZone.transform.position.x - selectedObject.transform.position.x);
            float yDifference = Mathf.Abs(combinationZone.transform.position.y - selectedObject.transform.position.y);

            // Checks if the selected object is close enough or on the object that's been designated as the combination area
            if (xDifference < sensitivity && yDifference < sensitivity)
            {
                // Check if there are >3 of the ingredient in the combining list
                if (selectedObject.transform.parent.transform.childCount > 3)
                {
                    Debug.Log("Can't have more than three of one ingredient!");
                    Destroy(selectedObject);
                    return;
                }

                // Snaps object into the same position
                selectedObject.transform.position = combinationZone.transform.position;
                dragged.Add(selectedObject);

                // Adds selected object to list
                combining.Add(selectedObject);

                // Once placed, decrease count for that ingredient by 1
                baseObject.GetComponent<Ingredient>().UseIngredient();
                
            }
            else // Destroy the ingredient instance selected if not placed close enough
                Destroy(selectedObject);
        }
        else if (nearestDistance > sensitivity)
        {
            // If a tower (not ingredient!) is not within distance, place back in original spot and destroy current instance
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

    public void CheckIfCombine()
    {
        Debug.Log("Checking if mixable!");

        // If nothing in Combine list, return that the bowl is empty!
        if (combining.Count == 0)
        {
            Debug.Log("There's nothing to combine!");
        }
        // Else if there isn't at least one of each ingredient, return that the base tower requires at least one of each
        else if (!CheckMinimum())
        {
            Debug.Log("You haven't added the minimum of one of each ingredient to create a unit!");
        }
        // Else while the list isn't empty, pull each item out and add the stats up, then instantiate a unit with those stats & correct layering
        else
        {
            // If player hits mix button, use up all the placed ingredients by tallying up their stats
            float attack = 0;
            float speed = 0;
            float health = 0;
            while (combining.Count > 0)
            {
                attack += combining[0].GetComponentInParent<Ingredient>().attack;
                speed += combining[0].GetComponentInParent<Ingredient>().speed;
                health += combining[0].GetComponentInParent<Ingredient>().health;
                Destroy(combining[0]);
                combining.Remove(combining[0]);
            }
            Debug.Log("BOOSTING:\nAttack: " + attack + ", Speed: " + speed + ", Health: " + health);

            // Create tower with those stats
            CreateNewTower(attack, speed, health);
        }

    }

    // Checks if the user has placed at minimum one of each ingredient
    bool CheckMinimum()
    {
        GameObject[] allIngredients = GameObject.FindGameObjectsWithTag("Ingredient");
        foreach (GameObject ingredient in allIngredients)
        {
            if (ingredient.transform.childCount < 1)
                return false;
        }
        return true;
    }

    // Creates new unit based on the given stats
    void CreateNewTower(float addAttack, float addSpeed, float addHealth)
    {
        newUnit = Instantiate(unit, combinationZone.transform.position, Quaternion.identity);

        // Change the unit's attack, speed, and health based on additional ingredient stats
        newUnit.GetComponent<UnitBehaviour>().projAddAttack += addAttack;
        newUnit.GetComponent<UnitBehaviour>().projAddSpeed += addSpeed;
        newUnit.GetComponent<UnitBehaviour>().health += addHealth;
        newUnit.tag = "Unit";
    }

    // OPTION if player wants to clear combination before mixing, delete if we don't use
    void ClearBowl()
    {
        // Empty any objects on the combination zone
        if (combining.Count > 0)
        {
            foreach (GameObject item in combining)
            {
                item.GetComponentInParent<Ingredient>().AddIngredient();
                Destroy(item);
            }
        }
        // Clear anything that was already added to the list to be combined
        combining.Clear();
        dragged.Clear();
    }
}