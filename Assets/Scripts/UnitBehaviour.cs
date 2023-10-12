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
    [HideInInspector] public float projAddAttack = 0;
    [HideInInspector] public float projAddSpeed = 0;
    public LayerMask projectileMask;
    private GameObject target;
    private GameObject unit;

    private void Start()
    {
        Invoke("ResetCooldown", cooldown);
        unit = gameObject;
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, range, projectileMask);
        Shoot();
    }

    void ResetCooldown()
    {
        canShoot = true;
    }

    public void TakeDamage(float dmgAmount)
    {
        health -= dmgAmount;
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
