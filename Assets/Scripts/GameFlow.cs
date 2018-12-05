using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameFlow : MonoBehaviour {

    public static int currentLevel;

    public Text password;

    public void Start()
    {
        password.text = "";
        //Globally turn off gravity, sketch but w/e
         Physics2D.gravity = Vector2.zero;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 1f - Time.timeScale;
            AudioListener.pause = !AudioListener.pause;
            if(Time.timeScale == 0)
            {
                GameObject.Find("Player").GetComponent<PlayerCombat>().enabled = false;
                password.text = "Password: nick_" + new string((Levels.GetLevelNum().ToString().ToCharArray().Select(c => (char)(c + 50))).ToArray());
            }
            else
            {
                GameObject.Find("Player").GetComponent<PlayerCombat>().enabled = true;
                password.text = "";
            }
        }
    }
}
