using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MovementController requires the GameObject to have a Ridgidbody2D component.
[RequireComponent( typeof(Rigidbody2D) )]

public class PlayerController : MonoBehaviour {

    public float acceleration;
    public GameObject arrow;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 direction;

    // Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        direction = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
        // Set the direction based on user input
        direction = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) direction += new Vector2( 0,  1);
        if (Input.GetKey(KeyCode.S)) direction += new Vector2( 0, -1);
        if (Input.GetKey(KeyCode.A)) direction += new Vector2(-1,  0);
        if (Input.GetKey(KeyCode.D)) direction += new Vector2( 1,  0);
        direction.Normalize();

        // Shoot arrow
        if (Input.GetMouseButtonDown(0)) {
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dir = worldMousePosition - transform.position;
            dir.z = 0;
            dir.Normalize();

            Instantiate(arrow, transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90));
        }

        // Set the facing
        if (direction.x < 0) spriteRenderer.flipX = true;
        if (direction.x > 0) spriteRenderer.flipX = false;

        // Set the animation
        animator.SetBool("isWalking", direction.magnitude > 0);
        
	}

    // FixedUpdate is called once per fixed frame
    void FixedUpdate() {
        body.AddForce(direction * acceleration, ForceMode2D.Impulse);
    }
}
