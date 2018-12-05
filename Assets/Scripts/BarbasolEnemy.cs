using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbasolEnemy : EnemyLifeCycle {

    public GameObject prefab;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override GameObject getPrefab()
    {
        return prefab;
    }
}
