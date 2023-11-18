using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveSpawning : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyObjects
    {
        public string enemyName;
        public GameObject enemyPrefab;
    }

    [System.Serializable]
    private class Wave
    {
        public int wave;
        public string[] enemyTypes;
        public string showcaseEnemy;
        public float duration;
        public int difficulty;
        public float warmUp;
        public float minSpawnInterval;
        public float maxSpawnInterval;
        public string track;
        public int trackFadeTime;
        public int trackFinalVolume;
        public int propScene;
    }

    [System.Serializable]
    private class Waves
    {
        public Wave[] waves;
    }


    private GameObject GH;
    private GameHandler GameHandler;
    private GameObject grid;
    private GridCreate gridScript;
    private List<Vector3> gridPositions;
    private Dictionary<Vector2, GameObject> unitPositions;
    private float waveTimer = 0f;
    private float waveDurationTimer = 0f;
    private float warmupTimer = 0f;
    private float currentWaveTimeMax = 0f;
    private int previousLane;
    private string spawnSound;
    private Waves wavesInFile;
    public EnemyObjects[] enemyPrefabs;
    public TextAsset wavesFile;
    public float enemyPosHeightAdjust = 0.5f; //make enemies be at the correct Y coord to allow projectiles to hit them
    public GameObject[] propScenesArr;
    private Prop prop;

    //variables that change depending on current wave in the JSON
    private int currentWave = 0;
    private int wavesNum;
    private bool waveShowcased = false;
    private float waveTimerMax;
    private float waveTimerMin;
    private float waveDuration;
    private int waveDifficulty;
    private float warmUpDuration;
    private string waveTrack;
    private int waveTrackFadeTime;
    private int waveTrackFinalVolume;
    private string waveShowcaseEnemy;
    private string[] waveEnemyTypes;
    private int waveEnemiesNum;
    private int wavePropScene;

    //public float waveTimerMax = 5f; //longest time between enemy spawns
    //public float waveTimerMin = 3f; //shortest time between enemy spawns
    //the spawning system picks a random float between waveTimerMax and waveTimerMin
    //public float waveInterval = 10f; //10 seconds
    //public float cooldownInterval = 10f; //10 seconds

    //Probably not gonna be a thing in final version. TBD
    //private int bossWave = 1;

    //public GameObject enemyPrefab;//tech Demo version
    //public GameObject bossPrefab;//tech Demo version


    // Start is called before the first frame update
    void Start()
    {
        //setup certain references that are important for spawning
        GH = GameObject.Find("GameHandler");
        GameHandler = GH.GetComponent<GameHandler>();
        grid = GameObject.Find("Grid"); ;
        gridScript = grid.GetComponent<GridCreate>();
        gridPositions = gridScript.getPositions(); //grabs list of grid positions from the GridCreate script
        unitPositions = GH.GetComponent<DragCombination>().filledPositions;


        //read in waves JSON, setup
        wavesInFile = JsonUtility.FromJson<Waves>(wavesFile.text);
        wavesNum = wavesInFile.waves.Length;

        foreach (Wave wave in wavesInFile.waves)
        {
            Debug.Log("max" + wave.maxSpawnInterval + "duration" + wave.duration);
        }

        //setup of first wave
        waveTimerMax = wavesInFile.waves[0].maxSpawnInterval;
        waveTimerMin = wavesInFile.waves[0].minSpawnInterval;
        waveDifficulty = wavesInFile.waves[0].difficulty;
        waveDuration = wavesInFile.waves[0].duration;
        waveTrack = wavesInFile.waves[0].track;
        waveTrackFadeTime = wavesInFile.waves[0].trackFadeTime;
        waveTrackFinalVolume = wavesInFile.waves[0].trackFinalVolume;
        warmUpDuration = wavesInFile.waves[0].warmUp;
        waveShowcaseEnemy = wavesInFile.waves[0].showcaseEnemy;
        waveEnemyTypes = wavesInFile.waves[0].enemyTypes;
        waveEnemiesNum = waveEnemyTypes.Length; //number of different enemy types in the wave
        playTrack(waveTrack); //fade in of new track
        wavePropScene = wavesInFile.waves[0].propScene;
        // Set all prop scenes to inactive except the first scene
        foreach (GameObject scene in propScenesArr)
        {
            if (scene.name == "Scene " + wavePropScene.ToString())
                scene.SetActive(true);
            else
                scene.SetActive(false);
        }
        prop = GameObject.Find("Props").GetComponent<Prop>();

    }

    // Update is called once per frame
    void Update()
    {      
        //starts with warmup
        if (warmupTimer < warmUpDuration)
        {
            warmupTimer += Time.deltaTime;
            toggleActive(false);
        }

        //once warmup has expired, start wave
        else if (waveDurationTimer < waveDuration)
        {
            
            toggleActive(true);
            waveDurationTimer += Time.deltaTime;
            waveTimer += Time.deltaTime;

            if (waveTimer > currentWaveTimeMax)
            {
                Vector3 enemyPos = selectLane();

                //StartCoroutine(Camera.main.GetComponent<AudioManager>().FadeTwo(true, "BassyMain", "BassyEvent", 0.1f, 1f)); // Fade in two music
                ///this line of code was implemented when the bossWave came about
                ///the new version of the waves system doesn't have this included, so a music event like this might have to be handled differently
                ///maybe depending of the enemy type(s) in the wave? or the wave number? a music attribute can also be added to the JSON, although this would likely be limited


                Spawn(enemyPos); //handles which enemy spawns internally

                waveTimer = 0f;
                currentWaveTimeMax = Random.Range(waveTimerMin, waveTimerMax); //changes time it should take between spawns
            }
        }

        //once wave has finished, start cooldown when last unit has been killed
        else
        {
            toggleActive(true);
            GameObject[] leftOvers = GameObject.FindGameObjectsWithTag("Enemy");
            if (leftOvers.Length == 0) //checks if there are no object with enemy tag currently in the scene
            {
                Debug.Log("newWave");
                currentWave += 1;

                if(currentWave < wavesNum)
                {

                    //changes variables to be those of the next wave
                    waveTimerMax = wavesInFile.waves[currentWave].maxSpawnInterval;
                    waveTimerMin = wavesInFile.waves[currentWave].minSpawnInterval;
                    waveDuration = wavesInFile.waves[currentWave].duration;
                    waveDifficulty = wavesInFile.waves[currentWave].difficulty;
                    warmUpDuration = wavesInFile.waves[currentWave].warmUp;
                    waveShowcaseEnemy = wavesInFile.waves[currentWave].showcaseEnemy;
                    waveShowcased = false;
                    waveEnemyTypes = wavesInFile.waves[currentWave].enemyTypes;
                    waveEnemiesNum = waveEnemyTypes.Length;
                    wavePropScene = wavesInFile.waves[currentWave].propScene;

                    //Music transition
                    if (wavesInFile.waves[currentWave].track != "none")  //only change tracks if a transition was mentioned
                    {
                        endTrack(waveTrack); //fadeout of current track
                        waveTrack = wavesInFile.waves[currentWave].track;
                        waveTrackFadeTime = wavesInFile.waves[currentWave].trackFadeTime;
                        waveTrackFinalVolume = wavesInFile.waves[currentWave].trackFinalVolume;
                        playTrack(waveTrack); //fade in of new track
                    }

                    // Prop transition
                    if (wavesInFile.waves[currentWave].propScene != 0) // Scene to fade to from what's currently up, 0 to indicate no change
                    {
                        string sceneName = "Scene " + wavesInFile.waves[currentWave].propScene.ToString();
                        string sceneToFade = "Scene " + (wavesInFile.waves[currentWave].propScene - 1).ToString();
                        for (int i = 0; i < propScenesArr.Length; i++)
                        {
                            if (propScenesArr[i].name == sceneName && propScenesArr[i - 1].name == sceneToFade)
                            {
                                prop.TransitionScenes(propScenesArr[i-1], propScenesArr[i]);
                            }
                        }
                    }

                }
                else
                {
                    Debug.Log("waves done!");
                }

                warmupTimer = 0f;
                waveTimer = 0f;
                currentWaveTimeMax = 0f;
                waveDurationTimer = 0f;

            }
        }
    }

   GameObject getEnemyObj(string givenName)
    {
        foreach (EnemyObjects e in enemyPrefabs)
        {
            if(givenName == e.enemyName)
            {
                Debug.Log(e.enemyName);  //how to grab enemy names
                return e.enemyPrefab;
            }
        }
        GameObject empty = new GameObject("NULL"); //returns a "NULL GameObject". so sloppy, but I seriously dont know how else to manage this. please have mercy if this causes bugs
        return empty;
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

    void playTrack(string trackName)
    {
        if (trackName != "none")
        {
            Debug.Log("PLAY SONG" + trackName);


            StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().Fade(true, trackName, waveTrackFadeTime, waveTrackFinalVolume));

        }
    }

    void endTrack(string trackName)
    {
        if (trackName != "none")
        {
            Debug.Log("End SONG");


            StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().Fade(false, trackName, waveTrackFadeTime, 0)); 

        }
    }


    Vector3 selectLane()
    {
        int rows = gridScript.rows;
        int cols = gridScript.columns;
        //Debug.Log("range" + (rows - 1));
        int randInd = (Random.Range(0, (rows))) * cols;
        //Debug.Log("randInd" + randInd);
        while (randInd == previousLane)
        {
            randInd = (Random.Range(0, (rows ))) * cols; ;  //stops two rats from spawning in same lane one after another
        }
        //Debug.Log("randInd" + randInd);
        //Debug.Log("previousLane" + previousLane);
        previousLane = randInd;
        //Debug.Log("previousLane" + previousLane);
        Vector3 selectedPos = new Vector3(GameHandler.EnemyStartXPosition, gridPositions[randInd].y + enemyPosHeightAdjust, (randInd/cols) + 1.5f);

        return selectedPos;
    }

    void Spawn(Vector3 position)
    {
        string enemyName;
        if (waveShowcaseEnemy != "none" && !waveShowcased)
        {
            enemyName = waveShowcaseEnemy;
            waveShowcased = true;
        }
        else
        {
            float probability = Random.Range(0f, 1f);
            Debug.Log("probability " + probability);
            float goalPost = 0.5f / (1f - (waveDifficulty * 0.125f)); //waveDifficulty determines what the probability must equal to spawn a non-basic enemy type
            int randInd = (int)Mathf.Round((goalPost * probability) * (waveEnemiesNum - 1)); //selects random index between range of enemy types
            enemyName = waveEnemyTypes[randInd];
            Debug.Log("randInd " + randInd + " enemyName " + enemyName + " waveEnemiesNum " + waveEnemiesNum);
        }
        GameObject enemy = getEnemyObj(enemyName);
        GameObject spawnedEnemy = Instantiate(enemy, position, enemy.transform.rotation);
        Debug.Log("pos z" + position.z);
        Debug.Log("layer " + ((int)Mathf.Floor(position.z) * 5));
        spawnedEnemy.GetComponent<SpriteRenderer>().sortingOrder = ((int)Mathf.Floor(position.z) * 5);
        spawnedEnemy.GetComponent<EnemyBehaviour>().lane = (int)(position.z - 1.5f);
        Debug.Log("layer " + (enemy.GetComponent<SpriteRenderer>().sortingOrder));
        string[] spawnSound = { "RatSpawn1", "RatSpawn2", "RatSpawn3", "RatSpawn4" };
        AudioManager.Instance.PlaySFX(this.spawnSound = spawnSound[Mathf.FloorToInt(Random.Range(0, 4))]);
    }
}
