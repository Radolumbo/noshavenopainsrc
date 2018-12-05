using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour {

	// Use this for initialization

    private bool waiting = false;

    public AudioManager audioManager;

    SpriteRenderer bossSprite;
    public Sprite finalBossSprite;

    static bool hasSeenFinal = false;

    
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	}

    public void Play(int sceneNum)
    {         
        GameObject.Find("Player").GetComponent<PlayerMovement2D>().enabled = false;
        GameObject.Find("Player").GetComponent<PlayerCombat>().enabled = false;
        GameObject.Find("GameManager").GetComponent<EnemySpawner>().enabled = false;
        // I'm just too tired to think of a better way to do this atm
        if(sceneNum == 1)
        {
            GameObject bossObject = GameObject.Find("barber");
            bossObject.GetComponent<EnemyAI>().enabled = false;
            Animator animator = bossObject.GetComponent<Animator>();
            StartCoroutine(PlayBarberAnimation(animator));
        }

        else if(sceneNum == 2)
        {
            GameObject bossObject = GameObject.Find("neganick");
            Animator animator = bossObject.GetComponent<Animator>();
            bossSprite = bossObject.GetComponent<SpriteRenderer>();
            if(hasSeenFinal)
            {
                animator.enabled = false;
                bossSprite.sprite = finalBossSprite;
                GameObject.Find("Player").GetComponent<PlayerMovement2D>().enabled = true;
                GameObject.Find("Player").GetComponent<PlayerCombat>().enabled = true;
                GameObject.Find("GameManager").GetComponent<EnemySpawner>().enabled = true;
                GameObject.Find("neganick").GetComponent<EnemyAI>().enabled = true;
                return;
            }
            hasSeenFinal = true;
            bossObject.GetComponent<EnemyAI>().enabled = false;
            StartCoroutine(PlayNickAnimation(animator));
        }
    }

    IEnumerator PlayBarberAnimation(Animator animator)
    {
        float animLength = 0f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == "BarberMouthAnimation") {
                animLength = clip.length * 8;
            }
        }
        animator.Play("BarberMouthAnimation");
        audioManager.Play("A Little Off The Top");

        yield return new WaitForSeconds(animLength);
        StartCoroutine(PlayBarberEyebrow(animator));
    }

    IEnumerator PlayBarberEyebrow(Animator animator)
    {
        float animLength = 0f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == "BarberEyebrowAnimation") {
                animLength = clip.length;
            }
        }
        animator.Play("BarberEyebrowAnimation");
        yield return new WaitForSeconds(animLength);
        animator.Play("BarberIdleAnimation");
        GameObject.Find("Player").GetComponent<PlayerMovement2D>().enabled = true;
        GameObject.Find("Player").GetComponent<PlayerCombat>().enabled = true;
        GameObject.Find("GameManager").GetComponent<EnemySpawner>().enabled = true;
        GameObject.Find("barber").GetComponent<EnemyAI>().enabled = true;
    }

    
    IEnumerator PlayNickAnimation(Animator animator)
    {
        float animLength = 0f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == "NickMouthTalk") {
                animLength = clip.length * 16;
            }
        }
        animator.Play("NickMouthTalk");
        audioManager.Play("Covered In Hair");

        yield return new WaitForSeconds(animLength);
        StartCoroutine(PlayNegatizeAnimation(animator));
    }

    IEnumerator PlayNegatizeAnimation(Animator animator)
    {
        float animLength = 0f;
        animator.speed = 2f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == "NickNegatize") {
                animLength = clip.length/2;
            }
        }
        animator.Play("NickNegatize");
        yield return new WaitForSeconds(animLength);

        StartCoroutine(PlayNegaNickMouthAnimation(animator));
    }

    IEnumerator PlayNegaNickMouthAnimation(Animator animator)
    {
        float animLength = 0f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == "NickNegaMouthTalk") {
                animLength = clip.length * 10;
            }
        }
        animator.speed = .6f;
        animator.Play("NickNegaMouthTalk");
        audioManager.Play("Shave Yourself");


        yield return new WaitForSeconds(animLength);


        animator.speed = 1f;

        animator.enabled = false;
        bossSprite.sprite = finalBossSprite;
        GameObject.Find("Player").GetComponent<PlayerMovement2D>().enabled = true;
        GameObject.Find("Player").GetComponent<PlayerCombat>().enabled = true;
        GameObject.Find("GameManager").GetComponent<EnemySpawner>().enabled = true;
        GameObject.Find("neganick").GetComponent<EnemyAI>().enabled = true;
    }
}
