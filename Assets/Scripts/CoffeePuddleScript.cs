using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeePuddleScript : MonoBehaviour
{

    private float currentTime;
    public float puddleDuration = 10f;
    private GameObject GH;
    private Dictionary<Vector2, GameObject> gridPositions;


    // Start is called before the first frame update
    void Start()
    {
        GH = GameObject.Find("GameHandler");
        gridPositions = GH.GetComponent<DragCombination>().filledPositions;
        Debug.Log(gridPositions.ContainsKey(gameObject.transform.position));
        //adds itself to the dictionary of filled positions, stops units from being placed
        gridPositions.Add(gameObject.transform.position, gameObject);
        Debug.Log(gridPositions.ContainsKey(gameObject.transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        gameObject.GetComponent <SpriteRenderer>().color = gameObject.GetComponent <SpriteRenderer>().color - new Color(0,0,0,(currentTime > 0.75* puddleDuration) ? (Mathf.Lerp(0,1, currentTime/(puddleDuration * 500))) : 0);
        if (currentTime > puddleDuration) {
            //destroys the puddle and frees the spot
            Destroy(gameObject);
            gridPositions.Remove(gameObject.transform.position);
        }
    }
}
