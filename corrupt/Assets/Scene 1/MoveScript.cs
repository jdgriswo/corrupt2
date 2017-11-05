using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour {

    public float speed; //store player's movement speed

    private Rigidbody2D rb2d; //reference to Rigidbody2D component

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        PlayerStatus.player = rb2d;
	}
	
	// Update is called at a fixed interval
	void FixedUpdate () {
        float moveHoriz = Input.GetAxis("Horizontal");
        float moveVert = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHoriz, moveVert);
        rb2d.velocity = movement * speed;

	}
}
