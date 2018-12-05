using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour {

    public float bulletVelocity = 20f;
    public GameObject bulletPrefab;
    public GameObject bullet2Prefab;
    public GameObject bullet3Prefab;
    public GameObject shootSource;
    private float nextTimeToFire = 0f;
    public AudioManager audioManager;

    private Rigidbody2D rb;

    public static int bulletLevel = 1;
	// Use this for initialization
	void Start () {
		
	}
	
    void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	// Update is called once per frame
	void Update () {
        Vector2 direction = Vector2.zero;

        bool shoot = false;

        if (Input.GetButton("Fire1"))
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction += (Vector2)((worldMousePos - shootSource.transform.position));
            shoot = true;            
        }            

        if(Input.GetKey(KeyCode.UpArrow))
        {
            direction += Vector2.up;
            PlayerMovement2D.lookAtMouse = false;
            shoot = true;
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            direction += Vector2.down;
            PlayerMovement2D.lookAtMouse = false;
            shoot = true;
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            direction += Vector2.left;
            PlayerMovement2D.lookAtMouse = false;
            shoot = true;
        }
        
        if(Input.GetKey(KeyCode.RightArrow))
        {
            direction += Vector2.right;
            PlayerMovement2D.lookAtMouse = false;
            shoot = true;
        }
        

        direction.Normalize();
        if(shoot && direction != Vector2.zero){
            if(!PlayerMovement2D.lookAtMouse)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                // Inverse so my chin points at the mouse
                rb.rotation =  angle + 180f;
            }
            Shoot(direction);
        }
	}

    void Shoot(Vector2 direction)
    {
        if (Time.time < nextTimeToFire)
        {
            return;
        }
        
        float fireDelay = bulletLevel > 4 ? .1f : .175f;

        GameObject bulletToInstantiate = bulletLevel > 1 ? bulletLevel > 3 ? bullet3Prefab : bullet2Prefab : bulletPrefab;
        // Creates the bullet locally
        GameObject bullet = (GameObject)Instantiate(
                                bulletToInstantiate,
                                shootSource.transform.position /* (Vector3)(direction * 0.5f)*/,
                                Quaternion.identity);
                                
        audioManager.Play("Shoot");
        // Adds velocity to the bullet
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletVelocity;

        if(bulletLevel > 2)
        {
            GameObject bullet2 = (GameObject)Instantiate(
                                bulletToInstantiate,
                                shootSource.transform.position /* (Vector3)(direction * 0.5f)*/,
                                Quaternion.identity);
            audioManager.Play("Shoot");
            bullet2.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 20) * direction * bulletVelocity;
            GameObject bullet3 = (GameObject)Instantiate(
                                bulletToInstantiate,
                                shootSource.transform.position /* (Vector3)(direction * 0.5f)*/,
                                Quaternion.identity);
            audioManager.Play("Shoot");
            bullet3.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, -20) * direction * bulletVelocity;
        }

        nextTimeToFire = Time.time + fireDelay;
    }

    void Reset()
    {
        bulletVelocity = 20f;
        //bulletPrefab = (GameObject)Resources.Load("../Prefabs/bullet", typeof(GameObject));
    }
}
