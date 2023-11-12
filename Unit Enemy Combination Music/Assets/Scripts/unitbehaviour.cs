using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitbehaviour : MonoBehaviour
{
    public GameObject projectile;
    public Transform ProjectileOrigin;
    public float cooldown;
    private bool canShoot;
    public float range;
    public float health;
    private int speed;
    private int damage;
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

    public void takeDamage(float dmgAmount)
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
        GameObject myProjectile = Instantiate(projectile, ProjectileOrigin.position, Quaternion.identity);

        if (health <= 0)
        {
            Destroy(unit); //kills the unit
        }
    }
}
