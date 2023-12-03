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
        public int ratsAtATime;
        public float duration;
        public int difficulty;
        public float warmUp;
        public float minSpawnInterval;
        public float maxSpawnInterval;
        public string track1;
        public string track2;
        public string track3;
        public string track4;
        public int trackFadeTime;
        public float trackFinalVolume1;
        public float trackFinalVolume2;
        public float trackFinalVolume3;
        public float trackFinalVolume4;
        public float trackSpeed1;
        public float trackSpeed2;
        public float trackSpeed3;
        public float trackSpeed4;
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
    private GameObject boss = null;
    private List<Vector3> gridPositions;
    private Dictionary<Vector2, GameObject> unitPositions;
    [HideInInspector] public float waveTimer = 0f;
    [HideInInspector] public float waveDurationTimer = 0f;
    private float warmupTimer = 0f;
    private float currentWaveTimeMax = 0f;
    private int previousLane;
    private int previousLane2;
    private string coffeeSpawnSound;
    private string rocketSpawnSound;
    private string ratSpawnSound;
    private string kingSummonSound;
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
    [HideInInspector] public float waveDuration;
    private int waveDifficulty;
    private float warmUpDuration;
    private string waveTrack1;
    private string waveTrack2;
    private string waveTrack3;
    private string waveTrack4;
    private int waveRatsAtATime;
    private int waveTrackFadeTime;
    private float waveTrackFinalVolume1;
    private float waveTrackFinalVolume2;
    private float waveTrackFinalVolume3;
    private float waveTrackFinalVolume4;
    private float waveTrackSpeed1;
    private float waveTrackSpeed2;
    private float waveTrackSpeed3;
    private float waveTrackSpeed4;
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

        /*foreach (Wave wave in wavesInFile.waves)
        {
            Debug.Log("max" + wave.maxSpawnInterval + "duration" + wave.duration);
        }*/

        //setup of first wave
        waveTimerMax = wavesInFile.waves[0].maxSpawnInterval;
        waveTimerMin = wavesInFile.waves[0].minSpawnInterval;
        waveDifficulty = wavesInFile.waves[0].difficulty;
        waveRatsAtATime = wavesInFile.waves[0].ratsAtATime;
        waveDuration = wavesInFile.waves[0].duration;
        waveTrack1 = wavesInFile.waves[0].track1;
        waveTrack2 = wavesInFile.waves[0].track2;
        waveTrack3 = wavesInFile.waves[0].track3;
        waveTrack4 = wavesInFile.waves[0].track4;
        waveTrackFadeTime = wavesInFile.waves[0].trackFadeTime;
        waveTrackFinalVolume1 = wavesInFile.waves[0].trackFinalVolume1;
        waveTrackFinalVolume2 = wavesInFile.waves[0].trackFinalVolume2;
        waveTrackFinalVolume3 = wavesInFile.waves[0].trackFinalVolume3;
        waveTrackFinalVolume4 = wavesInFile.waves[0].trackFinalVolume4;
        waveTrackSpeed1 = wavesInFile.waves[0].trackSpeed1;
        waveTrackSpeed2 = wavesInFile.waves[0].trackSpeed2;
        waveTrackSpeed3 = wavesInFile.waves[0].trackSpeed3;
        waveTrackSpeed4 = wavesInFile.waves[0].trackSpeed4;
        warmUpDuration = wavesInFile.waves[0].warmUp;
        waveShowcaseEnemy = wavesInFile.waves[0].showcaseEnemy;
        waveEnemyTypes = wavesInFile.waves[0].enemyTypes;
        waveEnemiesNum = waveEnemyTypes.Length; //number of different enemy types in the wave
        string[] tracks = { waveTrack1, waveTrack2, waveTrack3, waveTrack4 };
        float[] volumes = { waveTrackFinalVolume1, waveTrackFinalVolume2, waveTrackFinalVolume3, waveTrackFinalVolume4 };
        float[] speeds = { waveTrackSpeed1, waveTrackSpeed2, waveTrackSpeed3, waveTrackSpeed4 };

        playTrack(tracks, volumes, speeds); //fade in of new track
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
            //toggleActive(false);
        }

        //once warmup has expired, start wave
        else if (waveDurationTimer < waveDuration)
        {
            
            waveDurationTimer += Time.deltaTime;
            waveTimer += Time.deltaTime;

            if (waveTimer > currentWaveTimeMax)
            {

                //StartCoroutine(Camera.main.GetComponent<AudioManager>().FadeTwo(true, "BassyMain", "BassyEvent", 0.1f, 1f)); // Fade in two music
                ///this line of code was implemented when the bossWave came about
                ///the new version of the waves system doesn't have this included, so a music event like this might have to be handled differently
                ///maybe depending of the enemy type(s) in the wave? or the wave number? a music attribute can also be added to the JSON, although this would likely be limited

                if (boss)
                {
                    EnemyBehaviour EB = boss.GetComponent<EnemyBehaviour>();
                    EB.attackAdjust = 2 * (EB.animations[1].BaseAnimation.Count / EB.numOfAttacks);
                    EB.animIndex = 0;
                    EB.animTimer = 0;
                    EB.currentAnim = "Attack";
                    string[] kingSummonSound = { "KingSummon1", "KingSummon2" };
                    this.kingSummonSound = kingSummonSound[Mathf.FloorToInt(Random.Range(0, 2))];
                    AudioManager.Instance.PlaySFX(this.kingSummonSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.kingSummonSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.kingSummonSound][1]);
                    StartCoroutine(EB.StopMovement(1f));
                }

                for(int i = 0; i < waveRatsAtATime; i++)
                {
                    if (waveShowcaseEnemy != "none" && !waveShowcased)
                    {
                        i = waveRatsAtATime;
                    }
                    Vector3 enemyPos = selectLane();
                    Spawn(enemyPos); //handles which enemy spawns internally
                }
                
                waveTimer = 0f;
                currentWaveTimeMax = Random.Range(waveTimerMin, waveTimerMax); //changes time it should take between spawns
            }
        }

        //once wave has finished, start cooldown when last unit has been killed
        else
        {
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
                    waveRatsAtATime = wavesInFile.waves[currentWave].ratsAtATime;
                    warmUpDuration = wavesInFile.waves[currentWave].warmUp;
                    waveShowcaseEnemy = wavesInFile.waves[currentWave].showcaseEnemy;
                    waveShowcased = false;
                    waveEnemyTypes = wavesInFile.waves[currentWave].enemyTypes;
                    waveEnemiesNum = waveEnemyTypes.Length;
                    wavePropScene = wavesInFile.waves[currentWave].propScene;

                    //Music transition
                    if (wavesInFile.waves[currentWave].track1 == "none")  
                    {
                        for (int i = currentWave; i>0;i--)
                        {
                            if (wavesInFile.waves[i].track1 != "none")
                            {
                                waveTrack1 = wavesInFile.waves[i].track1;
                                break;
                            }
                        }
                        if (wavesInFile.waves[currentWave].track2 =="none")
                        {
                            for (int i = currentWave; i > 0; i--)
                            {
                                if (wavesInFile.waves[i].track2 != "none")
                                {
                                    waveTrack2 = wavesInFile.waves[i].track2;
                                    break;
                                }
                            }
                        }
                        if (wavesInFile.waves[currentWave].track3 == "none")
                        {
                            for (int i = currentWave; i > 0; i--)
                            {
                                if (wavesInFile.waves[i].track3 != "none")
                                {
                                    waveTrack3 = wavesInFile.waves[i].track3;
                                    break;
                                }
                            }
                        }
                        if (wavesInFile.waves[currentWave].track4 == "none")
                        {
                            for (int i = currentWave; i > 0; i--)
                            {
                                if (wavesInFile.waves[i].track4 != "none")
                                {
                                    waveTrack4 = wavesInFile.waves[i].track4;
                                    break;
                                }
                            }
                        }
                        waveTrackFadeTime = wavesInFile.waves[currentWave].trackFadeTime;
                        waveTrackFinalVolume1 = wavesInFile.waves[currentWave].trackFinalVolume1;
                        waveTrackFinalVolume2 = wavesInFile.waves[currentWave].trackFinalVolume2;
                        waveTrackFinalVolume3 = wavesInFile.waves[currentWave].trackFinalVolume3;
                        waveTrackFinalVolume4 = wavesInFile.waves[currentWave].trackFinalVolume4;
                        waveTrackSpeed1 = wavesInFile.waves[currentWave].trackSpeed1;
                        waveTrackSpeed2 = wavesInFile.waves[currentWave].trackSpeed2;
                        waveTrackSpeed3 = wavesInFile.waves[currentWave].trackSpeed3;
                        waveTrackSpeed4 = wavesInFile.waves[currentWave].trackSpeed4;
                        string[] tracks = { waveTrack1, waveTrack2, waveTrack3, waveTrack4 };
                        float[] volumes = { waveTrackFinalVolume1, waveTrackFinalVolume2, waveTrackFinalVolume3, waveTrackFinalVolume4 };
                        float[] speeds = { waveTrackSpeed1, waveTrackSpeed2, waveTrackSpeed3, waveTrackSpeed4 };
                        StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().AdjustMusicVolume(tracks, volumes,speeds));
                    }
                    else //only re-fade in tracks if a transition was mentioned
                    {
                        endTrack(waveTrack1, waveTrack2, waveTrack3, waveTrack4); //fadeout of current track
                        waveTrack1 = wavesInFile.waves[currentWave].track1;
                        waveTrack2 = wavesInFile.waves[currentWave].track2;
                        waveTrack3 = wavesInFile.waves[currentWave].track3;
                        waveTrack4 = wavesInFile.waves[currentWave].track4;
                        waveTrackFadeTime = wavesInFile.waves[currentWave].trackFadeTime;
                        waveTrackFinalVolume1 = wavesInFile.waves[currentWave].trackFinalVolume1;
                        waveTrackFinalVolume2 = wavesInFile.waves[currentWave].trackFinalVolume2;
                        waveTrackFinalVolume3 = wavesInFile.waves[currentWave].trackFinalVolume3;
                        waveTrackFinalVolume4 = wavesInFile.waves[currentWave].trackFinalVolume4;
                        waveTrackSpeed1 = wavesInFile.waves[currentWave].trackSpeed1;
                        waveTrackSpeed2 = wavesInFile.waves[currentWave].trackSpeed2;
                        waveTrackSpeed3 = wavesInFile.waves[currentWave].trackSpeed3;
                        waveTrackSpeed4 = wavesInFile.waves[currentWave].trackSpeed4;
                        string[] tracks = { waveTrack1, waveTrack2, waveTrack3, waveTrack4 };
                        float[] volumes = { waveTrackFinalVolume1, waveTrackFinalVolume2, waveTrackFinalVolume3, waveTrackFinalVolume4 };
                        float[] speeds = { waveTrackSpeed1, waveTrackSpeed2, waveTrackSpeed3, waveTrackSpeed4 };

                        playTrack(tracks, volumes, speeds); //fade in of new track
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


    public void toggleActive(bool state, int lane)
    {
        int rows = gridScript.rows;
        int cols = gridScript.columns;
        int lowerBound = lane * cols, upperBound = lowerBound + (cols - 1);
        //Debug.Log("lowerBound " + lowerBound + "upperBound " + upperBound);
        for(int i =lowerBound; i < upperBound; i++)
        {
            Vector3 currentPos = gridPositions[i];
            if (unitPositions.ContainsKey(currentPos) && unitPositions[currentPos].tag == "Unit")
            {
                GameObject unit = unitPositions[currentPos];
                UnitBehaviour unitScript = unit.GetComponent<UnitBehaviour>();
                unitScript.defending = state;
            }
        }
    }

    void playTrack(string[] trackName, float[] volume, float[] speed)
    {
        if (trackName[0] != "none")
        {
            Debug.Log("PLAY SONG" + trackName[0]);


            StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeIn(trackName, waveTrackFadeTime, volume, speed));

        }
    }

    void endTrack(string trackName1, string trackName2, string trackName3, string trackName4)
    {
        if (trackName1 != "none")
        {
            Debug.Log("End SONG");


            StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeOut(trackName1, trackName2, trackName3, trackName4, waveTrackFadeTime, 0)); 

        }
    }

    Vector3 selectLane()
    {
        int rows = gridScript.rows;
        int cols = gridScript.columns;
        //Debug.Log("range" + (rows - 1));
        int randInd = (Random.Range(0, (rows))) * cols;
        //Debug.Log("randInd" + randInd);
        while (randInd == previousLane || randInd == previousLane2)
        {
            randInd = (Random.Range(0, (rows ))) * cols; ;  //stops two rats from spawning in same lane one after another
        }
        //Debug.Log("randInd" + randInd);
        //Debug.Log("previousLane" + previousLane);
        previousLane2 = previousLane;
        previousLane = randInd;
        //Debug.Log("previousLane" + previousLane);
        Vector3 selectedPos = new Vector3(GameHandler.EnemyStartXPosition, gridPositions[randInd].y + enemyPosHeightAdjust, (randInd/cols) + 1.5f);

        return selectedPos;
    }

    void Spawn(Vector3 position)
    {
        string enemyName;
        if(waveEnemyTypes[0] != "none")
        {
            if (waveShowcaseEnemy != "none" && !waveShowcased)
            {
                enemyName = waveShowcaseEnemy;
                waveShowcased = true;
                if (enemyName == "BossRat")
                {
                    position = new Vector3(GameHandler.BossStartXPosition, GameHandler.BossStartYPosition, 2 + 1.5f);
                }
            }
            else
            {
                float probability = Random.Range(0f, 1f);
                //Debug.Log("probability " + probability);
                float goalPost = 0.5f / (1f - (waveDifficulty * 0.1875f)); //waveDifficulty determines what the probability must equal to spawn a non-basic enemy type
                int randInd = (int)Mathf.Clamp(Mathf.Round((goalPost * probability)), 0f, 1f) * (int)(waveEnemiesNum - Random.Range(1, waveEnemiesNum)); //selects random index between range of enemy types
                //Debug.Log("randInd " + randInd);
                enemyName = waveEnemyTypes[randInd];
                //Debug.Log("randInd " + randInd + " enemyName " + enemyName + " waveEnemiesNum " + waveEnemiesNum);
            }
            GameObject enemy = getEnemyObj(enemyName);
            GameObject spawnedEnemy = Instantiate(enemy, position, enemy.transform.rotation);
            if(enemyName == "BossRat")
            {
                boss = spawnedEnemy;
            }
            //Debug.Log("pos z" + position.z);
            //Debug.Log("layer " + ((int)Mathf.Floor(position.z) * 5));
            spawnedEnemy.GetComponent<SpriteRenderer>().sortingOrder = ((int)Mathf.Floor(position.z) * 5 + 2);
            spawnedEnemy.GetComponent<EnemyBehaviour>().lane = (int)(position.z - 1.5f);
            //Debug.Log("rat lane: " + spawnedEnemy.GetComponent<EnemyBehaviour>().lane);
            //Debug.Log("layer " + (enemy.GetComponent<SpriteRenderer>().sortingOrder));
            // Play spawn sfx
            if (enemyName == "RangedRat")
            {
                string[] coffeeSpawnSound = { "CoffeeSpawn1", "CoffeeSpawn2", "CoffeeSpawn3", "CoffeeSpawn4" };
                this.coffeeSpawnSound = coffeeSpawnSound[Mathf.FloorToInt(Random.Range(0, 4))];
                AudioManager.Instance.PlaySFX(this.coffeeSpawnSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.coffeeSpawnSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.coffeeSpawnSound][1]);
            }
            else if (enemyName == "Boss")
            {
                AudioManager.Instance.PlaySFX("RatKingSpawnFinal", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["RatKingSpawnFinal"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["RatKingSpawnFinal"][1]);
            }
            else if (enemyName == "SpeedRat")
            {
                string[] rocketSpawnSound = { "RocketSpawn1", "RocketSpawn2", "RocketSpawn3", "RocketSpawn4" };
                this.rocketSpawnSound = rocketSpawnSound[Mathf.FloorToInt(Random.Range(0, 4))];
                AudioManager.Instance.PlaySFX(this.rocketSpawnSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.rocketSpawnSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.rocketSpawnSound][1]);
            }
            else if (enemyName == "BasicRat" || enemyName == "HealthRat")
            {
                string[] ratSpawnSound = { "RatSpawn1", "RatSpawn2", "RatSpawn3", "RatSpawn4" };
                this.ratSpawnSound = ratSpawnSound[Mathf.FloorToInt(Random.Range(0, 4))];
                AudioManager.Instance.PlaySFX(this.ratSpawnSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.ratSpawnSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.ratSpawnSound][1]);
            }

        }  
    }
}
