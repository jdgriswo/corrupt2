using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blinkMovement : MonoBehaviour {
    public float speed;
    private Vector2 blinkPos;
    private Rigidbody2D rb2de;
    private Rigidbody2D target;
    private bool state;
    private int blinkcount;
	// Use this for initialization
	void Start () {
        target = PlayerStatus.player;
        rb2de = GetComponent<Rigidbody2D>();
        blinkcount = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        blinkPos = transform.position;
        if (Mathf.Abs(target.position.magnitude - rb2de.position.magnitude) >= 0.5) { state = false; }
        if (state == false)
        {
            Vector2 diff = rb2de.position - target.position;
            rb2de.velocity = -diff * (speed/diff.magnitude);
            blinkcount = blinkcount + 1;
            if (blinkcount == 50)
            {
                blinkcount = 0;
                rb2de.position = rb2de.position + (target.position - rb2de.position) / 2;
            }
        } 
        if (state == true) { rb2de.velocity = Vector2.zero; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            state = true;
        }
    }
}
