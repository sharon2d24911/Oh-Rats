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
    private void Update()
    {
        bool pastEdge = (enemyProjectile ? transform.position.x < screenEdge : transform.position.x > screenEdge);
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        if (pastEdge)
        {
            Debug.Log("position" + transform.position.x + "screenEdge" + screenEdge);
            Debug.Log("WHAR");
            Destroy(gameObject); //destroys projectile after reaching the edge of the screen
        }
    }
}
