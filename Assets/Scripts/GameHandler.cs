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
    private GameObject BGCanvasGO;
    private Canvas BGCanvas;
    private GameObject UICanvasGO;
    private Canvas UICanvas;
    Camera mainCamera;

    private void Start()
    {
        // Ensures at start of game that canvases that render based on camera have the camera attached
        mainCamera = Camera.main;
        BGCanvasGO = GameObject.Find("Background Canvas");
        BGCanvas = BGCanvasGO.GetComponent<Canvas>();
        BGCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        BGCanvas.worldCamera = mainCamera;
        UICanvasGO = GameObject.Find("UI Canvas");
        UICanvas = UICanvasGO.GetComponent<Canvas>();
        UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
        UICanvas.worldCamera = mainCamera;
    }

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
