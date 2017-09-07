using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager_TL : MonoBehaviour {

	public GameObject enemyField;
	public GameObject enemyRow0;
	public GameObject enemyRow1;
	public GameObject enemyRow2;
	public GameObject enemyDefault;
	public GameObject multiplier;
	public GameObject bulletEnemy;
	public GameObject bulletPlayer;
	public GameManager_Meta gameManager;
	public GameManager_BR managerBR;
	public GameObject player;
	public GameObject gamePieces;

	public Vector2 enemyZeroPosition;
	public int numRows;
	public float enemyStartX;
	public float enemyDistance;
	public float rowDistance;
	public int enemiesPerRow;
	public Vector3 enemyVelocity;
	public float enemyMovePeriod;
	public float enemyMoveDistance;
	public int enemyMovesPerShoot;

	private string gameCode = "TL";
	private int numEnemies;
	private int level = 1;
	private int lives = GameManager_Meta.startingLives;
	private float speedMultiplier = 0.5f;
	private float enemyMoveTimer = 0f;
	private float bulletOffsetPlayer;
	private float bulletOffsetEnemy;

	private float enemyShootTimer = 0f;

	private int multiplierTimer = 0;
	private int ratioBulletsToMultipliers = 1;
	private bool spawnMultiplierNextMove = false;

	private float endPauseTimer = 0f;
	private bool endPaused = false;

	private List <GameObject> bullets;
	private List <GameObject> enemies;


	private Dictionary<int, GameObject> enemyRows;

	void Start(){
		//enemyRow1 = new GameObject[enemiesPerRow];
		enemyRows = new Dictionary<int, GameObject> () {
			{0, enemyRow0},
			{1, enemyRow1},
			{2, enemyRow2}
		};

		enemies = new List <GameObject> ();
		bullets = new List <GameObject> ();
		numEnemies = numRows * enemiesPerRow;
		Vector3 offset = new Vector3 (0, rowDistance, 0);
		enemyRow0.transform.position -= offset;
		enemyRow2.transform.position += offset;
		bulletOffsetPlayer = player.transform.lossyScale.x*3f;
		bulletOffsetEnemy = bulletOffsetPlayer / 2f;
		ResetLevel ();
	}

	void Update(){
		if (!gameManager.IsGameOver ()) {
			
			if (endPaused) {
				endPauseTimer += Time.deltaTime;
				if (endPauseTimer >= GameManager_Meta.miniGameEndPause) {
					ResetLevel ();
					ToggleGameFreeze ();
				}

			} else {
				RunGame ();
			}
		}
	}

	private void RunGame(){
		if (enemyMoveTimer >= enemyMovePeriod) {
			enemyField.transform.position -= new Vector3 (enemyMoveDistance, 0, 0);
			enemyMoveTimer = 0;
			enemyShootTimer++;

			if (spawnMultiplierNextMove) {
				SpawnMultiplier ();
				multiplierTimer = 0;
				spawnMultiplierNextMove = false;
			}

			if (enemyShootTimer >= enemyMovesPerShoot) {
				EnemyShoots ();
				enemyShootTimer = 0;

				multiplierTimer++;

				if (multiplierTimer >= ratioBulletsToMultipliers) {
					spawnMultiplierNextMove = true;
				}
			}
		}
		enemyMoveTimer += Time.deltaTime * speedMultiplier;
	}

	void AdvanceLevel(){

	}

	void ResetLevel(){
		ClearScreen ();

		spawnMultiplierNextMove = false;

		// reset rows
		enemyField.transform.position = enemyZeroPosition;
		GameObject enRow = null;

		for (int i = 0; i < numRows; i++) {
			
			if (enemyRows.TryGetValue (i, out enRow)) {
				enRow.transform.position = new Vector3 (enemyZeroPosition.x, enRow.transform.position.y, 0);
			} else {
				Debug.Log ("ERROR: failed to retrieve enemy row in ResetLevel()");
			}
		}


		for(int i=0; i<numEnemies; i++){
			int row = (int)Mathf.Floor (i / enemiesPerRow);
			float x = enemyZeroPosition.x + enemyStartX + enemyDistance * (i - row * enemiesPerRow); 
			float y = enemyZeroPosition.y + rowDistance * (row - 1);
			NewEnemy (x, y, row);
		}
	}

	void ClearScreen(){
		foreach (GameObject enemyToKill in enemies) {
			enemyToKill.SetActive (false);
			Destroy (enemyToKill);
		}
		enemies.Clear ();

		foreach (GameObject bulletToKill in bullets) {
			bulletToKill.SetActive (false);
			Destroy (bulletToKill);
		}
		bullets.Clear ();
	}

	void SpawnMultiplier(){
		int row = Random.Range (0, 3);
		GameObject multiplierRow = null;

		if (enemyRows.TryGetValue (row, out multiplierRow)) {
			GameObject newMultiplier = Instantiate (multiplier, multiplierRow.transform.position + Vector3.left * bulletOffsetEnemy, Quaternion.identity) as GameObject;
			newMultiplier.transform.SetParent (gamePieces.transform);
			bullets.Add (newMultiplier);
		} else {
			Debug.Log ("ERROR: failed to find row to shoot from");
		}
	}

	void NewEnemy(float x, float y, int row){
		GameObject newEnemy = Instantiate (enemyDefault, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
		GameObject addToRow = null;

		if (enemyRows.TryGetValue (row, out addToRow)) {
			newEnemy.transform.SetParent(addToRow.transform);
		} else {
			Debug.Log ("ERROR: failed to find row to add generated enemy to");
		}
		enemies.Add (newEnemy);
	}

	void EnemyShoots(){
		int row = Random.Range (0, 3);
		GameObject shootingRow = null;

		if (enemyRows.TryGetValue (row, out shootingRow)) {
			GameObject newBullet = Instantiate (bulletEnemy, shootingRow.transform.position + Vector3.left * bulletOffsetEnemy, Quaternion.identity) as GameObject;
			//newBullet.GetComponent<Bullet_TL> ().velocity *= speedMultiplier;
			newBullet.transform.SetParent (gamePieces.transform);
			bullets.Add (newBullet);
		} else {
			Debug.Log ("ERROR: failed to find row to shoot from");
		}
	}

	private void ToggleGameFreeze(){
		endPaused = !endPaused;
		player.GetComponent<Player_TL>().ToggleFreeze ();

		foreach (GameObject thing in bullets) {
			thing.GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
		}

		if (endPauseTimer > 0) {
			endPauseTimer = 0f;
		}
	}

	public void KillEnemy(GameObject enemy){
		int points = enemy.GetComponent<ScoringObject>().points;
		gameManager.ScorePoints (points);

		if (enemy.transform.position.y == enemyZeroPosition.y - rowDistance) {
			enemyRow0.transform.position += new Vector3 (enemyDistance, 0, 0);

		} else if (enemy.transform.position.y == enemyZeroPosition.y) {
			enemyRow1.transform.position += new Vector3 (enemyDistance, 0, 0);
		} else {
			enemyRow2.transform.position += new Vector3 (enemyDistance, 0, 0);
		}
	}

	public void KillPlayer(){
		lives--;
		gameManager.UpdateLives (gameCode, lives, true);
		ToggleGameFreeze ();
	}

	public void PlayerShoots(){
		if(managerBR.PlayerIsGrounded()){
			GameObject newBullet = Instantiate (bulletPlayer, player.transform.position + bulletOffsetPlayer * Vector3.right, Quaternion.identity) as GameObject;
			newBullet.GetComponent<Bullet_TL> ().velocity *= speedMultiplier;
			newBullet.transform.SetParent (gamePieces.transform);
			bullets.Add (newBullet);
		}
	}

	public void ObtainedMultiplier(){
		gameManager.ObtainedMultiplier ();
	}

	public int GetLives(){
		return lives;
	}

	// ODOT - move lives to Meta?
	public void GainLife(){
		lives++;
	}
}
