using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public float GameOverXPosition = -7.0f; //X position enemies much reach for game loss
    public float EnemyStartXPosition = 17.5f;
    public Text WinScreen;
    public Text LoseScreen;
    private int numOfRats = 0;
    public Vector3 bossPos = new Vector3(22, -3, 1);

    public void PlayerWin()
    {
        WinScreen.gameObject.SetActive(true);
        Debug.Log("Player won!");
        Time.timeScale = 0;
    }

    public void PlayerLoss()
    {
        numOfRats += 1;

        if (numOfRats == 3)
        {
            LoseScreen.gameObject.SetActive(true);
            Debug.Log("Player lost!");
            Time.timeScale = 0;
        }

    }
}
