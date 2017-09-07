using UnityEngine;
using System.Collections;

public class Player_TL : MonoBehaviour {

	public GameManager_TL gameManager;
	public float speed;

	private bool freeze = false;
	
	void Start(){
		//velocity = new Vector3 (0, 0, 0);
	}
	
	void Update () {
		if (!gameManager.gameManager.IsGameOver () && !freeze) {
			// update by input
			if (transform.position.y < gameManager.enemyZeroPosition.y + gameManager.rowDistance && Input.GetKeyDown ("w")) {
				//velocity.y = speed;
				transform.position += new Vector3 (0, gameManager.rowDistance, 0);
			} 
			if (transform.position.y > gameManager.enemyZeroPosition.y - gameManager.rowDistance & Input.GetKeyDown ("s")) {
				//velocity.y = -speed;
				transform.position -= new Vector3 (0, gameManager.rowDistance, 0);
			}
			if (Input.GetKeyDown ("space")) {
				gameManager.PlayerShoots ();
			}
		}
	}

	public void ToggleFreeze(){
		freeze = !freeze;
	}
}
