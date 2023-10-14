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

    private void Start()
    {
        Invoke("ResetCooldown", cooldown);
        unit = gameObject;
        sprite = unit.GetComponent<SpriteRenderer>();
        animTimeMax = animTimeMax / frameRate;
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, range, projectileMask);
        Shoot();
        Idle();
    }

    private void Idle()
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

    public IEnumerator takeDamage(float dmgAmount)
    {
        health -= dmgAmount;
        sprite.color = Color.red;
        yield return new WaitForSeconds(damageTime / 5);
        sprite.color = Color.white;
        Debug.Log("DAMAGE UNIT");

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
