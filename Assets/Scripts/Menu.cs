using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class Menu : MonoBehaviour {

    public InputField passwordInput;

    public void StartGame()
    {
        if(passwordInput.text != "")
        {
            string isolatedCode = passwordInput.text.Remove(0,5);
            Levels.currentLevel = int.Parse(new string(isolatedCode.ToCharArray().Select(c => (char)(c - 50)).ToArray()));
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
