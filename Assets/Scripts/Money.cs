using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Money : MonoBehaviour
{
    public TextMeshProUGUI moneyCounter;
    private float moneyAmount;

    // Start is called before the first frame update
    void Start()
    {
        moneyAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        moneyCounter.SetText("$" + moneyAmount);
    }

    public void AddMoney (float money)
    {
        moneyAmount += money;
    }
}
