using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class NickBossEnemy : EnemyLifeCycle {

	public GameObject prefab;

    public Sprite deadSprite;

    public SpriteRenderer sprite;

    public Sound deadSound;

	public float minimum = 0.0f;
    public float maximum = 1f;
    public float duration = 5.0f;
    private float startTime;
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
    void Update()
    {
        healthBar.fillAmount = (float)health/(float)35000;
        if(!fade)
        {
            return;
        }
        float t = (Time.time - startTime) / duration;
        sprite.color = new Color(1f,1f,1f,Mathf.SmoothStep(maximum, minimum, t));        
    }

    void Awake()
    {
        deadSound.source = gameObject.AddComponent<AudioSource>();
        deadSound.source.clip = deadSound.clip;
        deadSound.source.volume = deadSound.volume;
        deadSound.source.loop = deadSound.loop;
    }

    public override GameObject getPrefab()
    {
        return prefab;
    }
	


    public override void Die()
    {
        StartCoroutine(EndSequence());
    }

    IEnumerator EndSequence()
    {
        GameObject.Find("Player").GetComponent<PlayerCombat>().enabled = false;
        GameObject.Find("Player").GetComponent<PlayerMovement2D>().enabled = false;
        GetComponent<EnemyAI>().enabled = false;
        GetComponent<EnemyAI>().StopAllCoroutines();
        var bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for(int i = bullets.Length - 1; i >= 0; --i)
        {
            Destroy(bullets[i]);
        }
        deadSound.source.Play();
        sprite.sprite = deadSprite;
        startTime = Time.time;
        fade = true;
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Destroy(gameObject);
    }

    

}
