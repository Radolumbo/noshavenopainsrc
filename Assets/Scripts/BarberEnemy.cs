using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarberEnemy : EnemyLifeCycle {

	public float minimum = 0.0f;
    public float maximum = 1f;
    private float duration = 2.5f;
    private float startTime;
    public SpriteRenderer sprite;
    public Sound deadSound;
    public Sprite deadSprite;

    bool fade = false;
	
    public Image healthBar;
    void Start() 
    {
        var images = GameObject.Find("GameCanvas").GetComponentsInChildren<Image>();
        foreach(var img in images)
        {
            img.enabled = true;
            if(img.gameObject.name == "BossHealthContent")
            {
                healthBar = img;
            }
        }
	}

    void Awake ()
    {
        deadSound.source = gameObject.AddComponent<AudioSource>();
        deadSound.source.clip = deadSound.clip;
        deadSound.source.volume = deadSound.volume;
        deadSound.source.loop = deadSound.loop;
    }
	
	// Update is called once per frame
	void Update () {
        healthBar.fillAmount = (float)health/(float)10000;

		if(!fade)
        {
            return;
        }
        float t = (Time.time - startTime) / duration;
        sprite.color = new Color(1f,.2f,.2f,Mathf.SmoothStep(maximum, minimum, t));     
	}

    public override void Die()
    {
        var images = GameObject.Find("GameCanvas").GetComponentsInChildren<Image>();
        foreach(var img in images)
        {
            if(img.gameObject.name == "BossHealthContent" || img.gameObject.name == "BossHealthMask" || img.gameObject.name == "BossHealthBar")
            {
                img.enabled = false;
            }
        }
        GetComponent<Animator>().enabled = false;
        StartCoroutine(EndSequence());
    }

    IEnumerator EndSequence()
    {
        GetComponent<EnemyAI>().enabled = false;
        var bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for(int i = bullets.Length - 1; i >= 0; --i)
        {
            Destroy(bullets[i]);
        }
        
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i = enemies.Length - 1; i >= 0; --i)
        {
            if(enemies[i].name != "barber")
            {
               Destroy(enemies[i]);
            }
        }
        
        GameObject.Find("Player").GetComponent<PlayerCombat>().enabled = false;
        GameObject.Find("Player").GetComponent<PlayerMovement2D>().enabled = false;
        GetComponent<EnemyAI>().StopAllCoroutines();
        deadSound.source.Play();
        sprite.sprite = deadSprite;
        startTime = Time.time;
        fade = true;
        yield return new WaitForSeconds(3);
        CharacterController2D.instance.LevelUp();
        GameObject.Find("Player").GetComponent<PlayerCombat>().enabled = true;
        GameObject.Find("Player").GetComponent<PlayerMovement2D>().enabled = true;
        GameObject.Find("GameManager").GetComponent<EnemySpawner>().enabled = true;
        Destroy(gameObject);
    }
}
