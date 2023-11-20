using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    [System.Serializable]
    public struct Animations
    {
        public string animation;
        public List<Sprite> BaseAnimation;
        public List<Sprite> AccessoryAnimation;
        public List<Sprite> BandageAnimation;
    }


    private GameObject enemy;
    private Rigidbody2D rb2d;
    private float currentTime;
    private float step = 4.0f;
    private GameObject GH;
    private GameHandler GameHandler;
    private Camera MCamera;
    private WaveSpawning WS;
    private GameObject Damage1;
    private GameObject Damage2;
    private GameObject Bubble;
    public float stunTime = 0.5f;
    public float attackTime = 1.5f;
    public float deathTime = 2.0f;
    public float moveTime = 0.5f;
    public float health;
    private bool isDead = false;
    private float initialHealth;
    [HideInInspector] public int lane = 0;
    private float gridWidth;
    public float damage;
    public float speed;
    public string enemyType;
   

    [Header("Boss")]
    public bool isBoss; //only should be set true for enemies that are bosses, duh
    public GameObject phase1;
    public GameObject phase2;
    public bool pIIActivated = false;

    [Header("Projectiles")]
    public GameObject projectile;
    private GameObject myProjectile;
    public Transform ProjectileOrigin;
    public bool isProjectileShooter = false;
    private string fireSound;
    public float cooldown;
    public bool canShoot = true;
    private string hitsSound;
    private string bottleHurtSound;
    private string hurtSound;
    private string defeatSound;
    private string capBreakSound;
    private string rocketDefeatSound;
    private string coffeeDefeatSound;

    [Header("Close Range Projectile")]
    public bool isThrower = false;
    public GameObject puddle;


    [Header("Animation")]
    //======Animation Stuff=========
    public float frameRate = 4f;
    [HideInInspector] public float animTimer;
    public float animTimeMax; //max seconds per frame. concept taken from lab 4
    public Animations[] animations;
    [HideInInspector] public string currentAnim = "Walk";
    private int animNum = 0;
    [HideInInspector] public int animIndex = 0;
    private SpriteRenderer sprite;
    private SpriteRenderer attackLayer;
    private SpriteRenderer healthLayer;
    private SpriteRenderer speedLayer;
    public int numOfAttacks = 1; //!!!! DO NOT EVER SET TO 0. WILL RESULT IN DIVISION BY ZERO
    [HideInInspector] public int healthBoost, speedBoost, attackBoost, attackAdjust = 0; //attackAdjust adjusts the current index of the attack anim
    //======Animation Stuff=========



    void Move()
    {
        currentTime += Time.deltaTime;

        Animate();

        if (currentTime < moveTime) { 
            
            enemy.transform.position +=  (Vector3.left * step) * Time.deltaTime * speed;
        
        }else if(currentTime >= moveTime)
        {
            currentTime = 0.0f;
        }
        
    }


    void Animate()
    {
        int animFrames = 0;
        switch (currentAnim)
        {
            case "Walk":
                animNum = 0;
                animFrames = animations[animNum].BaseAnimation.Count;
                break;
            case "Attack":
                animNum = 1;
                animFrames = (animations[animNum].BaseAnimation.Count)/numOfAttacks;
                break;
            case "Death":
                animNum = 2;
                animFrames = animations[animNum].BaseAnimation.Count;
                break;
            case "Transition":
                animNum = 1;
                animFrames = (animations[animNum].BaseAnimation.Count) / numOfAttacks;
                break;
        }
        animTimer += Time.deltaTime;

        Debug.Log("animFrames: " + animFrames + "animNum " + animNum + "attackAdjust " + attackAdjust);

        if (animTimer > animTimeMax)
        {
            animTimer = 0;
            if (animIndex < animFrames - 1)
            {
                animIndex++;
            }
            else
            {
                if (currentAnim != "Walk" && currentAnim != "Death" && currentAnim != "Transition") //all other animations should end after one cycle
                {
                    currentAnim = "Walk";
                }

                if(currentAnim != "Death" && currentAnim != "Transition")
                {
                    animIndex = 0;
                    attackAdjust = 0;
                }

                if (currentAnim == "Transition")
                {
                    currentAnim = "Walk";
                }
            }

            //-->cycles through all layers of animations 
            sprite.sprite = animations[animNum].BaseAnimation[animIndex + (animNum == 1 ? attackAdjust : 0)]; //only add the attack adjust if the current anim is attack


            if (animations[animNum].AccessoryAnimation.Count != 0) //if the rat has an accessory
            {
                Debug.Log("animate accessory");
                
                if (Damage2 != null)
                {
                    Damage2.GetComponent<SpriteRenderer>().sprite = animations[animNum].AccessoryAnimation[animIndex + (animNum == 1 ? attackAdjust : 0)];
                }
            }

            if (animations[animNum].BandageAnimation.Count > 0)
            {
                Debug.Log("bandaid animate");
                Damage1.GetComponent<SpriteRenderer>().sprite = animations[animNum].BandageAnimation[animIndex +( (animNum == 1 && isBoss) ? attackAdjust : 0)];
            }
            else
            {
               // Damage1.GetComponent<SpriteRenderer>().sprite = animations[1].BandageAnimation[4]; //super temporary, so gross, please fix
            }
        }
    }



    IEnumerator takeDamage(float dmgAmount)
    {
        health -= dmgAmount;
   
        if (enemyType == "BottleCap")
        {
            string[] bottleHurtSound = { "BottleCapHurt1", "BottleCapHurt2", "BottleCapHurt3", "BottleCapHurt4" };
            this.bottleHurtSound = bottleHurtSound[Mathf.FloorToInt(Random.Range(0, 4))];
            AudioManager.Instance.PlaySFX(this.bottleHurtSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.bottleHurtSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.bottleHurtSound][1]);
        }
        else
        {
            string[] hurtSound = { "RatHurt1", "RatHurt2", "RatHurt3", "RatHurt4" };
            this.hurtSound = hurtSound[Mathf.FloorToInt(Random.Range(0, 4))];
            AudioManager.Instance.PlaySFX(this.hurtSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.hurtSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.hurtSound][1]);
        }
        
        sprite.color = Color.red;
        yield return new WaitForSeconds(stunTime / 5);
        sprite.color = Color.white;
        StartCoroutine(StopMovement(stunTime));
        Debug.Log("health:" + health);

    }

    void ProjectileCollide(GameObject projectile)
    {
        ProjectileScript projectileScript = projectile.GetComponent<ProjectileScript>();
        float dmgAmount = projectileScript.attack;
        if (health > 0)
        {
            StartCoroutine(takeDamage(dmgAmount));
            Destroy(projectile, projectileScript.collideTime);
        }
    }

    IEnumerator UnitDamage(UnitBehaviour unitScript, Transform unitTransform)
    {
        float prevSpeed = speed;
        while (unitScript.health > 0 && health > 0)
        {
            canShoot = false;
            speed = 0f;
            if (isThrower) {
                damage = unitScript.health;
                //insta-kill
            }
            StartCoroutine(unitScript.takeDamage(damage));
            if (isThrower)
            {
                GameObject newPuddle = Instantiate(puddle, unitTransform.position, unitTransform.rotation);
            }
           
            animIndex = 0;
            animTimer = 0;
            currentAnim = "Attack";
            Animate();

            yield return new WaitForSeconds(attackTime);
        }
        Debug.Log("DONE");
        Debug.Log(speed);
        speed = prevSpeed;
        Debug.Log(speed);
    }

    public IEnumerator StopMovement(float resumeTime) //number of seconds
    {
        float prevSpeed = speed;
        speed = 0f;
        yield return new WaitForSeconds(resumeTime);
        speed = prevSpeed;
    }

    void CheckLoss() // --> return int? something to indicate to GameHandler that unit just crossed the line
    {
        Debug.Log("lane: " + lane);
        if (enemy.transform.position.x <= (GameHandler.GameOverXPosition + (GameObject.Find("Grid").gameObject.GetComponent<GridCreate>().rows - lane))) 
        {
            Destroy(enemy);
            GameHandler.PlayerLoss();

            if (isBoss) //insta lose
            {
                GameHandler.PlayerLoss();
                GameHandler.PlayerLoss();
            }
            StartCoroutine(StopMovement(deathTime + 1.0f));
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        enemy = gameObject;
        sprite = enemy.GetComponent<SpriteRenderer>();
        rb2d = enemy.GetComponent<Rigidbody2D>();
        GH = GameObject.Find("GameHandler");
        MCamera = Camera.main;
        GameHandler = GH.GetComponent<GameHandler>();
        WS = GH.GetComponent<WaveSpawning>();
        Damage1 = enemy.transform.GetChild(0).gameObject;

        if (enemy.transform.childCount > 1 && !isBoss)
        {
            Damage2 = enemy.transform.GetChild(1).gameObject;
        }
        else
        {
            Damage2 = null;
        }

        initialHealth = health;
        animTimeMax = animTimeMax / frameRate;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Projectile" && !collision.gameObject.GetComponent<ProjectileScript>().enemyProjectile)
        {
            Debug.Log("projectile hit");
            ProjectileCollide(collision.gameObject);
            string[] hitsSound = { "ProjectileHit1", "ProjectileHit2", "ProjectileHit3" };
            this.hitsSound = hitsSound[Mathf.FloorToInt(Random.Range(0, 3))];
            AudioManager.Instance.PlaySFX(this.hitsSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.hitsSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.hitsSound][1]);
            
        }else if (collision.gameObject.tag == "Unit")
        {
            Debug.Log("enemy hit unit");
            UnitBehaviour unitScript = collision.gameObject.GetComponent<UnitBehaviour>();
            if (unitScript.placed)  //only damage placed units
            {
                //string[] biteSound = { "Bite1", "Bite2", "Bite3" };
                //this.biteSound = biteSound[Mathf.FloorToInt(Random.Range(0, 3))];
                //AudioManager.Instance.PlaySFX(this.biteSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.biteSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.biteSound][1]);
                Debug.Log("enemy hit unit");
                StartCoroutine(UnitDamage(unitScript, collision.gameObject.transform));
            }
        }

    }


    void ResetCooldown()
    {
        canShoot = true;
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        animIndex = 0;
        animTimer = 0;
        currentAnim = "Attack";
        attackAdjust = (animations[1].BaseAnimation.Count) / numOfAttacks;
        Animate();
        yield return new WaitForSeconds(animTimeMax * frameRate);
        Invoke("ResetCooldown", (cooldown - animTimeMax * frameRate));

        // Sfx for projectile fire
        string[] fireSound = { "CoffeeFire1", "CoffeeFire2"};
        this.fireSound = fireSound[Mathf.FloorToInt(Random.Range(0, 2))];
        AudioManager.Instance.PlaySFX(this.fireSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.fireSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.fireSound][1]);
        myProjectile = Instantiate(projectile, ProjectileOrigin.position, Quaternion.identity);
        myProjectile.GetComponent<SpriteRenderer>().sortingOrder = enemy.GetComponent<SpriteRenderer>().sortingOrder + 1;
        myProjectile.GetComponent<ProjectileScript>().screenEdge = GameHandler.GameOverXPosition + (GameObject.Find("Grid").gameObject.GetComponent<GridCreate>().rows - lane) - 1;
        if (isBoss)
        {
            Debug.Log("swap");
            ProjectileOrigin.localPosition = new Vector3(ProjectileOrigin.localPosition.x, -1f * ProjectileOrigin.localPosition.y, ProjectileOrigin.localPosition.z);
        }
    }



    // Update is called once per frame
    void Update()
    {
        //get Phase II decoy to begin life MUAHAHAHA
        if(isBoss && phase1 == null && phase2 == null && !pIIActivated )
        {
            sprite.enabled = true;
            currentAnim = "Transition";
            animIndex = 0;
            animTimer = 0;
            pIIActivated = true;
            Animate();
        }
        if(currentAnim == "Transition")
        {
            Animate();
        }

        Debug.Log("health" + health);
        if (health <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                if (isBoss)
                {
                    //spawns in phase2 decoy if current enemy is the "real" boss
                    if (phase2 != null)
                    {
                        //kills all units/enemies ==> DEATH HOWL
                        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
                        foreach (GameObject enemy in enemies)
                        {
                            enemy.GetComponent<EnemyBehaviour>().health = 0;
                        }
                        foreach (GameObject unit in units)
                        {
                            if (unit.GetComponent<UnitBehaviour>().placed)
                            {
                                unit.GetComponent<UnitBehaviour>().health = 0;
                            }
                        }
                        //finishes current wave
                        WS.waveTimer = 0;
                        WS.waveDurationTimer = WS.waveDuration;
                        GameObject PII = Instantiate(phase2, enemy.transform.position, enemy.transform.rotation);
                        PII.GetComponent<EnemyBehaviour>().lane = lane;
                        PII.GetComponent<EnemyBehaviour>().phase1 = enemy;
                    }
                    else {

                        Debug.Log("decoy died");
                        StartCoroutine(GameHandler.PlayerWin());
                    }
                    
                }
                else {
                    WS.toggleActive(false, lane);
                    if (enemyType == "RocketRat")
                    {
                        string[] rocketDefeatSound = { "RocketDefeat1", "RocketDefeat2" };
                        this.rocketDefeatSound = rocketDefeatSound[Mathf.FloorToInt(Random.Range(0, 2))];
                        AudioManager.Instance.PlaySFX(this.rocketDefeatSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.rocketDefeatSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.rocketDefeatSound][1]);
                    }
                    else if (enemyType == "CoffeeRat")
                    {
                        string[] coffeeDefeatSound = { "CoffeeDefeat1", "CoffeeDefeat2" };
                        this.coffeeDefeatSound = coffeeDefeatSound[Mathf.FloorToInt(Random.Range(0, 2))];
                        AudioManager.Instance.PlaySFX(this.coffeeDefeatSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.coffeeDefeatSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.coffeeDefeatSound][1]);
                    }
                    else
                    {
                        string[] defeatSound = { "RatDefeat1", "RatDefeat2" };
                        this.defeatSound = defeatSound[Mathf.FloorToInt(Random.Range(0, 2))];
                        AudioManager.Instance.PlaySFX(this.defeatSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.defeatSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.defeatSound][1]);
                    }
                }
                    
                animIndex = 0;
                animTimer = 0;
                Destroy(enemy, deathTime); //kills the enemy
            }
            currentAnim = "Death";
            Animate();
        }else if(health > 0)
        {
            Move();
            CheckLoss();
            if (isBoss)
            {
                Debug.Log("lane " + lane);
                WS.toggleActive(true, lane - 1);
                WS.toggleActive(true, lane);
            }
            else
            {
                WS.toggleActive(true, lane);
            }  
            if (canShoot && isProjectileShooter)
            {
                StartCoroutine(Shoot());
            }
        }

        if (Damage1 != null)
        {

            Damage1.GetComponent<SpriteRenderer>().sortingOrder = sprite.sortingOrder + 1;
        }

        if(Damage2 != null)
        {
            Damage2.GetComponent<SpriteRenderer>().sortingOrder = sprite.sortingOrder + 1;
        }

        //destroys bottle cap and shows bandage if there is a bottle cap
        if (enemyType == "BottleCap")
        {
            if (health > 0 && health <= 0.50 * initialHealth && !isProjectileShooter) //only destroy if this isnt a projectile shooter
            {
                string[] capBreakSound = { "BottleCapBreak1", "BottleCapBreak2" };
                this.capBreakSound = capBreakSound[Mathf.FloorToInt(Random.Range(0, 2))];
                AudioManager.Instance.PlaySFX(this.capBreakSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.capBreakSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.capBreakSound][1]);
                Destroy(Damage2);
                enemyType = "Basic";
                initialHealth = 150;
            }
        } 
        //shows bandage if no bottle cap
        else if(enemyType != "BottleCap" && Damage1 != null && health > 0 && health <= 0.50 * initialHealth)
        {
            Damage1.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 1);
        }
        else if (pIIActivated)
        {
            Damage1.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 1);
        }
    }
}
