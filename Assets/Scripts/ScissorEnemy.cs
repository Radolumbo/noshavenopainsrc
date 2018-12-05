using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorEnemy : EnemyLifeCycle {
    public bool bigScissor = false;
    public GameObject prefab;

    public bool isScary = false;

    public Sound snippasnippasnippa;
	// Use this for initialization
	void Start () {
        snippasnippasnippa.source = gameObject.AddComponent<AudioSource>();
        snippasnippasnippa.source.clip = snippasnippasnippa.clip;
        snippasnippasnippa.source.volume = snippasnippasnippa.volume;
        snippasnippasnippa.source.loop = snippasnippasnippa.loop;
        snippasnippasnippa.source.spatialBlend = 1f;
        if(bigScissor)
        {
            snippasnippasnippa.source.pitch = .4f;  
        }

        snippasnippasnippa.source.Play();
    }

    public override GameObject getPrefab()
    {
        return prefab;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDestroy()
    {
        if(snippasnippasnippa != null && snippasnippasnippa.source != null)
        {
            snippasnippasnippa.source.Stop();
        }
    }
}
