using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropBehaviourScript : MonoBehaviour
{
    private GameObject selectedObject;
    private Vector3 startingPosition;
    private List<GameObject> combining = new List<GameObject>();
    public GameObject combinationZone;
    private float sensitivity = 1.0f;
    private GameObject grid;

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

        // Dragging the selected object
        if (Input.GetMouseButton(0) && selectedObject != null)
        {
            // Change the position of the selected object based on the position of the mouse (accounting for offset)
            selectedObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 10.0f));
        }
        
        // Letting go of mouse button releases selected object
        if (Input.GetMouseButtonUp(0) && selectedObject != null)
        {
            DropObject();
        }
        
    }

    void CheckHitObject()
    {
        // Gets mouse coordinates and maps to where it is on the game screen
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Raycasting: did the mouse hit a collider?
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (hit.collider != null)
        {
            selectedObject = hit.collider.gameObject;
            startingPosition = selectedObject.transform.position;
        }
    }

    void DropObject()
    {

        // Clones the selected object at its starting position once dropped
        /* Ideally this clone should be happening at the start of the dragging to
         * make more sense (player isn't always seemingly dragging out the last item),
         * but having the code there right now allows players to drag clones out from
         * the combination area after the original object has already been placed down.
         * Will need to be fixed later on but should still work for the prototype logic.
         */
        GameObject clone = Instantiate(selectedObject);
        GridCreate gridScript = grid.GetComponent<GridCreate>();
        Vector3 nearestPos = selectedObject.transform.position;
        float nearestDistance = Vector3.Distance(grid.transform.position, selectedObject.transform.position);
        List<Vector3> gridPositions;
        gridPositions = gridScript.getPositions(); //grabs list of grid positions from the GridCreate script
        clone.transform.position = startingPosition;

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

        // Checks if the selected object is close enough or on the object that's been designated as the combination area
        // Will only accept new object placements if the list of things to be combined is not yet at 2 (assuming 2 objects will be the limit, subject to chenge)
        if (Vector3.Distance(combinationZone.transform.position, selectedObject.transform.position) < sensitivity && combining.Count < 2)
        {
            // Snaps object into the same position
            selectedObject.transform.position = new Vector3(combinationZone.transform.position.x, combinationZone.transform.position.y, combinationZone.transform.position.z - 0.1f);

            // Adds selected object to list
            combining.Add(selectedObject);
        } else if (nearestDistance < sensitivity) {
            // Snaps object into the same position
            selectedObject.transform.position = nearestPos;
        }
        else
        {
            // Destroy the object selected if there are already 2 items in combination area or if not placed close enough

            /* Should be updated by combination logic so that both items
             * will be destroyed either way once there are two items here
             * regardless of if they are allowed to combine or not.
             * Might need a new function, probably also an elif for when
             * combining.Count == 2.
             */
            Destroy(selectedObject);
        }

        selectedObject = null;
    }
}
