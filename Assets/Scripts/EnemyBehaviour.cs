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
    }


    private GameObject enemy;
    private Rigidbody2D rb2d;
    private float currentTime;
    private float step = 4.0f;
    private GameObject GH;
    private GameHandler GameHandler;
    private GameObject Damage1;
    private GameObject Damage2;
    public float stunTime = 0.5f;
    public float attackTime = 1.5f;
    public float deathTime = 2.0f;
    public float moveTime = 0.5f;
    public float health;
    private bool isDead = false;
    private float initialHealth;
    public float damage;
    public float speed;
    public float difficultyIndex;
    public bool isBoss; //only should be set true for enemies that are bosses, duh
    private string hitsSound;
    private string biteSound;
    private string hurtSound;

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
                if(Damage2 != null)
                {
                    Damage2.GetComponent<SpriteRenderer>().sprite = animations[animNum].AccessoryAnimation[animIndex];
                }
            }
        }
    }



    IEnumerator takeDamage(float dmgAmount)
    {
        health -= dmgAmount;
        string[] hurtSound = { "RatHurt1", "RatHurt2", "RatHurt3", "RatHurt4" };
        AudioManager.Instance.PlaySFX(this.hurtSound = hurtSound[Mathf.FloorToInt(Random.Range(0, 4))]);
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

    IEnumerator UnitDamage(UnitBehaviour unitScript)
    {
        float prevSpeed = speed;
        while (unitScript.health > 0 && health > 0)
        {
            speed = 0f;
            AudioManager.Instance.PlaySFX("Bite1");
            StartCoroutine(unitScript.takeDamage(damage));
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
        if(enemy.transform.position.x <= GameHandler.GameOverXPosition)
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
        Damage1 = enemy.transform.GetChild(0).gameObject;

        if (enemy.transform.childCount > 1)
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
        if (collision.gameObject.tag == "Projectile")
        {
            Debug.Log("projectile hit");
            string[] hitsSound = {"ProjectileHit1", "ProjectileHit2", "ProjectileHit3"};
            AudioManager.Instance.PlaySFX(this.hitsSound = hitsSound[Mathf.FloorToInt(Random.Range(0, 3))]);
            ProjectileCollide(collision.gameObject);
        }else if (collision.gameObject.tag == "Unit")
        {
            Debug.Log("enemy hit unit");
            string[] biteSound = { "Bite1", "Bite2", "Bite3" };
            AudioManager.Instance.PlaySFX(this.biteSound = biteSound[Mathf.FloorToInt(Random.Range(0, 3))]);
            UnitBehaviour unitScript = collision.gameObject.GetComponent<UnitBehaviour>();
            if (unitScript.placed)  //only damage placed units
            {
                Debug.Log("enemy hit unit");
                StartCoroutine(UnitDamage(unitScript));
            }
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
                animIndex = 0;
                animTimer = 0;
                Destroy(Damage1);
            }
            currentAnim = "Death";
            Animate();
            if (isBoss)
            {
                Debug.Log("boss health" + health);
                GameHandler.PlayerWin();
            }

            Destroy(enemy, deathTime); //kills the enemy
        }else if(health > 0)
        {
            Move();
            CheckLoss();
        }

        if (Damage2 != null)
        {
            if (health > 0 && health <= 0.50 * initialHealth)
            {
                Destroy(Damage2);
            }
        }

        if (health >0 && health <= 0.25*initialHealth)
        {
            Damage1.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 1);
        }

        //.tag == "Projectile"
    }
}
