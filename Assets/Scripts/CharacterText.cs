using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterText : MonoBehaviour {

        public GameObject textUI;
        public float ok = 0f;

        public static CharacterText instance;
	    Quaternion rotation;
        string levelUpMessage = "Next Day!";
        
        private List<IEnumerator> coroutines;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }

            rotation = transform.rotation;
        }

        public void ShowLevelUp()
        {
            StartCoroutine(ShowText());
        }

        public IEnumerator ShowText()
        {
            Text text = textUI.GetComponent<Text>();
            if(text.text != "") yield break;
            foreach(char letter in levelUpMessage)
            {
                text.text += letter;
                yield return new WaitForSeconds(.1f);
            }
            yield return new WaitForSeconds(2);
            text.text = "";
        }
        void Update()
        {
        }
        void LateUpdate()
        {
                transform.rotation = rotation;
        }
}
