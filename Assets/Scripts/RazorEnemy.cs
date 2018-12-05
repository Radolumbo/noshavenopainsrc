using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazorEnemy : EnemyLifeCycle {

	public GameObject prefab;
	// Use this for initialization
	void Start () {
        
    }

    public override GameObject getPrefab()
    {
        return prefab;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
