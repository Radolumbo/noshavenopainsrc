using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawner : MonoBehaviour {

	public float startSpawnRadius = 10f;
	private float spawnRadius;
    private float spawnRate = .75f;

    private int maxEnemies;

    public GameObject rogainePrefab;

    public static List<EnemyLifeCycle> currentEnemies;
    
    public Levels levels;
    
    private int levelNum;

	[HideInInspector]
	public Level currentLevel;

	private float nextSpawnTime = 1f;

    void Start()
    {
        currentLevel = levels.GetLevel();
        maxEnemies = currentLevel.maxEnemies;
        spawnRate = currentLevel.spawnRate;
        levelNum = currentLevel.levelNumber;

        GameObject boss = currentLevel.GetBoss();
        if(boss != null)
        {
            SpawnBoss(boss);
        }
        if(currentLevel.cutsceneNumber > 0)
        {
            CutsceneManager cm = GetComponent<CutsceneManager>();
            cm.Play(currentLevel.cutsceneNumber);
        }
        if(currentLevel.levelNumber > 5 && PlayerCombat.bulletLevel < 2)
        {
            SpawnRogaine();
        }
        if(currentLevel.levelNumber > 15 && PlayerCombat.bulletLevel < 3)
        {
            SpawnRogaine();
        }
        if(currentLevel.levelNumber > 20 && PlayerCombat.bulletLevel < 4) 
        {
            SpawnRogaine();
        }
        if(currentLevel.levelNumber > 29 && PlayerCombat.bulletLevel < 5)
        {
            SpawnRogaine();
        }
        
    }

    public void NextLevel()
    {
        var bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for(int i = bullets.Length - 1; i >= 0; --i)
        {
            Destroy(bullets[i]);
        }
        ClearEnemies();
        currentLevel = levels.GetLevel();
        maxEnemies = currentLevel.maxEnemies;
        spawnRate = currentLevel.spawnRate;
        levelNum = currentLevel.levelNumber;

        GameObject boss = currentLevel.GetBoss();
        if(boss != null)
        {
            SpawnBoss(boss);
        }
        if(currentLevel.cutsceneNumber > 0)
        {
            CutsceneManager cm = GetComponent<CutsceneManager>();
            cm.Play(currentLevel.cutsceneNumber);
        }

        if(currentLevel.spawnRogaine || (currentLevel.levelNumber > 5 && PlayerCombat.bulletLevel < 2) ||
                 (currentLevel.levelNumber > 15 && PlayerCombat.bulletLevel < 3) ||
                 (currentLevel.levelNumber > 20 && PlayerCombat.bulletLevel < 4) ||
                 (currentLevel.levelNumber > 29 && PlayerCombat.bulletLevel < 5)){
            SpawnRogaine();
        }

    }

    void Awake()
    {
        currentEnemies = new List<EnemyLifeCycle>();
    }
 
	// Update is called once per frame
	void Update () {
        
        spawnRadius = 20;

		if (Time.time >= nextSpawnTime)
		{
			SpawnWave();
			nextSpawnTime = Time.time + 1f / spawnRate;
		}
	}

    void ClearEnemies()
    {
        for(int i = currentEnemies.Count - 1; i >= 0; i--)
        {
            Destroy(currentEnemies[i].gameObject);
            currentEnemies.RemoveAt(i);
        }
    }
    public static void KillEnemy(int enemyId)
    {
        for(int i = currentEnemies.Count - 1; i >= 0; i--)
        {
            if(currentEnemies[i].getId() == enemyId)
            {
                currentEnemies.RemoveAt(i);
            }
        }

    }


    void SpawnRogaine()
    {
        Vector2 spawnPos = CharacterController2D.position;
        Vector2 randomUnit = Random.insideUnitCircle.normalized;
		spawnPos += randomUnit * 5;
		Instantiate(rogainePrefab, spawnPos, Quaternion.identity);
    }
	void SpawnWave ()
	{
        //Debug.Log("Enemies left: " + currentEnemies.Count.ToString());
		foreach(GameObject enemy in currentLevel.GetEnemies())
		{
            EnemyLifeCycle eType = enemy.GetComponent<EnemyLifeCycle>();
			if (currentEnemies.Count < maxEnemies && Random.value <= eType.spawnChance)
			{
				SpawnEnemy(eType.getPrefab());
			}
		}
	}

    // Make spawning async
    IEnumerator SpawnEnemyWrapper(GameObject enemyPrefab)
    {
        yield return new WaitForEndOfFrame();
        SpawnEnemy(enemyPrefab);
    }

	void SpawnEnemy(GameObject enemyPrefab)
	{
        //return;
		Vector2 spawnPos = CharacterController2D.position + (Random.insideUnitCircle * spawnRadius);

        // Spawn inside bounds and not on the player
        while(spawnPos.x < -19 || spawnPos.x > 12 || spawnPos.y > 13 || spawnPos.y < -8 || Vector2.Distance(CharacterController2D.position, spawnPos) < 5)
        {
            spawnPos = CharacterController2D.position + (Random.insideUnitCircle * spawnRadius);
        }
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
		currentEnemies.Add(enemy.GetComponent<EnemyLifeCycle>());
	}

    // Spawn close
    void SpawnBoss(GameObject enemyPrefab)
	{
        int spawnDistance = 5;
        //Big scissors spawn further pls
        if(enemyPrefab.GetComponent<ScissorEnemy>() != null)
        {
            spawnDistance = 8;   
        }
        
		Vector2 spawnPos = CharacterController2D.position + (Random.insideUnitCircle.normalized * spawnDistance);
        //Spawn not on top of player, and not too far above player to see face
        while(spawnPos.x < -19 || spawnPos.x > 12 || spawnPos.y > 13 || spawnPos.y < -8 || (spawnPos.y - CharacterController2D.position.y > 2) )
        {
            spawnPos = CharacterController2D.position + (Random.insideUnitCircle.normalized * spawnDistance);
        }
        GameObject boss = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        if(boss.GetComponent<BarberEnemy>() != null)
        {
            boss.name = "barber";
        }
        else if(boss.GetComponent<NickBossEnemy>() != null)
        {
            boss.name = "neganick";
        }
		currentEnemies.Add(boss.GetComponent<EnemyLifeCycle>());
	}
}
