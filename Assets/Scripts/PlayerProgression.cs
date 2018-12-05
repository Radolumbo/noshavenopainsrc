using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgression : MonoBehaviour {

    public List<Sprite> sprites;
    public SpriteRenderer playerSprite;
    public int spriteNum = 0;

	// Use this for initialization
	void Start () {
		spriteNum = Levels.GetLevelNum() - 1;
        playerSprite.sprite = sprites[spriteNum];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NextSprite()
    {
        playerSprite.sprite = sprites[++spriteNum];
    }
}
