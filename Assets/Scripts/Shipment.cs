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
    public Button deliveryButton;
    private bool shipping;
    private int currentWave;
    public bool tutorial = false;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        shipmentTimer = 0;
        targetPosition = target.transform.position;
        shipping = false;
        if (!tutorial)
        {
            currentWave = GameObject.FindWithTag("GameHandler").GetComponent<WaveSpawning>().currentWave;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shipping)
        {
            shipmentTimer += Time.deltaTime;

            //Debug.Log("Shipment arriving in: " + (shipmentTimerMax - shipmentTimer) + " seconds");
            // Moves delivery towards ingredients area over the number of seconds it takes for the new shipment to show up
            transform.position = Vector3.Lerp(startPosition, targetPosition, shipmentTimer / shipmentTimerMax);
            if (shipmentTimer < shipmentTimerMax)
            {
                progressBar.fillAmount = shipmentTimer / shipmentTimerMax;
            }
            if (shipmentTimer >= shipmentTimerMax && ingredients.Count > 0)
            {
                if (!tutorial)
                {
                    currentWave = GameObject.FindWithTag("GameHandler").GetComponent<WaveSpawning>().currentWave;
                }
                for (int i = 0; i < ingredients.Count; i++)
                {
                    // Adds one of each ingredient
                    AudioManager.Instance.PlaySFX("delivery", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["delivery"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["delivery"][1]);

                    //SpudNut build
                    if (!tutorial)
                    {
                        Debug.Log("from shipment: " + currentWave);
                        ingredients[i].GetComponent<Ingredient>().AddIngredient((int)Random.Range(1, (currentWave > 2 ? Mathf.Clamp(currentWave, 1f, 3f) : 1)));
                    }
                    else
                    {
                        ingredients[i].GetComponent<Ingredient>().AddIngredient(3);
                    }


                }
                shipmentTimer = 0;
                transform.position = startPosition;
                progressBar.fillAmount = 0;
                shipping = false;
                if(!tutorial)
                    deliveryButton.interactable = true;
            }
        }
    }

    // When Ingredient delivery button is pressed
    public void ShipIngredients()
    {
        shipping = true;
        deliveryButton.interactable = false;
    }
}
