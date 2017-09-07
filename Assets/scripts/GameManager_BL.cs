using UnityEngine;
using System.Collections;

public class GameManager_BL : MonoBehaviour {

	public GameObject blockField;
	public GameObject blockDefault;
	public GameObject multiplier;
	public GameObject player;
	public Ball_BL ball;
	public GameManager_Meta gameManager;

	public float levelSpeedFactor;
	public int[] spawnMultipliersFrom;

	private string gameCode = "BL";
	private int numBlocks = 16;
	private int blocksPerRow = 4;
	//private float blockOffsetX = 1.1f;
	//private float blockOffsetY = 0.35f;
	private float blockOffsetX = 11f;
	private float blockOffsetY = 3.5f;
	private int blockCounter;
	private int multipliersSpawned = 0;
	private int level = 1;
	private int lives = GameManager_Meta.startingLives;
	private GameObject[] blocks;

	private float endPauseTimer = 0f;
	private bool endPaused = false;

	void Start(){
		blocks = new GameObject[numBlocks];
		ResetLevel ();
	}

	void Update(){
		if (!gameManager.IsGameOver ()) {

			if (endPaused) {
				endPauseTimer += Time.deltaTime;
				if (endPauseTimer >= GameManager_Meta.miniGameEndPause) {
					ToggleGameFreeze ();
				}

			} else {
				if (blockCounter == 0) {
					ToggleGameFreeze ();
					AdvanceLevel ();
					ResetLevel ();
				}
			}
		}
	}

	private void AdvanceLevel(){
		ball.IncrementSpeed (
			Mathf.Pow(levelSpeedFactor, Mathf.Floor((level+1)/2))
		);
		level++;
	}

	private void ResetLevel(){
		int row;

		for (int i = 0; i < numBlocks; i++) {
			
			Vector3 coord = transform.position;
			row = (int)Mathf.Floor (i / blocksPerRow);
			coord.x += (float)(i - 0.5 * (blocksPerRow - 1) - row * blocksPerRow) * blockOffsetX;
			coord.y += blockOffsetY * row;
			blocks[i] = Instantiate (blockDefault, coord, Quaternion.identity) as GameObject;
			blocks[i].transform.SetParent (blockField.transform);
			blocks [i].name = i.ToString ();
		}
		multipliersSpawned = 0;
		blockCounter = numBlocks;
	}

	private void SpawnMultiplier(GameObject spawnFrom){
		GameObject newMultiplier = Instantiate (multiplier, spawnFrom.transform.position + Vector3.down * blockOffsetY / 2, Quaternion.identity) as GameObject;
		newMultiplier.transform.SetParent (blockField.transform);
	}

	private void ToggleGameFreeze(){
		endPaused = !endPaused;
		player.GetComponent<Player_BL>().ToggleFreeze ();
		ball.InitBall ();
		if (endPaused) {
			ball.GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
		}

		if (endPauseTimer > 0) {
			endPauseTimer = 0f;
		}
	}

	public void BallOut(){
		lives--;
		gameManager.UpdateLives (gameCode, lives, true);
		ToggleGameFreeze ();
	}

	public void KillBlock(GameObject block){
		gameManager.ScorePoints (block.GetComponent<ScoringObject>().points);
		int blockNum = int.Parse(block.name);

		for (int i = 0; i < spawnMultipliersFrom.Length; i++) {
			if (spawnMultipliersFrom [i] == blockNum) {
				SpawnMultiplier (block);
			}
		}

		block.SetActive(false);
		Destroy (block);
		blockCounter--;
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
