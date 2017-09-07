using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager_TR : MonoBehaviour {

	public GameManager_Meta gameManager;
	public GameObject piecesField;
	public GameObject boardSlot;
	public Vector3 zeroPosition;
	public Sprite playerSprite;
	public Sprite AISprite;
	public Sprite multiplierSprite;
	public int pointsPerWin;
	public int pointsPerRound;
	public int roundEndOnNumWins;

	public SpriteRenderer[] gameScoreUI;
	public Sprite[] scoreSprites;

	private string gameCode = "TR";
	private int level = 1;
	private int lives = GameManager_Meta.startingLives;
	private int rows = 3;
	private int slotsPerRow = 3;
	private int boardSize;
	private float slotOffset = 10f;
	private _slot[] boardModel;
	private GameObject[] boardObjects;
	private bool isPlayerTurn = false;
	private List <int> openSlots;
	private int[] totalWins;

	private int multiplierTimer = 0;
	private int spawnMultiplierAt;
	private int minMultiplierDelay = 5;
	private int maxMultiplierDelay = 10;

	private float endPauseTimer = 0f;
	private bool endPaused = false;

	private enum _slot {none = -1, player, AI, multiplier};

	void Start () {
		boardSize = rows * slotsPerRow;
		boardModel = new _slot[boardSize];
		boardObjects = new GameObject[boardSize];
		openSlots = new List <int> ();
		totalWins = new int[2];
		totalWins [0] = 0;
		totalWins [1] = 0;
		spawnMultiplierAt = Random.Range (minMultiplierDelay, maxMultiplierDelay);


		Vector3 newPos;

		for (int i = 0; i < boardSize; i++) {
			newPos = zeroPosition + Vector3.right * (i - (Mathf.Floor(i/slotsPerRow) * slotsPerRow) - Mathf.Floor(slotsPerRow/2)) * slotOffset
				+ Vector3.up * (Mathf.Floor(-i/slotsPerRow) + Mathf.Floor(rows/2)) * slotOffset;

			boardModel [i] = _slot.none;
			boardObjects [i] = Instantiate (boardSlot, newPos, Quaternion.identity) as GameObject;
			boardObjects [i].transform.SetParent (piecesField.transform);
			openSlots.Add (i);
		}

	}

	private void ResetBoard(){
		isPlayerTurn = false;
		openSlots.Clear ();

		for (int i = 0; i < boardSize; i++) {
			openSlots.Add (i);
			boardModel [i] = _slot.none;
			boardObjects[i].GetComponent<SpriteRenderer>().sprite = null;
		}
	}

	private void NewRound(_slot winner){
		totalWins [0] = 0;
		totalWins [1] = 0;


		if (winner == _slot.player) {
			gameManager.ScorePoints (pointsPerRound);
			lives++;
			gameManager.UpdateLives (gameCode, lives, false);
		}
	}

	void Update () {
		if (!gameManager.IsGameOver ()) {

			if (endPaused) {
				endPauseTimer += Time.deltaTime;
				if (endPauseTimer >= GameManager_Meta.miniGameEndPause) {
					ResetBoard ();
					endPauseTimer = 0f;
					endPaused = false;
				}

			} else {
				if (isPlayerTurn) {
					PlayerTurn ();
				} else {
					AITurn ();
				}
			}
		}
	}

	private void PlayerTurn(){

		if (Input.GetKeyDown (KeyCode.Keypad7) && openSlots.Contains(0)) {
			PlayerMove (0);
		} else if (Input.GetKeyDown (KeyCode.Keypad8) && openSlots.Contains(1)) {
			PlayerMove (1);
		} else if (Input.GetKeyDown (KeyCode.Keypad9) && openSlots.Contains(2)) {
			PlayerMove (2);
		} else if (Input.GetKeyDown (KeyCode.Keypad4) && openSlots.Contains(3)) {
			PlayerMove (3);
		} else if (Input.GetKeyDown (KeyCode.Keypad5) && openSlots.Contains(4)) {
			PlayerMove (4);
		} else if (Input.GetKeyDown (KeyCode.Keypad6) && openSlots.Contains(5)) {
			PlayerMove (5);
		} else if (Input.GetKeyDown (KeyCode.Keypad1) && openSlots.Contains(6)) {
			PlayerMove (6);
		} else if (Input.GetKeyDown (KeyCode.Keypad2) && openSlots.Contains(7)) {
			PlayerMove (7);
		} else if (Input.GetKeyDown (KeyCode.Keypad3) && openSlots.Contains(8)) {
			PlayerMove (8);
		}
	}

	private void PlayerMove(int selection){

		if (boardModel [selection] == _slot.multiplier) {
			gameManager.ObtainedMultiplier ();
		}

		PlaceMove (selection, _slot.player);

		if (GameIsWon (_slot.player)) {
			GameOver (_slot.player);
		} else if (openSlots.Count == 0) {
			GameOver (_slot.none);
		} else {
			isPlayerTurn = false;
		}
	}

	private void AITurn(){
		int selection = -1;

		while (!openSlots.Contains (selection) && openSlots.Count > 0) {
			selection = Random.Range (0, boardSize);
		}
		if (selection != -1) {
			PlaceMove (selection, _slot.AI);
		}

		if (GameIsWon (_slot.AI)) {
			GameOver (_slot.AI);
		} else if (openSlots.Count == 0) {
			GameOver (_slot.none);
		} else {
			isPlayerTurn = true;
		}

		GenerateMultiplier ();
	}

	private void GenerateMultiplier(){

		Debug.Log ("checking for multiplier");

		if (multiplierTimer >= spawnMultiplierAt && openSlots.Count > 0) {
			Debug.Log ("generating multiplier");

			int multiplierSlot = -1;

			while (!openSlots.Contains (multiplierSlot) && openSlots.Count > 0) {
				multiplierSlot = Random.Range (0, boardSize);
			}

			boardModel [multiplierSlot] = _slot.multiplier;
			boardObjects [multiplierSlot].GetComponent<SpriteRenderer> ().sprite = multiplierSprite;

			// reset for next round
			spawnMultiplierAt = Random.Range (minMultiplierDelay, maxMultiplierDelay);
			multiplierTimer = 0;
		}

		multiplierTimer ++;
		Debug.Log ("multipliertimer = " + multiplierTimer);
	}

	private bool GameIsWon(_slot placer){
		for (int i = 0; i < boardSize; i++) {

			if (boardModel [i] == placer) {
				
				if (i == 0) {
					if (CheckHorizontal (i, placer) || CheckVertical (i, placer) || CheckDiagonal (i, placer)) {
						return true;
					}

				} else if (i == slotsPerRow - 1) {
					if (CheckVertical (i, placer) || CheckDiagonal (i, placer)) {
						return true;
					}

				} else if (i < slotsPerRow) {
					if (CheckVertical (i, placer)) {
						return true;
					}

				} else if (i - Mathf.Floor (i / slotsPerRow) * slotsPerRow == 0) {
					if (CheckHorizontal (i, placer)) {
						return true;
					}

				}
			}
		}
		return false;
	}

	private bool CheckHorizontal(int slot, _slot placer){
		for (int i = 0; i < rows; i++) {
			if (boardModel [slot + i] != placer) {
				return false;
			}
		}
		return true;
	}

	private bool CheckVertical(int slot, _slot placer){
		for (int i = 0; i < rows; i++) {
			if (boardModel [slot + i * slotsPerRow] != placer) {
				return false;
			}
		}
		return true;
	}

	private bool CheckDiagonal(int slot, _slot placer){

		if (slot == 0) {
			for (int i = 0; i < rows; i++) {
				if (boardModel [slot + i + i * slotsPerRow] != placer) {
					return false;
				}
			}
			return true;

		} else if (slot == slotsPerRow - 1) {
			for (int i = 0; i < rows; i++) {
				if (boardModel [slot - i + i * slotsPerRow] != placer) {
					return false;
				}
			}
			return true;

		} else {
			Debug.Log ("ERROR: TR CheckDiagonal() was passed invalid starting slot");
			return false;
		}

		return false;
	}

	private void PlaceMove(int slot, _slot placer){

		boardModel [slot] = placer;
		openSlots.Remove (slot);

		if (placer == _slot.AI) {
			boardObjects [slot].GetComponent<SpriteRenderer> ().sprite = AISprite;
		} else if (placer == _slot.player) {
			boardObjects [slot].GetComponent<SpriteRenderer> ().sprite = playerSprite;
		} else {
			Debug.Log ("ERROR: game TR received invalid _slot value in PlaceMove()");
		}
	}

	private void GameOver(_slot winner){
		endPaused = true;

		if (winner == _slot.player) {
			gameManager.ScorePoints (pointsPerWin);
		} else if (winner == _slot.AI) {
			lives--;
			gameManager.UpdateLives (gameCode, lives, true);
		}

		if (winner != _slot.none) {
			totalWins [(int)winner] += 1;

			if (totalWins [(int)winner] >= roundEndOnNumWins) {
				NewRound (winner);
			}
		}

		for (int i = 0; i < 2; i++) {
			gameScoreUI [i].sprite = scoreSprites [totalWins [i]];
		}
	}

	public int GetLives(){
		return lives;
	}

	// ODOT - move lives to Meta?
	public void GainLife(){
		lives++;
	}
}
