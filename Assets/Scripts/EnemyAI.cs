using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;
public class EnemyAI : MonoBehaviour {

	//private static List<Rigidbody2D> EnemyRBs;

    public GameObject scissorPrefab;

	public float moveSpeed = 5f;

	[Range(0f, 1f)]
	public float turnSpeed = .1f;

	public float repelRange = .5f;
	public float repelAmount = 1f;

	public float startMaxChaseDistance = 20f;
	private float maxChaseDistance;

    public EnemyLifeCycle enemyLifeCycle;

    public bool isBarber = false;
    public bool isNick = false;
    float nextTimeToLaugh;

    public SpriteRenderer bossSprite;
    public Sprite finalBossSprite;

    [Header("Ramming")]
    public bool isRam = false;


    private float nextTimeToRam = 2f;
    
    // Tracks the number of frames during which the thing should ram
    private float numRams = 0;
    private float targetNumRams = 10;
    public float ramRate = .5f;

	[Header("Shooting")]
    public bool isExploder;

	public int numShots = 0;
    public float bulletVelocity = 7f;
	public float strafeSpeed = 1f;
	public float shootDistance = 10f;
	public GameObject bulletPrefab;
    public GameObject explosivePrefab;
	public GameObject firePoint;
	public float fireRate = .5f;
    public Sound globShotSound;
    public Sound[] bossSounds;

	public float nextTimeToFire = 2f;

    private float movementDelay;
	private Rigidbody2D rb;

	private Vector3 velocity;

    // For barber
    private float nextStrafeSwitch = 1f;
    private bool currentStrafeDirection = false;
    
    private bool barberShootMode = true;
    private float nextBarberModeSwitch = 8f;

	// Use this for initialization
	void Start () {
        movementDelay = Time.time + 1f;
        nextTimeToFire = Time.time + 1f;

        nextTimeToLaugh = Time.time + 10f;
        
	}

    void Awake()
    {
		rb = GetComponent<Rigidbody2D>();        
        if(globShotSound != null)
        {
            globShotSound.source = gameObject.AddComponent<AudioSource>();
            globShotSound.source.clip = globShotSound.clip;
            globShotSound.source.volume = globShotSound.volume;
            globShotSound.source.loop = globShotSound.loop;
        }
        foreach (Sound sound in bossSounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
        }
    }

	private void OnDestroy()
	{
		//EnemyRBs.Remove(rb);
	}

	// Update is called once per frame
	void FixedUpdate () {
        //hacky, but works for now
        if(Time.time < movementDelay)
		{
            return;    
        }

		float distance = Vector2.Distance(rb.position, PlayerMovement2D.Position);

		Vector2 direction = (PlayerMovement2D.Position - rb.position).normalized;

		Vector2 newPos;

        if(isNick)
        {
            NickAI(direction, distance);
        }
        else if(isBarber)
        {
            if(Time.time >= nextBarberModeSwitch)
            {
                nextBarberModeSwitch = Time.time + Random.Range(4,8);
                barberShootMode = !barberShootMode;
                if(barberShootMode == false)
                {
                    //Spawn some scissors
                    Vector2 spawnPos = (Vector2)transform.position + new Vector2(2.5f,0);
                    Instantiate(scissorPrefab, spawnPos, Quaternion.identity);
                    spawnPos = (Vector2)transform.position + new Vector2(-2.5f,0);
                    Instantiate(scissorPrefab, spawnPos, Quaternion.identity);

                    StartCoroutine(ShootCircle(12, 0));
                    if(enemyLifeCycle.health < 6666)
                    {
                        StartCoroutine(ShootCircle(12, 3));
                    }
                    if(enemyLifeCycle.health < 4000)
                    {
                        StartCoroutine(ShootCircle(12, 1.5f));
                    }
                }
                else
                {
                    if(enemyLifeCycle.health < 5000)
                    {
                        StartCoroutine(ShootCircle(12, 2));
                    }
                    if(enemyLifeCycle.health < 2000)
                    {
                        StartCoroutine(ShootCircle(12, 4));
                    }
                }
            }
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = 0f;
            if(barberShootMode)
            {
                if (Time.time >= nextStrafeSwitch)
                {
                    nextStrafeSwitch = Time.time + Random.Range(1,5);
                    currentStrafeDirection = !currentStrafeDirection;
                }
                ShootWithRandomAdjustment(30f);

                if (distance > 9)
                {
                    newPos = MoveRegular(direction);
                } else
                {
                    newPos = (Vector2)transform.position + (currentStrafeDirection ? Vector2.Perpendicular(direction) : -1 * Vector2.Perpendicular(direction)) * Time.fixedDeltaTime * strafeSpeed;
                }
            }
            else
            {
                if(distance > 9)
                {
                    newPos = MoveRegular(direction);
                }
                else
                {
                    newPos = MoveStrafing(direction);
                }
            }
            rb.MovePosition(newPos);

        }
		else if (numShots > 0)
		{
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg  /* - 90f*/;
			rb.rotation = angle;

			if (distance > shootDistance)
			{
				newPos = MoveRegular(direction);
			} else
			{
				newPos = MoveStrafing(direction);
			}

			Shoot();

            rb.MovePosition(newPos);
        
			rb.rotation = angle;

		}
        else if(isRam){
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
			rb.rotation = Mathf.LerpAngle(rb.rotation, angle, turnSpeed);

            if(numRams >= targetNumRams)
            {
                nextTimeToRam = Time.time + 1f / ramRate;
                numRams = 0;
            }
            
            if (Time.time >= nextTimeToRam)
            {
                newPos = MoveRam(direction);
                numRams++;
            }
            else
            {
                newPos = MoveStrafingRam(direction);
            }
            rb.MovePosition(newPos);

        }
        else if(isExploder)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
			rb.rotation = angle;
            if (distance > shootDistance)
			{
				newPos = MoveRegular(direction);
			} else
			{
				newPos = MoveStrafing(direction);
			}
            Vector2 direction1 = (PlayerMovement2D.Position - (Vector2)firePoint.transform.position).normalized;
            StartCoroutine(ShootExplosive(8, 1.25f,direction1,0));
            rb.MovePosition(newPos);
        }   
        else
		{
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
			rb.rotation = Mathf.LerpAngle(rb.rotation, angle, turnSpeed);

			newPos = MoveRegular(direction);
            
            rb.MovePosition(newPos);

		}
	}

	void Shoot ()
	{
		if (Time.time >= nextTimeToFire)
		{
			GameObject bullet1 = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
            
            globShotSound.source.Play();


            Vector2 direction = (PlayerMovement2D.Position - (Vector2)firePoint.transform.position).normalized;
            

            bullet1.GetComponent<Rigidbody2D>().velocity = direction * bulletVelocity; 

            float offset = 60/numShots;
            for(int i = 1; i < numShots; ++i)
            {
                float angle = offset * (i % 2 == 0 ? -1 * (i - 1) : 1 * i);
                GameObject bullet2 = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
                bullet2.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, angle) * direction * bulletVelocity;
            }



			nextTimeToFire = Time.time + 1f / fireRate;
		}
	}

    void ShootWithRandomAdjustment (float range)
	{
		if (Time.time >= nextTimeToFire)
		{
			GameObject bullet1 = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
            
            globShotSound.source.Play();


            Vector2 direction = (PlayerMovement2D.Position - (Vector2)firePoint.transform.position).normalized;
            


            bullet1.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, Random.Range(-1*range,range)) * direction * bulletVelocity * Random.Range(.9f,1.1f); 

			nextTimeToFire = Time.time + 1f / fireRate;
		}
	}

    IEnumerator ShootCircle (int numShots, float waitSec)
	{
        yield return new WaitForSeconds(waitSec);
        float curAngle = 0f;
        for(int i = 0; i < numShots; ++i)
        {
            GameObject bullet1 = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
            
            globShotSound.source.Play();


            Vector2 direction = (PlayerMovement2D.Position - (Vector2)firePoint.transform.position).normalized;
            
            float bulletVelocity = 4f;

            bullet1.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, curAngle) *direction * bulletVelocity;
            curAngle += 360/numShots;
        }
	}

    IEnumerator ShootExplosive (int numShots, float waitSec, Vector2 direction1, float shotDelay, bool forceShot = false)
	{
        if (!forceShot && Time.time < nextTimeToFire)
		{
            yield break;
        }
        yield return new WaitForSeconds(shotDelay);
        GameObject bullet1 = Instantiate(explosivePrefab, firePoint.transform.position, firePoint.transform.rotation);
            
        globShotSound.source.Play();       

        bullet1.GetComponent<Rigidbody2D>().velocity = direction1 * bulletVelocity; 
        StartCoroutine(Explode(bullet1, numShots, waitSec));
        nextTimeToFire = Time.time + 1f / fireRate;
	}

    IEnumerator Explode(GameObject bullet1, int numShots, float waitSec)
    {
        yield return new WaitForSeconds(waitSec);
        float curAngle = 0f;
        for(int i = 0; i < numShots; ++i)
        {
            if(bullet1 == null)
            {
                yield break;
            }
            GameObject bullet = Instantiate(bulletPrefab, bullet1.transform.position, bullet1.transform.rotation);
        
            Vector2 direction = (PlayerMovement2D.Position - rb.position).normalized;
            
            float bulletVelocity = 4f;

            bullet.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, curAngle) *direction * bulletVelocity;
            curAngle += 360/numShots;
        }
        Destroy(bullet1);
    }

	Vector2 MoveStrafing (Vector2 direction)
	{
		Vector2 newPos = transform.position + transform.up * Time.fixedDeltaTime * strafeSpeed;
		return newPos;
	}

    Vector2 MoveStrafingRam (Vector2 direction)
	{
		Vector2 newPos = transform.position + Quaternion.Euler(0, 0, 90) * transform.up * Time.fixedDeltaTime * strafeSpeed;
		return newPos;
	}

	Vector2 MoveRegular (Vector2 direction)
	{
		Vector2 newPos = transform.position + (Vector3)direction * Time.fixedDeltaTime * moveSpeed;
		return newPos;
	}

    Vector2 MoveRam (Vector2 direction)
	{

		Vector2 newPos = transform.position + (Vector3)direction * Time.fixedDeltaTime * 16;
		return newPos;
	}

    bool laughing = false;
    bool shouldShoot = true;
    float nextBossAttack = 0f;

    void NickAI(Vector2 direction, float distance)
    {
        Vector2 newPos = rb.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        rb.rotation = Mathf.LerpAngle(rb.rotation, angle, turnSpeed);

        if(Time.time >= nextTimeToLaugh)
        {
            Animator animator = GetComponent<Animator>();
            animator.enabled = true;
            int num = Random.Range(0, bossSounds.Length-1);
            StartCoroutine(Laugh(bossSounds[num], animator));
            nextTimeToLaugh = Time.time + Random.Range(10,16);
            laughing = true;
        }

        if(laughing)
        {
            return;
        }

        //Vector2 direction1 = (PlayerMovement2D.Position - (Vector2)firePoint.transform.position).normalized;
        //StartCoroutine(ShootExplosive(8, 1.25f,direction1,0));

        if(Time.time >= nextBossAttack)
        {
            nextBossAttack = Time.time + Random.Range(8f,10f);
            int attack = Random.Range(0,4);
            switch(attack)
            {
                case 0:
                    shouldShoot = false;
                    StartCoroutine(StarShot(direction));
                    break;
                case 1:
                    StartCoroutine(SpreadShot(0));
                    StartCoroutine(SpreadShot(1));
                    StartCoroutine(SpreadShot(2));
                    StartCoroutine(SpreadShot(3));
                    break;
                case 2:
                    shouldShoot = false;
                    nextBossAttack += 3f;
                    StartCoroutine(CrazyCircleShot());
                    break;
                case 3:
                    StartCoroutine(CrazyStraightShot());
                    break;
                default:
                    break;
            }
        }
        if(shouldShoot)
        {
            ShootWithRandomAdjustment(45f);
        }

        if (Time.time >= nextStrafeSwitch)
        {
            nextStrafeSwitch = Time.time + Random.Range(1,10);
            currentStrafeDirection = !currentStrafeDirection;
        }

        if (distance > 8)
        {
            newPos = MoveRegular(direction);
        } else
        {
            newPos = (Vector2)transform.position + (currentStrafeDirection ? Vector2.Perpendicular(direction) : -1 * Vector2.Perpendicular(direction)) * Time.fixedDeltaTime * strafeSpeed;
        }

        rb.MovePosition(newPos);
    }

    IEnumerator CrazyCircleShot()
    {
        Vector2 direction = (PlayerMovement2D.Position - rb.position).normalized;
        for(int i = 0; i < 60; ++i)
        {
            yield return new WaitForSeconds(.1f);
            GameObject bullet1 = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
            bullet1.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, i * 12) * direction * 6f;
            GameObject bullet2 = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
            bullet2.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 180) * Quaternion.Euler(0, 0, i * 12) * direction * 6f;
        }
        yield return new WaitForSeconds(2);    
        shouldShoot = true;
    }

    IEnumerator CrazyStraightShot()
    {
        Vector2 direction = (PlayerMovement2D.Position - rb.position).normalized;
        for(int i = 0; i < 500; ++i)
        {
            if(i%20 == 0){
                direction = Quaternion.Euler(0, 0, Random.Range(-90,90)) * direction;
            }
            yield return new WaitForSeconds(.005f);
            GameObject bullet1 = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
            bullet1.GetComponent<Rigidbody2D>().velocity = direction * 9f;
        }
    }


    IEnumerator SpreadShot(float wait)
    {
        yield return new WaitForSeconds(wait);
        Vector2 direction = (PlayerMovement2D.Position - rb.position).normalized;
        GameObject bullet1 = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
        
        bullet1.GetComponent<Rigidbody2D>().velocity = direction * bulletVelocity; 

        float offset = 120/12;
        for(int i = 1; i < 12; ++i)
        {
            float angle = offset * (i % 2 == 0 ? -1 * (i - 1) : 1 * i);
            GameObject bullet2 = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
            bullet2.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, angle) * direction * 6f;
        }
    }


    IEnumerator StarShot(Vector2 direction)
    {
        float offset = 15;
        for(float i = 0; i < 12; ++i)
        {
            float angle = 90 - offset * i;
            StartCoroutine(ShootExplosive(9, .75f, Quaternion.Euler(0, 0, angle) * direction, 5 - i*(5f/12f), true));
        }
        yield return new WaitForSeconds(6);    
        shouldShoot = true;
    }

    IEnumerator Laugh(Sound sound, Animator animator)
    {
        sound.source.Play();
        float animLength = 0f;
        animLength = sound.clip.length;
        animator.speed = .5f;
        animator.Play("NickNegaMouthTalk");

        StartCoroutine(ActivateShooting(animLength));

        yield return new WaitForSeconds(animLength);

        animator.enabled = false;
        animator.speed = 1f;
        bossSprite.sprite = finalBossSprite;
    }

    IEnumerator ActivateShooting(float animLength)
    {
        yield return new WaitForSeconds(Math.Min(animLength, 3));
        laughing = false;
    }
}