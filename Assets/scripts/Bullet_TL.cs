using UnityEngine;
using System.Collections;

public class Bullet_TL : MonoBehaviour {

	public GameManager_TL gameManager;
	public bool belongsToEnemy;
	public Vector3 velocity;
	public static float speed = 3f;

	private bool freeze = false;

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
		if(belongsToEnemy && other.gameObject.CompareTag("Player")){
			
			KillBullet();
			gameManager.KillPlayer ();

		} else if (!belongsToEnemy && other.gameObject.CompareTag("Enemy")){
			KillBullet();
			gameManager.KillEnemy (other.gameObject);
		}
	}

	void OnTriggerEnder2D(Collision2D other){
		if(other.gameObject.CompareTag("Cleaner")){
			KillBullet ();
		}
	}

	void KillBullet(){
		gameObject.SetActive (false);
		Destroy (this);
	}

	public void ToggleFreeze(){
		freeze = !freeze;
	}
}
