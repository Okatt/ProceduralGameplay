using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {

    public float acceleration;
    
    private Rigidbody2D body;
    private Vector2 direction; 
  
    // Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
        direction = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
        direction = transform.rotation * Vector2.up;
	}

    // FixedUpdate is called once per fixed frame
    void FixedUpdate() {
        body.AddForce(direction * acceleration, ForceMode2D.Impulse);
    }
}
