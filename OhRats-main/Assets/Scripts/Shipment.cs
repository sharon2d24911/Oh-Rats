using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shipment : MonoBehaviour
{
    private float shipmentTimer;
    public float shipmentTimerMax;
    public List<Ingredient> ingredients;
    public GameObject target;
    private Vector3 targetPosition;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        shipmentTimer = 0;
        targetPosition = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        shipmentTimer += Time.deltaTime;

        //Debug.Log("Shipment arriving in: " + (shipmentTimerMax - shipmentTimer) + " seconds");

        // Moves box towards ingredients area over the number of seconds it takes for the new shipment to show up
        transform.position = Vector3.Lerp(startPosition, targetPosition, shipmentTimer / shipmentTimerMax);
        if (shipmentTimer >= shipmentTimerMax && ingredients.Count > 0)
        {
            for (int i = 0; i < ingredients.Count; i++)
            {
                // Adds one of each ingredient
                ingredients[i].GetComponent<Ingredient>().AddIngredient();
                Debug.Log(ingredients[i].name + " remaining: " + ingredients[i].remaining);
            }
            shipmentTimer = 0;
            transform.position = startPosition;
        }

    }


}
