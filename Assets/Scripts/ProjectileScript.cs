using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float attack;
    public float speed;
    public float collideTime = 0.5f;
    public float screenEdge = 13.3f;
    public bool enemyProjectile = false;
    private float previousRotation = 0f;
    public float spinQuant = 0; //allows projectiles to spin in the air

    [Header("Boss Projectiles")]
    public bool bossProjectile = false;
    public GameObject puddle;

    private void Update()
    {
        bool pastEdge = (enemyProjectile ? transform.position.x < screenEdge : transform.position.x > screenEdge);
        Quaternion newRot = Quaternion.identity;
        previousRotation += spinQuant*Time.deltaTime;
        newRot.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, previousRotation);
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        transform.rotation = newRot; 
        if (pastEdge)
        {
            //Debug.Log("position" + transform.position.x + "screenEdge" + screenEdge);
            Destroy(gameObject); //destroys projectile after reaching the edge of the screen
        }
    }
}
