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
    public LayerMask projectileMask;
    public float health;

    private GameObject target;

    private void Start()
    {
        Invoke("ResetCooldown", cooldown);
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

    void Shoot()
    {
        if (!canShoot)
           return;
        canShoot = false;
        Invoke("ResetCooldown", cooldown);
        GameObject myProjectile = Instantiate(projectile, ProjectileOrigin.position, Quaternion.identity);
    }
}
