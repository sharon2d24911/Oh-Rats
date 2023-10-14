using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawning : MonoBehaviour
{
    private GameObject GH;
    private GameHandler GameHandler;
    private float waveTimer = 0f;
    private float waveIntervalTimer = 0f;
    private float cooldownIntervalTimer = 0f;
    public float enemyPosHeightAdjust = 0.5f;
    public float waveTimerMax = 5f;
    public float waveTimerMin = 3f;
    public float waveInterval = 10f; //10 seconds
    public float cooldownInterval = 10f; //10 seconds
    private float currentWaveTimeMax;
    private int currentWave = 0;
    public GameObject enemyPrefab;//tech Demo version
    private GameObject lastSpawn;
    private int previousLane;
    private int bossWave = 1;
    public GameObject bossPrefab;//tech Demo version
    //public List<GameObject> enemyPrefabs = new List<GameObject>();
    //public List<TextAsset> waves = new List<TextAsset>();

    // Start is called before the first frame update
    void Start()
    {
        GH = GameObject.Find("GameHandler");
        GameHandler = GH.GetComponent<GameHandler>();
        currentWaveTimeMax = Random.Range(waveTimerMin, waveTimerMax);
    }

    // Update is called once per frame
    void Update()
    {
        waveTimer += Time.deltaTime;

        if (cooldownIntervalTimer < cooldownInterval)
        {
            cooldownIntervalTimer += Time.deltaTime;
        }
        else if (waveIntervalTimer < waveInterval)
        {
            waveIntervalTimer += Time.deltaTime;
            if (waveTimer > currentWaveTimeMax)
            {
                Vector3 enemyPos = selectLane();
                if (currentWave == bossWave)
                {
                    Vector3 bossPos = GameHandler.bossPos;
                    Spawn(bossPrefab, bossPos);
                    currentWave += 1;
                }
                else if (currentWave < bossWave)
                {
                    Spawn(enemyPrefab, enemyPos); //enemyPrefab to be replaced with more expandable version
                }
                waveTimer = 0;
                currentWaveTimeMax = Random.Range(waveTimerMin, waveTimerMax);

            }
        }
        else
        {
            if (lastSpawn && lastSpawn.GetComponent<EnemyBehaviour>().health <= 0)
            {
                Debug.Log("newWave");
                waveIntervalTimer = 0;
                cooldownInterval = 0;
                currentWave += 1;
            }
        }
    }

    Vector3 selectLane()
    {
        GameObject grid = GameObject.Find("Grid"); ;
        GridCreate gridScript = grid.GetComponent<GridCreate>();
        int rows = gridScript.rows;
        int cols = gridScript.columns;
        List<Vector3>  gridPositions = gridScript.getPositions(); //grabs list of grid positions from the GridCreate script
        Debug.Log("range" + (rows - 1));
        int randInd = (Random.Range(0, (rows - 1))) * cols;
        Debug.Log("randInd" + randInd);
        while (randInd == previousLane)
        {
            randInd = (Random.Range(0, (rows - 1))) * cols; ;  //stops two rats from spawning in same lane one after another
        }
        Debug.Log("randInd" + randInd);
        Debug.Log("previousLane" + previousLane);
        previousLane = randInd;
        Debug.Log("previousLane" + previousLane);
        Vector3 selectedPos = new Vector3(GameHandler.EnemyStartXPosition, gridPositions[randInd].y + enemyPosHeightAdjust, 0);

        return selectedPos;
    }

    void Spawn(GameObject enemy, Vector3 position)
    {
        lastSpawn = Instantiate(enemy, position, enemy.transform.rotation);
    }
}
