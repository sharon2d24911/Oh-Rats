using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviour : MonoBehaviour
{
    public GameObject projectile;
    private GameObject myProjectile;
    public Transform ProjectileOrigin;
    public float cooldown;
    private bool canShoot;
    public bool defending;
    public float range;
    public float health;
    public float damageTime;
    public float frameRate = 16f;
    public List<Sprite> IdleAnimation;
    private float animTimer;
    public float animTimeMax; //max seconds per frame. concept taken from lab 4
    int animIndex = 0;
    [HideInInspector] public float projAddAttack = 0;
    [HideInInspector] public float projAddSpeed = 0;
    public LayerMask projectileMask;
    private GameObject target;
    private GameObject unit;
    private SpriteRenderer sprite;

    void Start()
    {
        Invoke("ResetCooldown", cooldown);
        unit = gameObject;
        sprite = unit.GetComponent<SpriteRenderer>();
        animTimeMax = animTimeMax / frameRate;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, range, projectileMask);
        Idle();
        if (defending)
        {
            Shoot();
        }
        if (health <= 0)
        {
            Destroy(unit);
        }
    }
    void Idle()
    {
        int animFrames = IdleAnimation.Count;
        animTimer += Time.deltaTime;

       if(animTimer > animTimeMax)
        {
            animTimer = 0;
            if (animIndex < animFrames - 1)
            {
                animIndex++;
            }
            else
            {
                animIndex = 0;
            }
        sprite.sprite = IdleAnimation[animIndex];
        }
    }

    void ResetCooldown()
    {
        canShoot = true;
    }

    public void takeDamage(float dmgAmount)
    {
        health -= dmgAmount;
        StartCoroutine(spriteColorChange(sprite));
        Debug.Log("DAMAGE UNIT");

    }

    IEnumerator spriteColorChange(SpriteRenderer sprite) //kinda doesnt work?????????
    {
        for (int i = 0; i < IdleAnimation.Count; i++)  //in this case, IdleAnimation can be swapped out for other animations when implemented -->function arg
        {
            sprite.color = Color.red;
        }
       yield return new WaitForSeconds(damageTime / 5);
        for (int i = 0; i < IdleAnimation.Count; i++)  //in this case, IdleAnimation can be swapped out for other animations when implemented -->function arg
        {
            sprite.color = Color.white;
        }
    }

    void Shoot()
    {
        if (!canShoot)
           return;
        canShoot = false;
        Invoke("ResetCooldown", cooldown);
        myProjectile = Instantiate(projectile, ProjectileOrigin.position, Quaternion.identity);
        
        // Projectile update to match the unit's boosted stats
        myProjectile.GetComponent<ProjectileScript>().attack += projAddAttack;
        myProjectile.GetComponent<ProjectileScript>().speed += projAddSpeed;

        if (health <= 0)
        {
            Destroy(unit); //kills the unit
        }
    }
}
