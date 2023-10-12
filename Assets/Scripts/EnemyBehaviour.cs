using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private GameObject enemy;
    private float currentTime;
    private float step = 4.0f;
    private GameObject GH; //can be put somewhere else if Enemy becomes a subclass of something else????
    private GameHandler GameHandler;
    public float stunTime = 0.5f;
    public float attackTime = 1.5f;
    public float deathTime = 2.0f;
    public float moveTime = 0.5f;
    public float health;
    public float damage;
    public float speed;
    public float difficultyIndex;

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

    void TakeDamage(float dmgAmount)
    {
        health -= dmgAmount;
        StopMovement(stunTime);
        Debug.Log("DAMAGE");

    }

    void ProjectileCollide(GameObject projectile)
    {
        ProjectileScript projectileScript = projectile.GetComponent<ProjectileScript>();
        float dmgAmount = projectileScript.attack;
        TakeDamage(dmgAmount);
        Destroy(projectile, projectileScript.collideTime);
    }

    IEnumerator UnitDamage(UnitBehaviour unitScript)
    {
        float prevSpeed = speed;
        while (unitScript.health > 0 && health > 0)
        {
            speed = 0f;
            unitScript.TakeDamage(damage);

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
            Debug.Log("Player lost!");
            StopMovement(deathTime + 1.0f);
            Destroy(enemy, deathTime);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        enemy = gameObject;
        GH = GameObject.Find("GameHandler");
        GameHandler = GH.GetComponent<GameHandler>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            Debug.Log("projectile hit");
            ProjectileCollide(collision.gameObject);
        }else if (collision.gameObject.tag == "Unit")
        {
            Debug.Log("enemy hit unit");
            UnitBehaviour unitScript = collision.gameObject.GetComponent<UnitBehaviour>();
            StartCoroutine(UnitDamage(unitScript));
        }

    }


    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(enemy, deathTime); //kills the enemy
        }
        Move();
        CheckLoss();
        //.tag == "Projectile"
    }
}
