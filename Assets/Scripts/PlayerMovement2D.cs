using UnityEngine;

public class PlayerMovement2D : MonoBehaviour {

    public CharacterController2D controller;
    float horizontalMove = 0f;
    float verticalMove = 0f;
    public float speed = 55f;

    public static bool lookAtMouse = true;

    public static Vector2 Position;
    Vector2 mousePos = Vector2.zero;

    public Rigidbody2D rb;

	// Use this for initialization
	void Awake () {
         rb = GetComponent<Rigidbody2D>();
	}

     void OnDisable()
    {
        horizontalMove = 0f;
        verticalMove = 0f;
        rb.velocity = Vector2.zero;
    }
	
	// Update is called once per frame
	void Update () {
		horizontalMove = Input.GetAxisRaw("Horizontal") * 40f;
        verticalMove = Input.GetAxisRaw("Vertical") * 40f;

        if(HasMouseMoved())
        {
            lookAtMouse = true;
        }

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, false);
        controller.VerticalMove(verticalMove * Time.fixedDeltaTime);

        if(lookAtMouse)
        {
            Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            // Inverse so my chin points at the mouse
            rb.rotation =  angle + 180f;
        }

		Position = rb.position;
    }

    bool HasMouseMoved()
     {
         //I feel dirty even doing this 
         return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
     }
}
