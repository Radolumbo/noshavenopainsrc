using UnityEngine;

public class EnemyLifeCycle : MonoBehaviour {

    public int health = 50;

    [Range(0f, 1f)]
    public float spawnChance = .2f;
    public bool isBoss = false;
    public int experience = 1;
    Rigidbody2D rb;
	// Use this for initialization

	void Awake () {
        rb = gameObject.GetComponent<Rigidbody2D>();    

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int getId()
    {
        return gameObject.GetInstanceID();
    }

    public virtual GameObject getPrefab()
    {
        return null;
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D (Collision2D collisionInfo)
    {
        Collider2D collider = collisionInfo.collider;

        if(collider.tag == "PlayerBullet")
        {
            //rb.velocity = Vector2.zero;
            GameObject bullet = collider.gameObject;
            BulletInfo bulletInfo = bullet.GetComponent<BulletInfo>();
            health -= bulletInfo.GetDamage();
            Destroy(bullet);
            if(health <= 0)
            {
                EnemySpawner.KillEnemy(gameObject.GetInstanceID());;
                Die();
                if(isBoss)
                {
                    CharacterController2D.instance.LevelUp();
                }
                else
                {
                    CharacterController2D.instance.GainExperience(experience);
                }
            }
        }
    }
}
