using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	private Rigidbody2D rb;
	private Vector3 defaultVelocity = Vector3.zero;
    public ParticleSystem levelUpParticle;
    public GameObject deathParticlePrefab;
    public int experienceForLevel = 4;
    public int experienceScale = 3;
    public int currExperience = 0;
    public Image expBar;
    public Text dayText;

    public static Vector2 position;

    public static CharacterController2D instance;

    public int health = 1;

    public PlayerProgression playerProgression;

    public AudioManager audioManager;
    public EnemySpawner enemySpawner;
    public GameFlow gameFlow;

    private bool dyingProcess = false;

    private void Update()
    {
        expBar.fillAmount = (float)currExperience/(float)experienceForLevel;
        dayText.text = "Day " + Levels.GetLevelNum().ToString();
    }

	private void Awake()
	{
        currExperience = 0;
        experienceForLevel = 4 + 2 * (Levels.GetLevelNum());
        if(Levels.GetLevelNum() > 20)
        {
            experienceForLevel += 2 * (Levels.GetLevelNum());
        }
        enemySpawner = GameObject.Find ("GameManager").GetComponent<EnemySpawner> ();
        gameFlow = GameObject.Find ("GameManager").GetComponent<GameFlow> ();

        dyingProcess = false;

		rb = GetComponent<Rigidbody2D>();
        position = rb.position;

        if (instance == null)
        {
            instance = this;
        }
	}

	private void FixedUpdate()
	{
		levelUpParticle.transform.parent = transform;
        position = rb.position;
	}

    public void GainExperience(int exp)
    {
        currExperience += exp;
        if(currExperience >= experienceForLevel)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        currExperience = 0;
        levelUpParticle.Clear();
        levelUpParticle.Play();
        playerProgression.NextSprite();
        CharacterText.instance.ShowLevelUp();
        enemySpawner.NextLevel();
        experienceForLevel = 4 + 2 * (Levels.GetLevelNum());
        if(Levels.GetLevelNum() > 20)
        {
            experienceForLevel += 2 * (Levels.GetLevelNum());
        }
    }

    public void BulletUp()
    {
        PlayerCombat.bulletLevel++;
    }

    void Restart()
    {
        // Hacky as shit, but otherwise when you die it'll increment the level num
        Levels.currentLevel--;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Die()
    {
        if(dyingProcess)
        {
            return;
        }
        dyingProcess = true;
        audioManager.Play("Death");
        Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);

        Invoke("Restart", 2f);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<PolygonCollider2D>().enabled = false;
        GetComponent<PlayerMovement2D>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;
    }

    void OnCollisionEnter2D (Collision2D collisionInfo)
    {
        Collider2D collider = collisionInfo.collider;

        if(collider.tag == "Enemy")
        {
            health -= 1;
            if(health <= 0)
            {
                Die();
            }
        }
        if(collider.tag == "EnemyBullet" || collider.tag == "LaserBullet")
        {
            GameObject bullet = collider.gameObject;
            BulletInfo bulletInfo = bullet.GetComponent<BulletInfo>();
            health -= bulletInfo.GetDamage();
            Destroy(bullet);
            if(health <= 0)
            {
                Die();
            }
        }
        else if(collider.tag == "PowerUp")
        {
            string effect = collider.GetComponent<PowerUpEffects>().What();
            if(effect == "BulletUp")
            {
                BulletUp();
                Destroy(collider.gameObject);
            }
        }
    }

    public void VerticalMove(float move)
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(rb.velocity.x, move * 10f);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref defaultVelocity, m_MovementSmoothing);
    }
	public void Move(float move, bool crouch, bool jump)
	{

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref defaultVelocity, m_MovementSmoothing);

	}


}