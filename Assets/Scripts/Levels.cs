using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour {

    public static int currentLevel = 1;
    public List<Level> levels;

	// Use this for initialization
	void Start () 
    {
       
	}

    void Awake()
    {
        //currentLevel = 1;
    }

    public void Reset()
    {
        currentLevel = 1;
    }

    public static int GetLevelNum()
    {
        // Always 1 higher than reality atm because I am dumb.
        return currentLevel - 1;
    }
	
	public Level GetLevel()
    {

        var level = levels[currentLevel-1];
        currentLevel++;
        return level;
    }

}
