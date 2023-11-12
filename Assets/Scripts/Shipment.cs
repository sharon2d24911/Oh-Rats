using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shipment : MonoBehaviour
{
    private float shipmentTimer;
    public float shipmentTimerMax;
    public List<Ingredient> ingredients;
    public GameObject target;
    private Vector3 targetPosition;
    private Vector3 startPosition;
    public Image progressBar;

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
        if (shipmentTimer < shipmentTimerMax)
        {
            progressBar.fillAmount = shipmentTimer / shipmentTimerMax;
        }
        if (shipmentTimer >= shipmentTimerMax && ingredients.Count > 0)
        {
            for (int i = 0; i < ingredients.Count; i++)
            {
                // Adds one of each ingredient
                AudioManager.Instance.PlaySFX("delivery", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["delivery"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["delivery"][1]);
                ingredients[i].GetComponent<Ingredient>().AddIngredient();
                Debug.Log(ingredients[i].name + " remaining: " + ingredients[i].remaining);
            }
            shipmentTimer = 0;
            transform.position = startPosition;
            progressBar.fillAmount = 0;
        }

    }


}
