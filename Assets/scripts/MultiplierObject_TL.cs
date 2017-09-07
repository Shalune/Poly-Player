using UnityEngine;
using System.Collections;

public class MultiplierObject_TL : MonoBehaviour {

	public GameManager_TL gameManager;
	public Vector3 velocity;
	public static float speed = 3f;

	Rigidbody2D rb;

	void Start () {
		gameManager = GameObject.Find ("minigame TL").GetComponent<GameManager_TL> ();

		rb = GetComponent<Rigidbody2D> ();
		rb.AddForce (velocity * speed);
	}

	void Update(){
		if (gameManager.gameManager.IsGameOver ()) {
			rb.velocity = Vector3.zero;
		}
	}

	void OnCollisionEnter2D(Collision2D other){
		if(other.gameObject.CompareTag("Player")){
			gameManager.ObtainedMultiplier ();
			KillThis();
		}
	}

	void OnTriggerEnder2D(Collision2D other){
		if(other.gameObject.CompareTag("Cleaner")){
			KillThis ();
		}
	}

	void KillThis(){
		gameObject.SetActive (false);
		Destroy (this);
	}
}
