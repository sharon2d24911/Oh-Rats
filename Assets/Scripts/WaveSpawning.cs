using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawning : MonoBehaviour
{
    private GameObject GH;
    private GameHandler GameHandler;
    private GameObject grid;
    private GridCreate gridScript;
    private List<Vector3> gridPositions;
    private Dictionary<Vector3, GameObject> unitPositions;
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
        grid = GameObject.Find("Grid"); ;
        gridScript = grid.GetComponent<GridCreate>();
        gridPositions = gridScript.getPositions(); //grabs list of grid positions from the GridCreate script
        unitPositions = GameHandler.GetComponent<DragCombination>().filledPositions;
    }

    // Update is called once per frame
    void Update()
    {
        waveTimer += Time.deltaTime;
        
        //starts with cooldown
        if (cooldownIntervalTimer < cooldownInterval)
        {
            cooldownIntervalTimer += Time.deltaTime;
            toggleActive(false);
        }

        //once cooldown has expired, start wave
        else if (waveIntervalTimer < waveInterval)
        {
            toggleActive(true);
            waveIntervalTimer += Time.deltaTime;
            if (waveTimer > currentWaveTimeMax)
            {
                Vector3 enemyPos = selectLane();
                if (currentWave == bossWave)
                {
                    AudioManager.Instance.PlayMusic("BassyEvent");
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

        //once wave has finished, start cooldown when last unit has been killed
        else
        {
            GameObject[] leftOvers = GameObject.FindGameObjectsWithTag("Enemy");
            if (leftOvers.Length == 0)
            {
                Debug.Log("newWave");
                cooldownIntervalTimer = 0;
                waveIntervalTimer = 0;
                currentWave += 1;
            }
        }
    }

    void toggleActive(bool state)
    {
        for(int i =0; i < gridPositions.Count; i++)
        {
            Vector3 currentPos = gridPositions[i];
            if (unitPositions.ContainsKey(currentPos))
            {
                GameObject unit = unitPositions[currentPos];
                UnitBehaviour unitScript = unit.GetComponent<UnitBehaviour>();
                unitScript.defending = state;
            }
        }
    }

    Vector3 selectLane()
    {
        int rows = gridScript.rows;
        int cols = gridScript.columns;
        Debug.Log("range" + (rows - 1));
        int randInd = (Random.Range(0, (rows))) * cols;
        Debug.Log("randInd" + randInd);
        while (randInd == previousLane)
        {
            randInd = (Random.Range(0, (rows ))) * cols; ;  //stops two rats from spawning in same lane one after another
        }
        Debug.Log("randInd" + randInd);
        Debug.Log("previousLane" + previousLane);
        previousLane = randInd;
        Debug.Log("previousLane" + previousLane);
        Vector3 selectedPos = new Vector3(GameHandler.EnemyStartXPosition, gridPositions[randInd].y + enemyPosHeightAdjust, 1);

        return selectedPos;
    }

    void Spawn(GameObject enemy, Vector3 position)
    {
        Instantiate(enemy, position, enemy.transform.rotation);
    }
}
