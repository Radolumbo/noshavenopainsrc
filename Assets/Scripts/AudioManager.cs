using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

	// Use this for initialization
    Dictionary<string, Sound> soundMap = new Dictionary<string, Sound>();
    public List<Sound> sounds;

    public static bool musicOn = true;
    void Start()
    {
        if(musicOn)
        {
            Play("BGMusic");
        }
    }
	void Awake ()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
            soundMap[sound.name] = sound;

        }
	}

    void Update()
    {
        if(Input.GetKeyDown("m"))
        {
            if(!soundMap["BGMusic"].source.isPlaying)
            {
                musicOn = true;
                Play("BGMusic");
            }
            else
            {
                musicOn = false;
                soundMap["BGMusic"].source.Pause();
            }
        }
    }
	public void Play (string name) {
		soundMap[name].source.Play();
	}

    public void Stop (string name) {
		soundMap[name].source.Stop();
	}

}
