using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragCombination : MonoBehaviour
{
    private GameObject selectedObject;
    private GameObject baseObject;
    private Vector2 startingPosition;
    private List<GameObject> combining = new List<GameObject>();
    private List<GameObject> dragged = new List<GameObject>();
    public Dictionary<Vector2, GameObject> filledPositions = new Dictionary<Vector2, GameObject>();
    public GameObject combinationZone;
    private readonly float sensitivity = 2.0f;
    private bool isIngredient;
    private GameObject grid;
    public GameObject unit;
    public Button mixButton;
    private GameObject newUnit;
    private string sugarDropSound;
    private string flourDropSound;
    private string eggDropSound;
    private string sugarGrabSound;
    private string flourGrabSound;
    private string eggGrabSound;
    private GameObject[] allIngredients;

    void Start()
    {
        grid = GameObject.Find("Grid");
        allIngredients = GameObject.FindGameObjectsWithTag("Ingredient");
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the mix button should allow interaction
        if (!CheckMinimum())
            mixButton.interactable = false;
        else
            mixButton.interactable = true;

        // When left mouse is pressed
        if (Input.GetMouseButtonDown(0) && Time.timeScale != 0f)
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
                
                // Sfx for ingredient grab
                if (selectedObject.name == "Sugar") // sugar sfx
                {
                    string[] sugarGrabSound = { "SugarGrab1", "SugarGrab2" };
                    AudioManager.Instance.PlaySFX(this.sugarGrabSound = sugarGrabSound[Mathf.FloorToInt(Random.Range(0, 2))]);
                }
                else if (selectedObject.name == "Flour") // flour sfx
                {
                    string[] flourGrabSound = { "FlourGrab1", "FlourGrab2" };
                    AudioManager.Instance.PlaySFX(this.flourGrabSound = flourGrabSound[Mathf.FloorToInt(Random.Range(0, 2))]);
                }
                else if (selectedObject.name == "Egg") // egg sfx
                {
                    string[] eggGrabSound = { "EggGrab1", "EggGrab2", "EggGrab3" };
                    AudioManager.Instance.PlaySFX(this.eggGrabSound = eggGrabSound[Mathf.FloorToInt(Random.Range(0, 3))]);
                }

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

        Vector2 selectedV2 = selectedObject.transform.position;

        GridCreate gridScript = grid.GetComponent<GridCreate>();
        Vector2 nearestPos = startingPosition;
        float nearestDistance = Vector2.Distance(grid.transform.position, selectedV2);
        int gridDepth = 0, i = 0;
        List<Vector3> gridPositions;
        gridPositions = gridScript.getPositions(); //grabs list of grid positions from the GridCreate script

        Debug.Log(nearestDistance);

        foreach (Vector2 p in gridPositions)
        {
            i++;
            float newDistance = Vector2.Distance(p, selectedV2);
            if (newDistance < nearestDistance)
            {
                nearestDistance = newDistance;

                if (!filledPositions.ContainsKey(p))  //position isn't occupied in the dictionary, and so is free on the grid
                {
                    Debug.Log("spot empty");
                    Debug.Log("i " + i);
                    gridDepth = (i / gridScript.columns);
                    Debug.Log("gridDepth " + gridDepth);
                    nearestPos = p;
                }
                else //position is occupied in the dictionary, not free on the grid
                {
                    Debug.Log("spot filled");
                }
            }
        }

        if (isIngredient)
        {
            Vector2 combV2 = combinationZone.transform.position;

            // Checks if the selected object is close enough or on the object that's been designated as the combination area
            if (Vector2.Distance(selectedV2, combV2) < sensitivity)
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

                // Sfx for ingredient drop
                if (selectedObject.name == "Sugar_individual(Clone)") // sugar sfx
                {
                    string[] sugarDropSound = { "SugarDrop1", "SugarDrop2"};
                    AudioManager.Instance.PlaySFX(this.sugarDropSound = sugarDropSound[Mathf.FloorToInt(Random.Range(0, 2))]);
                }
                else if(selectedObject.name == "Flour_individual(Clone)") // flour sfx
                {
                    string[] flourDropSound = { "FlourDrop1", "FlourDrop2" };
                    AudioManager.Instance.PlaySFX(this.flourDropSound = flourDropSound[Mathf.FloorToInt(Random.Range(0, 2))]);
                }
                else if (selectedObject.name == "Egg_individual(Clone)") // egg sfx
                {
                    string[] eggDropSound = { "EggDrop1", "EggDrop2", "EggDrop3", "EggDrop4" };
                    AudioManager.Instance.PlaySFX(this.eggDropSound = eggDropSound[Mathf.FloorToInt(Random.Range(0, 4))]);
                }

                // Adds selected object to list
                combining.Add(selectedObject);

                // Once placed, decrease count for that ingredient by 1
                baseObject.GetComponent<Ingredient>().UseIngredient();

            }
            else // Destroy the ingredient instance selected if not placed close enough
                Destroy(selectedObject);
        }
        else if (nearestDistance > sensitivity || nearestPos == selectedV2)
        {
            // If Unit is not within distance, place back in original spot and destroy current instance
            // GameObject clone = Instantiate(selectedObject);
            // clone.transform.position = startingPosition;
            // Destroy(selectedObject);
            selectedObject.transform.position = new Vector3(startingPosition.x, startingPosition.y, 1);
        }
        else
        {
            // If tower is within distance of a grid spot, snaps object into the same position
            selectedObject.transform.position = new Vector3(nearestPos.x, nearestPos.y, gridDepth + 1f);
            selectedObject.GetComponent<UnitBehaviour>().placed = true;

            filledPositions.Add(nearestPos, selectedObject); //puts unit in dictionary, position will no longer be free on the grid
            dragged.Add(selectedObject);
            AudioManager.Instance.PlaySFX("DonutPlace");
        }

        selectedObject = null;
    }

    public void CheckIfCombine()
    {
        // If player hits mix button, use up all the placed ingredients by tallying up their stats
        float attack = 0;
        float speed = 0;
        float health = 0;

        // Clear ingredients used counters
        foreach (GameObject ingredient in allIngredients)
        {
            ingredient.GetComponent<Ingredient>().ClearUse();
        }

        // Pull each item out and add the stats up, then instantiate a unit with those stats & correct layering
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
        StartCoroutine(CombineWithDelay(attack, speed, health));

    }

    // Checks if the user has placed at minimum one of each ingredient
    bool CheckMinimum()
    {
        // Minimum of 3 ingredients, early check
        if (combining.Count < 3)
            return false;

        // Checks if each ingredient has at least one child (and is not the currently selected one, as that hasn't been placed in the bowl yet)
        foreach (GameObject ingredient in allIngredients)
        {
            if (ingredient.transform.childCount < 1 || (ingredient.transform.childCount == 1 && selectedObject != null && selectedObject.transform.parent == ingredient))
                return false;
        }
        return true;
        
    }

    // Creates new unit based on the given stats
    IEnumerator CombineWithDelay(float addAttack, float addSpeed, float addHealth)
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(2);

        newUnit = Instantiate(unit, combinationZone.transform.position, Quaternion.identity);
        //Layer = Instantiate(layer1, combinationZone.transform.position, Quaternion.identity);

        // Change the unit's attack, speed, and health based on additional ingredient stats
        newUnit.GetComponent<UnitBehaviour>().projAddAttack += addAttack;
        newUnit.GetComponent<UnitBehaviour>().projAddSpeed += addSpeed;
        newUnit.GetComponent<UnitBehaviour>().health += addHealth;

        //Note from Matthieu: for right now, this is simply hard coded cause I couldnt find a simpler way to grab the "base" values for each stat. If you have a fix, please implement it. Thanks
        newUnit.GetComponent<UnitBehaviour>().attackBoost = (int)(addAttack / 5) - 1;
        newUnit.GetComponent<UnitBehaviour>().speedBoost = (int)(addSpeed / 0.2) - 1;
        newUnit.GetComponent<UnitBehaviour>().healthBoost = (int)(addHealth / 25) - 1;
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