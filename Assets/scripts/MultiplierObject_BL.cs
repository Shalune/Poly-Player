using UnityEngine;
using System.Collections;

public class MultiplierObject_BL : MonoBehaviour {

	public GameManager_BL gameManager;
	public Vector3 velocity;
	public static float speed = 3f;

	void Start () {
		gameManager = GameObject.Find ("minigame BL").GetComponent<GameManager_BL> ();
	}

	void Update(){
		if (!gameManager.gameManager.IsGameOver ()) {
			transform.position += velocity * speed * Time.deltaTime;
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
