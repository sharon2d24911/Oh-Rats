using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public float GameOverXPosition = -7.0f; //X position enemies much reach for game loss
    public float EnemyStartXPosition = 17.5f;
    public Text WinScreen;

    public void PlayerWin()
    {
        WinScreen.gameObject.SetActive(true);
        Debug.Log("Player won!");
    }
}
