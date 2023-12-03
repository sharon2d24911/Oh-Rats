using UnityEngine;
using TMPro;

public class Ingredient : MonoBehaviour
{
    [HideInInspector] public int remaining = 3;
    public float attack;
    public float speed;
    public float health;
    public GameObject singularIngredient;
    public GameObject remainingCounter;
    public GameObject usedCounter;
    private int used = 0;

    // Start is called before the first frame update
    void Start()
    {
        remainingCounter.GetComponent<TextMeshProUGUI>().text = remaining.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Used for shipment, adds one to the ingredient and updates remaining counter
    public void AddIngredient(int amount)
    {
        remaining += amount;
        remainingCounter.GetComponent<TextMeshProUGUI>().text = remaining.ToString();
        if (remaining > 0)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        }
    }

    // Used in combining, removes one of the ingredient and updates both counters
    public void UseIngredient()
    {
        remaining -= 1;
        remainingCounter.GetComponent<TextMeshProUGUI>().text = remaining.ToString();

        used += 1;
        if (used == 3)
        {
            usedCounter.GetComponent<TextMeshProUGUI>().text = "<color=#c44f4f>" + used.ToString() + "</color>";
        } else
            usedCounter.GetComponent<TextMeshProUGUI>().text = used.ToString();

        if (remaining < 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        }
    }

    // Used in combining, clears used counter when combining all the ingredients in the bowl
    public void ClearUse()
    {
        used = 0;
        usedCounter.GetComponent<TextMeshProUGUI>().text = used.ToString();
    }
}
