using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ball_BL : MonoBehaviour {

	public float playerBounceAngling;
	public Vector2 startPosition;

	private GameManager_BL gameManager;
	private float speed = 3f;
	private Rigidbody2D rb;

	void Start(){
		gameManager = GameObject.Find ("minigame BL").GetComponent<GameManager_BL>();
		rb = GetComponent<Rigidbody2D> ();
		InitBall ();
	}

	public void InitBall(){
		float x = Random.Range (0f, 0.5f);
		float y = 1f - x;

		Vector2 velocity = new Vector2 (x, y).normalized;
		rb.velocity = velocity * speed;
		//rb.AddForce (velocity*speed);


		transform.position = new Vector3 (startPosition.x, startPosition.y, 0);
	}

	void Update(){
		if (gameManager.gameManager.IsGameOver ()) {
			rb.velocity = Vector3.up * 0;
		}
	}


	void OnTriggerEnter2D(Collider2D other) {
		
		if (other.gameObject.CompareTag ("Cleaner")) {
			// score and alerts
			gameManager.BallOut();
			return;
		}
	}


	void OnCollisionEnter2D (Collision2D other){
		if(other.gameObject.CompareTag("Breakout Block")){
			gameManager.KillBlock(other.gameObject);

		} else if (other.gameObject.CompareTag ("Player")) {
			float difference = transform.position.x - other.transform.position.x;
			Vector2 velocity = rb.velocity;
			velocity.x += difference * playerBounceAngling;
			float currentMag = rb.velocity.magnitude;
			rb.velocity = velocity.normalized * currentMag;

		}

		// make sure does not lose Y velocity on bounce
		if (Mathf.Abs (rb.velocity.x) > Mathf.Abs (rb.velocity.y) * 5) {
			
			Vector2 newV = new Vector2 (rb.velocity.x, Mathf.Abs(rb.velocity.x) / 4).normalized;
			//Debug.Log ("normalized vector? = " + newV);
			newV *= speed;
			//Debug.Log ("New v = (" + newV.x + ", " + newV.y + ")");
			if (other.transform.position.y - transform.position.y > 0) {
				rb.velocity = newV;
			} else {
				rb.velocity = newV;
			}

		}
	}

	public void IncrementSpeed(float increase){
		speed += increase;
	}
}
