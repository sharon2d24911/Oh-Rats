using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private GameObject enemy;
    private Rigidbody2D rb2d;
    private float currentTime;
    private float step = 4.0f;
    private GameObject GH;
    private GameHandler GameHandler;
    private GameObject Damage1;
    public float stunTime = 0.5f;
    public float attackTime = 1.5f;
    public float deathTime = 2.0f;
    public float moveTime = 0.5f;
    public float health;
    private float initialHealth;
    public float damage;
    public float speed;
    public float difficultyIndex;
    public bool isBoss; //only should be set true for enemies that are bosses, duh
    private string hitsSound;
    private string biteSound;
    private string hurtSound;
    void Move()
    {
        currentTime += Time.deltaTime;

        if(currentTime > 0.5*moveTime && currentTime < moveTime) { 
            
            enemy.transform.position +=  (Vector3.left * step) * Time.deltaTime * speed;
        
        }else if(currentTime >= moveTime)
        {
            currentTime = 0.0f;
        }
        
    }

    IEnumerator takeDamage(float dmgAmount)
    {
        health -= dmgAmount;
        SpriteRenderer sprite = enemy.GetComponent<SpriteRenderer>();
        string[] hurtSound = { "RatHurt1", "RatHurt2", "RatHurt3", "RatHurt4" };
        AudioManager.Instance.PlaySFX(this.hurtSound = hurtSound[Mathf.FloorToInt(Random.Range(0, 4))]);
        sprite.color = Color.red;
        yield return new WaitForSeconds(stunTime / 5);
        sprite.color = Color.white;
        StopMovement(stunTime);
        Debug.Log("DAMAGE");
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
            unitScript.takeDamage(damage);

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
        rb2d = enemy.GetComponent<Rigidbody2D>();
        GH = GameObject.Find("GameHandler");
        GameHandler = GH.GetComponent<GameHandler>();
        Damage1 = enemy.transform.GetChild(0).gameObject;
        initialHealth = health;
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
            StartCoroutine(UnitDamage(unitScript));
        }

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("health" + health);
        if (health <= 0)
        {
            if (isBoss)
            {
                Debug.Log("boss health" + health);
                GameHandler.PlayerWin();
            }

            StopMovement(deathTime + 1.0f);
            Destroy(enemy, deathTime); //kills the enemy
        }
        else if (health <= 0.5*initialHealth)
        {
            Damage1.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 1);
        }
        Move();
        CheckLoss();
        //.tag == "Projectile"
    }
}
