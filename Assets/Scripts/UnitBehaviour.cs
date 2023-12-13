using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviour : MonoBehaviour
{

    [System.Serializable]
    public struct Animations
    {
        public string animation;
        public List<Sprite> BaseAnimation;
        public List<Sprite> AttackAnimation;
        public List<Sprite> HealthAnimation;
        public List<Sprite> SpeedAnimation;
    }

    public GameObject projectile;
    private GameObject myProjectile;
    public Transform ProjectileOrigin;
    public float cooldown;
    private bool canShoot;
    private GameObject GH;
    private Dictionary<Vector2, GameObject> unitPositions;
    [HideInInspector] public bool defending = false;
    [HideInInspector] public bool placed = false;
    public float range;
    public float health;
    public float damageTime;
    [HideInInspector] public float projAddAttack = 0;
    [HideInInspector] public float projAddSpeed = 0;
    public LayerMask projectileMask;
    private GameObject target;
    private GameObject unit;
    private string projectileFire1Sound;
    private string projectileFire2Sound;
    private string projectileFire3Sound;
    private string projectileFire4Sound;
    private int projectileFireCount = 1;
    private string hitsSound;
    private string potionHitSound;

    //======Animation Stuff=========
    public float frameRate = 4f;
    private float animTimer;
    public float animTimeMax; //max seconds per frame. concept taken from lab 4
    public Animations[] animations;
    private string currentAnim = "Idle";
    private int animNum = 0;
    private int animIndex = 0;
    private SpriteRenderer sprite;
    private SpriteRenderer attackLayer;
    private SpriteRenderer healthLayer;
    private SpriteRenderer speedLayer;
    [HideInInspector] public int healthBoost, speedBoost, attackBoost;
    //======Animation Stuff=========


    void Start()
    {
        Debug.Log("Im alive!");

        Debug.Log(healthBoost);
        Debug.Log(speedBoost);
        Debug.Log(attackBoost);

        Invoke("ResetCooldown", cooldown);
        unit = gameObject;
        sprite = unit.GetComponent<SpriteRenderer>();
        GH = GameObject.Find("GameHandler");
        unitPositions = GH.GetComponent<DragCombination>().filledPositions;
        attackLayer = unit.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
        healthLayer = unit.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>();
        speedLayer = unit.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>();
        animTimeMax = animTimeMax / frameRate;

    }

    public void layerSprites(int gridDepth)
    {
        sprite.sortingOrder = gridDepth;
        attackLayer.sortingOrder = gridDepth + 2;
        healthLayer.sortingOrder = gridDepth + 3;
        speedLayer.sortingOrder = gridDepth + 1;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, range, projectileMask);
        if (defending)
        {
            if (canShoot)
            {
                StartCoroutine(Shoot());
            }
        }
        if (health <= 0)
        {
            unitPositions.Remove(unit.transform.position);
            Destroy(unit); //kills the unit
        }
        Animate();
    }


    void Animate()
    {
        switch (currentAnim)
        {
            case "Idle":
                animNum = 0;
                break;
            case "Attack":
                animNum = 1;
                break;
        }
        int animFrames = animations[animNum].BaseAnimation.Count; //should be conistent across all animations, otherwise everything will look wonky
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
                if(animNum != 0) //all other animations should end after one cycle
                {
                    currentAnim = "Idle";
                }
               animIndex = 0;
           }

            //assuming Idle is the first array in "Animations"
            //-->cycles through all layers of animations 
                sprite.sprite = animations[animNum].BaseAnimation[animIndex];
                attackLayer.sprite = animations[animNum].AttackAnimation[animIndex + (animFrames * (attackBoost / 2))]; //adjusts starting point of anim index depending on the boost of each stat
                speedLayer.sprite = animations[animNum].SpeedAnimation[animIndex + (animFrames * (speedBoost / 2))];
                healthLayer.sprite = animations[animNum].HealthAnimation[animIndex + (animFrames * (healthBoost / 2))];

                sprite.color += new Color(0, 0, 0, 1);
                //depending on stats of the unit, change appearance of layers
                attackLayer.color += new Color(0, 0, 0, (attackBoost > 0 ? 1 : 0));  //use of ternary operator to determine whether each respective layer should be visible or not
                speedLayer.color += new Color(0, 0, 0, (speedBoost > 0 ? 1 : 0));
                healthLayer.color += new Color(0, 0, 0, (healthBoost > 0 ? 1 : 0));
        }
    }


    void ResetCooldown()
    {
        canShoot = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Projectile" && collision.gameObject.GetComponent<ProjectileScript>().enemyProjectile && placed)
        {
            Debug.Log("unit projectile hit");
            ProjectileCollide(collision.gameObject);
            if (GameObject.FindWithTag("Projectile").GetComponent<ProjectileScript>().enemyProjectile == false)
            {
                string[] hitsSound = { "CoffeeHit1", "CoffeeHit2", "CoffeeHit3" };
                this.hitsSound = hitsSound[Mathf.FloorToInt(Random.Range(0, 3))];
                AudioManager.Instance.PlaySFX(this.hitsSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.hitsSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.hitsSound][1]);
            }
            else
            {
                string[] potionHitSound = { "PotionHit1", "PotionHit2", "PotionHit3", "PotionHit4" };
                this.potionHitSound = potionHitSound[Mathf.FloorToInt(Random.Range(0, 4))];
                AudioManager.Instance.PlaySFX(this.potionHitSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.potionHitSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.potionHitSound][1]);
            }
            
        }
    }

    void ProjectileCollide(GameObject projectile)
    {
        ProjectileScript projectileScript = projectile.GetComponent<ProjectileScript>();
        float dmgAmount = projectileScript.attack;
        if (projectileScript.bossProjectile)
        {
            dmgAmount = health; //instakill
        }
        if (health > 0)
        {
            StartCoroutine(takeDamage(dmgAmount));
            if (projectileScript.bossProjectile)
            {
                GameObject puddle = Instantiate(projectileScript.puddle, unit.transform.position, unit.transform.rotation);
                puddle.GetComponent<SpriteRenderer>().sortingOrder = sprite.sortingOrder;
            }
            Destroy(projectile, projectileScript.collideTime);
        }
    }

    public IEnumerator takeDamage(float dmgAmount)
    {
        
        health -= dmgAmount;
        sprite.color = Color.red;
        Debug.Log("DAMAGE UNIT");
        if (health <= 0)
        {
            unitPositions.Remove(unit.transform.position);
            Destroy(unit); //kills the unit
        }
        yield return new WaitForSeconds(damageTime / 5);
        if (unit != null && sprite != null)
        {
            sprite.color = Color.white;
        }
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        animIndex = 0;
        animTimer = 0;
        currentAnim = "Attack";
        Animate();
        yield return new WaitForSeconds(animTimeMax * frameRate);
        Invoke("ResetCooldown", (cooldown - animTimeMax * frameRate));
        // Sfx for projectile fire
        if (projectileFireCount == 1)
        {
            string[] projectileFire1Sound = { "ProjectileFire1A", "ProjectileFire1B", "ProjectileFire1C" };
            this.projectileFire1Sound = projectileFire1Sound[Mathf.FloorToInt(Random.Range(0, 3))];
            AudioManager.Instance.PlaySFX(this.projectileFire1Sound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.projectileFire1Sound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.projectileFire1Sound][1]);
            projectileFireCount++;
        }
        else if (projectileFireCount == 2)
        {
            string[] projectileFire2Sound = { "ProjectileFire2A", "ProjectileFire2B", "ProjectileFire2C" };
            this.projectileFire2Sound = projectileFire2Sound[Mathf.FloorToInt(Random.Range(0, 3))];
            AudioManager.Instance.PlaySFX(this.projectileFire2Sound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.projectileFire2Sound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.projectileFire2Sound][1]);
            projectileFireCount++;
        }
        else if (projectileFireCount == 3)
        {
            string[] projectileHit3Sound = { "ProjectileFire3A", "ProjectileFire3B", "ProjectileFire3C" };
            this.projectileFire3Sound = projectileHit3Sound[Mathf.FloorToInt(Random.Range(0, 3))];
            AudioManager.Instance.PlaySFX(this.projectileFire3Sound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.projectileFire3Sound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.projectileFire3Sound][1]);
            projectileFireCount++;
        }
        else
        {
            string[] projectileHit4Sound = { "ProjectileFire4A", "ProjectileFire4B", "ProjectileFire4C" };
            this.projectileFire4Sound = projectileHit4Sound[Mathf.FloorToInt(Random.Range(0, 3))];
            AudioManager.Instance.PlaySFX(this.projectileFire4Sound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.projectileFire4Sound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.projectileFire4Sound][1]);
            projectileFireCount = 1;
        }
        myProjectile = Instantiate(projectile, ProjectileOrigin.position, Quaternion.identity);
        // Projectile update to match the unit's boosted stats
        myProjectile.GetComponent<ProjectileScript>().attack += projAddAttack;
        myProjectile.GetComponent<ProjectileScript>().speed += projAddSpeed;
        myProjectile.GetComponent<SpriteRenderer>().sortingOrder = healthLayer.sortingOrder + 1;
    }
}
