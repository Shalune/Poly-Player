using UnityEngine;
using System.Collections;

public class GameManager_BR : MonoBehaviour {

	public GameObject gameWorld;
	public GameObject worldPacket0;
	public GameObject worldPacket1;
	public GameManager_Meta gameManager;
	public GameObject terrainBlock;
	public GameObject coin;
	public GameObject multiplier;
	public GameObject player;

	public float chanceOfGap;
	public float chanceOfCoinSpawn;
	public float spawnYOffset;
	public int minMultiplierSpawnDelay;
	public int maxMultiplierSpawnDelay;
	public int drySpellBlocksPit;
	public Vector3 zeroPosition;

	private string gameCode = "BR";
	private float playerXOffset = 22.5f;
	private int level = 1;
	private int lives = GameManager_Meta.startingLives;
	private float speed = 7f;
	private float ratioGravityToSpeed = 1.5f;
	private int maxCoinsPerSpawn = 2;
	private int blocksPerPacket = 5;
	private int blockXOffset = 15;
	private int blockYOffset = 15;
	private int blocksSinceLastPit = 0;
	private int safeStartingArea = 3;
	private int blocksSinceLastMultiplier = 0;
	private int spawnMultiplierAt;
	private bool prevIsPit = false;
	private float endPauseTimer = 0f;
	private bool endPaused = false;

	private Player_BR playerScript;

	void Start () {
		playerScript = player.GetComponent<Player_BR> ();
		Physics2D.gravity = Vector2.down * ratioGravityToSpeed * speed;
		ResetWorld ();
	}

	void Update(){
		if (!gameManager.IsGameOver ()) {
			if (endPaused) {
				endPauseTimer += Time.deltaTime;
				if (endPauseTimer >= GameManager_Meta.miniGameEndPause) {
					ToggleGameFreeze ();
					ResetPlayer ();
					ResetWorld ();
				}

			} else {
				MoveWorld ();
			}
		}
	}

	private void MoveWorld(){
		gameWorld.transform.position += Vector3.left * speed * Time.deltaTime;

		if (worldPacket0.transform.position.x <= zeroPosition.x - blocksPerPacket * blockXOffset) {

			transform.position = zeroPosition;
			worldPacket1.transform.position = zeroPosition;
			worldPacket0.transform.position = zeroPosition + Vector3.right * blocksPerPacket * blockXOffset;
			GenerateWorldPacket (worldPacket0, 0);

		} else if (worldPacket1.transform.position.x <= zeroPosition.x - blocksPerPacket * blockXOffset) {

			transform.position = zeroPosition;
			worldPacket0.transform.position = zeroPosition;
			worldPacket1.transform.position = zeroPosition + Vector3.right * blocksPerPacket * blockXOffset;
			GenerateWorldPacket (worldPacket1, 0);

		}
	}

	private void ResetWorld(){
		spawnMultiplierAt = Random.Range (minMultiplierSpawnDelay, maxMultiplierSpawnDelay+1);

		// modified packet gen to guarantee some floor
		worldPacket0.transform.position = zeroPosition;
		GenerateWorldPacket(worldPacket0, safeStartingArea);

		worldPacket1.transform.position = zeroPosition + Vector3.right * blocksPerPacket * blockXOffset;
		GenerateWorldPacket(worldPacket1, 0);
	}

	private void GenerateWorldPacket(GameObject packet, int nothingBefore){
		ClearWorldPacket (packet);


		float x;
		float y = packet.transform.position.y - blockYOffset;
		float startXInterval = Mathf.Floor (blocksPerPacket / 2f);
		GameObject newBlock;

		for (int i = 0; i < blocksPerPacket; i++) {

			x = packet.transform.position.x + (i - startXInterval) * blockXOffset;

			//int rand = Random.Range(0, 100);

			if (!prevIsPit && i >= nothingBefore &&  
					(Random.Range(0f, 1f) < chanceOfGap || blocksSinceLastPit >= drySpellBlocksPit)) {
				// create pit
				blocksSinceLastPit = 0;
				prevIsPit = true;

			} else {
				// create terrain block
				newBlock = Instantiate (terrainBlock, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
				newBlock.transform.SetParent (packet.transform);
				prevIsPit = false;
				blocksSinceLastPit++;
			}

			PopulateWorldBlock (new Vector3(x, y, 0), packet);
		}


	}

	private void PopulateWorldBlock(Vector3 blockPosition, GameObject packet){
		// if prevIsPit, newBlock = pit

		GameObject newObject;
		int spawnHeightMultiplier = prevIsPit ? 2 : 1 + Random.Range (0, 2);
		Vector3 spawnPosition = blockPosition + Vector3.up * spawnHeightMultiplier * spawnYOffset;

		if (blocksSinceLastMultiplier >= spawnMultiplierAt) {
			// create multiplier
			newObject = Instantiate(multiplier, spawnPosition, Quaternion.identity) as GameObject;
			newObject.transform.SetParent (packet.transform);

			blocksSinceLastMultiplier = 0;
			spawnMultiplierAt = Random.Range (minMultiplierSpawnDelay, maxMultiplierSpawnDelay);

		} else if (Random.Range(0f,1f) <= chanceOfCoinSpawn) {
			int numSpawnCoins = Random.Range (1, maxCoinsPerSpawn);

			newObject = Instantiate(coin, spawnPosition, Quaternion.identity) as GameObject;
			newObject.transform.SetParent (packet.transform);
		}

		blocksSinceLastMultiplier++;
	}

	private void ClearWorldPacket(GameObject packet){
		foreach (Transform child in packet.transform) {
			child.gameObject.SetActive (false);
			Destroy (child.gameObject);
		}
	}

	private void ResetPlayer(){
		player.transform.position = zeroPosition + Vector3.left * playerXOffset;
		player.GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
	}

	private void ToggleGameFreeze(){
		endPaused = !endPaused;
		player.GetComponent<Player_BR>().ToggleFreeze ();

		if (endPauseTimer > 0) {
			endPauseTimer = 0f;
		}
	}

	public float GetSpeed(){
		return speed;
	}

	public void PlayerDied(){
		lives--;
		gameManager.UpdateLives (gameCode, lives, true);
		ToggleGameFreeze ();
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

	public bool PlayerIsGrounded() { return playerScript.IsGrounded (); }
}
