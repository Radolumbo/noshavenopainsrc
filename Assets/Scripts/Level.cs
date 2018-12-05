using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level {

    public int levelNumber;
    public List<GameObject> enemyOptions;
    public int maxEnemies;
    [Range(0,5)]
    public float spawnRate;

    public GameObject bossEnemy;

    public int cutsceneNumber = 0;

    public bool spawnRogaine = false; 

    public List<GameObject> GetEnemies()
    {
        return enemyOptions;
    }

    public GameObject GetBoss()
    {
        return bossEnemy;
    }
}
