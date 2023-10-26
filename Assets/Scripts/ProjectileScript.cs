using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float attack;
    public float speed;
    public float collideTime = 0.5f;
    public float screenEdge = 13.3f;
    private void Update()
    {
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        if (transform.position.x > screenEdge)
        {
            Destroy(gameObject); //destroys projectile after reaching the edge of the screen
        }
    }
}
