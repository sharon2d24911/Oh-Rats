using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float attack;
    public float speed;
    public float collideTime = 0.5f;
    private float destroyTime = 2f;
    private float timer;
    private void Update()
    {
        transform.position += new Vector3(speed * Time.fixedDeltaTime, 0, 0);
        timer += Time.deltaTime;
        if (timer > destroyTime)
        {
            Destroy(gameObject); //destroys projectile after given time
        }
    }
}
