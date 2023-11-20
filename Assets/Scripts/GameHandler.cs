using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{

    public float GameOverXPosition = -7.0f; //X position enemies much reach for game loss
    public float EnemyStartXPosition = 17.5f;
    public float BossStartXPosition = 21f;
    public float BossStartYPosition = -4.28f;
    private int numOfRats = 0;
    public Vector3 bossPos = new Vector3(22, -3, 1);
    private GameObject BGCanvasGO;
    private Canvas BGCanvas;
    private GameObject UICanvasGO;
    private Canvas UICanvas;
    private int maxRats = 3, playerLivesMaxInd = 3 -1;
    public float ratLifeAdjust;
    public GameObject ratLife;
    private List<GameObject> playerLives = new List<GameObject>();
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


        if (ratLife != null)
        {
            for (int i = 0; i < maxRats; i++)
            {
                GameObject life = Instantiate(ratLife);
                life.transform.parent = BGCanvasGO.transform.GetChild(5);
                playerLives.Add(life);
                life.transform.position += new Vector3(i * ratLifeAdjust, 0f,0f);
                life.transform.position = new Vector3(life.transform.position.x, -9.1f, 4f);
            }
        }
    }
     
    public IEnumerator PlayerWin()
    {
        yield return new WaitForSeconds(7f);
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().StopMusic("BassyMain", "BassyEvent", "BassyDrums", "none");
        SceneManager.LoadScene("WinScene");
        Debug.Log("Player won!");
    }

    public void PlayerLoss()
    {
        numOfRats += 1;
        if (numOfRats == maxRats)
        {
            GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().StopMusic("BassyMain", "BassyEvent", "BassyDrums", "none");
            AudioManager.Instance.PlaySFX("GameOver", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["GameOver"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["GameOver"][1]);
            SceneManager.LoadScene("LoseScene");
            Debug.Log("Player lost!");
        }
        AudioManager.Instance.PlaySFX("PlayerLifeLost", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["PlayerLifeLost"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["PlayerLifeLost"][1]);
        Destroy(playerLives[playerLivesMaxInd]);
        playerLivesMaxInd -= 1;

    }
}
