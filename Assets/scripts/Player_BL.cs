using UnityEngine;
using System.Collections;

public class Player_BL : MonoBehaviour {

	public GameManager_BL gameManager;

	public float speed;
	public Vector3 velocity;
	public float minX;
	public float maxX;


	private Vector3 lastStep;
	private bool blocked = false;
	private bool freeze = false;
	private Rigidbody2D rb;

	void Start(){
		rb = GetComponent<Rigidbody2D> ();
		velocity = new Vector3 (0, 0, 0);
	}

	void FixedUpdate () {
		if (!gameManager.gameManager.IsGameOver () && !freeze) {
			if (!blocked) {
				// update by input
				if (Input.GetKey ("a")) {
					velocity.x = -speed;
				} else if (Input.GetKey ("d")) {
					velocity.x = speed;
				}

				// apply velocity
				lastStep = velocity * Time.deltaTime;
				transform.position = new Vector3 (Mathf.Clamp (transform.position.x + lastStep.x, minX, maxX), transform.position.y + lastStep.y);

			}
			velocity.x = 0;
			blocked = false;
		}
	}

	void OnCollisionEnter2D(Collision2D other) {

		if (other.gameObject.CompareTag ("Barrier")) {
			transform.position -= lastStep;
			blocked = true;
		}
	}

	public void ToggleFreeze(){
		freeze = !freeze;
	}
}
