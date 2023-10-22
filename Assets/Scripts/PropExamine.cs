using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropExamine : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && CheckIfProp())
        {
            this.transform.position = new Vector3(0, 0, 0);
            this.transform.localScale = new Vector3(2.373809f, 2.373809f, 2.373809f);
        }
    }

    bool CheckIfProp()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Raycasting: did the mouse hit a collider?
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (hit.collider != null && hit.collider.gameObject.name == this.name)
            return true;
        return false;
    }
}
