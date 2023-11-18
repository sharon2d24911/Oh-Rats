﻿using System.Collections;
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
    public GameObject phase2;

    [Header("Projectiles")]
    public GameObject projectile;
    private GameObject myProjectile;
    public Transform ProjectileOrigin;
    public bool isProjectileShooter = false;
    private string fireSound;
    public float cooldown;
    public bool canShoot = true;
    private string hitsSound;
    private string biteSound;
    private string hurtSound;
    private string defeatSound;
    private string capHurtSound;

    [Header("Close Range Projectile")]
    public bool isThrower = false;
    public GameObject puddle;


    [Header("Animation")]
    //======Animation Stuff=========
    public float frameRate = 4f;
    private float animTimer;
    public float animTimeMax; //max seconds per frame. concept taken from lab 4
    public Animations[] animations;
    private string currentAnim = "Walk";
    private int animNum = 0;
    private int animIndex = 0;
    private SpriteRenderer sprite;
    private SpriteRenderer attackLayer;
    private SpriteRenderer healthLayer;
    private SpriteRenderer speedLayer;
    [HideInInspector] public int healthBoost, speedBoost, attackBoost;
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
        switch (currentAnim)
        {
            case "Walk":
                animNum = 0;
                break;
            case "Attack":
                animNum = 1;
                break;
            case "Death":
                animNum = 2;
                break;
        }
        int animFrames = animations[animNum].BaseAnimation.Count; //should be conistent across all animations, otherwise everything will look wonky
        animTimer += Time.deltaTime;


        if (animTimer > animTimeMax)
        {
            animTimer = 0;
            if (animIndex < animFrames - 1)
            {
                animIndex++;
            }
            else
            {
                if (currentAnim != "Walk" && currentAnim != "Death") //all other animations should end after one cycle
                {
                    currentAnim = "Walk";
                }

                if(currentAnim != "Death")
                {
                    animIndex = 0;
                }    
            }

            //-->cycles through all layers of animations 
            sprite.sprite = animations[animNum].BaseAnimation[animIndex];


            if (animations[animNum].AccessoryAnimation.Count != 0) //if the rat has an accessory
            {
                Debug.Log("animate accessory");
                
                if (Damage2 != null)
                {
                    Damage2.GetComponent<SpriteRenderer>().sprite = animations[animNum].AccessoryAnimation[animIndex];
                }
            }

            if (animations[animNum].BandageAnimation.Count > 0)
            {
                Debug.Log("bandaid animate");
                Damage1.GetComponent<SpriteRenderer>().sprite = animations[animNum].BandageAnimation[animIndex];
            }
            else
            {
                Damage1.GetComponent<SpriteRenderer>().sprite = animations[1].BandageAnimation[4]; //super temporary, so gross, please fix
            }
        }
    }



    IEnumerator takeDamage(float dmgAmount)
    {
        health -= dmgAmount;
        string[] hurtSound = { "RatHurt1", "RatHurt2", "RatHurt3", "RatHurt4" };
        this.hurtSound = hurtSound[Mathf.FloorToInt(Random.Range(0, 4))];
        AudioManager.Instance.PlaySFX(this.hurtSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.hurtSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.hurtSound][1]);
        sprite.color = Color.red;
        yield return new WaitForSeconds(stunTime / 5);
        sprite.color = Color.white;
        StopMovement(stunTime);
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
            speed = 0f;
            //AudioManager.Instance.PlaySFX("Bite1");
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

    public void StopMovement(float resumeTime) //number of seconds
    {
        float timer = 0.0f;

        while(timer < resumeTime)
        {
            currentTime = 0.0f;
            timer += Time.deltaTime;
        }
    }

    void CheckLoss() // --> return int? something to indicate to GameHandler that unit just crossed the line
    {
        Debug.Log("lane: " + lane);
        if (enemy.transform.position.x <= (GameHandler.GameOverXPosition + (GameObject.Find("Grid").gameObject.GetComponent<GridCreate>().rows - lane))) 
        {
            Destroy(enemy);
            GameHandler.PlayerLoss();
            StopMovement(deathTime + 1.0f);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        enemy = gameObject;
        sprite = enemy.GetComponent<SpriteRenderer>();
        rb2d = enemy.GetComponent<Rigidbody2D>();
        GH = GameObject.Find("GameHandler");
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
            string[] hitsSound = {"ProjectileHit1", "ProjectileHit2", "ProjectileHit3"};
            this.hitsSound = hitsSound[Mathf.FloorToInt(Random.Range(0, 3))];
            AudioManager.Instance.PlaySFX(this.hitsSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.hitsSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.hitsSound][1]);
            ProjectileCollide(collision.gameObject);
        }else if (collision.gameObject.tag == "Unit")
        {
            Debug.Log("enemy hit unit");
            UnitBehaviour unitScript = collision.gameObject.GetComponent<UnitBehaviour>();
            if (unitScript.placed)  //only damage placed units
            {
                string[] biteSound = { "Bite1", "Bite2", "Bite3" };
                this.biteSound = biteSound[Mathf.FloorToInt(Random.Range(0, 3))];
                AudioManager.Instance.PlaySFX(this.biteSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.biteSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.biteSound][1]);
                Debug.Log("enemy hit unit");
                StartCoroutine(UnitDamage(unitScript, collision.gameObject.transform));
            }
        }

    }


    IEnumerator Shoot()
    {
        canShoot = false;
        //animIndex = 0;
        //animTimer = 0;
        //currentAnim = "Attack";
        //Animate();
        //yield return new WaitForSeconds(animTimeMax * frameRate);
        yield return new WaitForSeconds(cooldown);
        //Invoke("ResetCooldown", (cooldown - animTimeMax * frameRate));

        // Sfx for projectile fire
        string[] fireSound = { "ProjectileFire1", "ProjectileFire2", "ProjectileFire3", "ProjectileFire4" };
        this.fireSound = fireSound[Mathf.FloorToInt(Random.Range(0, 4))];
        AudioManager.Instance.PlaySFX(this.fireSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.fireSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.fireSound][1]);
        myProjectile = Instantiate(projectile, ProjectileOrigin.position, Quaternion.identity);
        myProjectile.GetComponent<ProjectileScript>().screenEdge = GameHandler.GameOverXPosition + (GameObject.Find("Grid").gameObject.GetComponent<GridCreate>().rows - lane) - 1;
        canShoot = true;
        if (isBoss)
        {
            ProjectileOrigin.localPosition = new Vector3(ProjectileOrigin.localPosition.x, -1f * ProjectileOrigin.localPosition.y, ProjectileOrigin.localPosition.z);
        }
    }



    // Update is called once per frame
    void Update()
    {

        Debug.Log("health" + health);
        if (health <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                if (isBoss)
                {
                    //kills all units/enemies ==> DEATH HOWL
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
                    foreach (GameObject enemy in enemies){
                        enemy.GetComponent<EnemyBehaviour>().health = 0;
                    }
                    foreach(GameObject unit in units){
                        unit.GetComponent<UnitBehaviour>().health = 0;
                    }
                    //finishes current wave
                    WS.waveTimer = 0;
                    WS.waveDurationTimer = WS.waveDuration;

                    //spawns in phase2 decoy if current enemy is the "real" boss
                    if (phase2 != null)
                    {
                        GameObject PII = Instantiate(phase2, enemy.transform.position, enemy.transform.rotation);
                        PII.GetComponent<EnemyBehaviour>().lane = lane;
                    }
                    else {

                        Debug.Log("decoy died");
                        GameHandler.PlayerWin();
                    }
                    
                }
                else {
                    WS.toggleActive(false, lane);
                    string[] defeatSound = { "RatDefeat1", "RatDefeat2" };
                    this.defeatSound = defeatSound[Mathf.FloorToInt(Random.Range(0, 2))];
                    AudioManager.Instance.PlaySFX(this.defeatSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.defeatSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.defeatSound][1]);
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

        //destroys bottle cap and shows bandage if there is a bottle cap
        if (Damage2 != null)
        {
            Damage2.GetComponent<SpriteRenderer>().sortingOrder = sprite.sortingOrder + 1;
            if (health > 0 && health <= 0.50 * initialHealth && !isProjectileShooter) //only destroy if this isnt a projectile shooter
            {
                string[] capHurtSound = { "BottleCapHurt1", "BottleCapHurt2" };
                this.capHurtSound = capHurtSound[Mathf.FloorToInt(Random.Range(0, 2))];
                AudioManager.Instance.PlaySFX(this.capHurtSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.capHurtSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.capHurtSound][1]);
                Destroy(Damage2);
            } else if (health > 0 && health <= 0.25 * initialHealth) {
                Damage1.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 1);
            }
        } 
        //shows bandage if no bottle cap
        else if( (Damage2 == null || isProjectileShooter) && Damage1 != null && health > 0 && health <= 0.50 * initialHealth)
        {
            Damage1.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 1);
        }

        //.tag == "Projectile"
    }
}
