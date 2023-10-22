using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ingredient : MonoBehaviour
{
    public int remaining;
    public float attack;
    public float speed;
    public float health;
    public GameObject singularIngredient;
    public GameObject counter;

    // Start is called before the first frame update
    void Start()
    {
        counter.GetComponent<TMPro.TextMeshProUGUI>().text = remaining.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddIngredient()
    {
        remaining += 1;
        counter.GetComponent<TMPro.TextMeshProUGUI>().text = remaining.ToString();
        if (remaining == 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        }
    }

    public void UseIngredient()
    {
        remaining -= 1;
        counter.GetComponent<TMPro.TextMeshProUGUI>().text = remaining.ToString();
        Debug.Log("Used 1 " + this);
        if (remaining < 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        }
    }
}
