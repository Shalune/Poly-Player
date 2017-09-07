using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager_Meta : MonoBehaviour {

	public GameManager_TL gameTL;
	public GameManager_TR gameTR;
	public GameManager_BL gameBL;
	public GameManager_BR gameBR;

	public Text livesTextTL;
	public Text livesTextBL;
	public Text livesTextTR;
	public Text livesTextBR;
	public Text scoreText;
	public Text limitedMultiplierText;
	public Text globalMultiplierText;
	public string scoreTextHeader = "SCORE POINTS : ";
	public string multiplierTextHeader = "x";

	public const int startingLives = 1;
	public const float miniGameEndPause = 3f;
	public static List<string> gameLayers;

	private int score = 0;
	private int scoreMagnitude = 2;
	private int scoreGivenThisSecond = 0;
	private float passiveScoreRate = 0.1f;
	private float globalScoreMultiplier = 1f;
	private int limitedScoreMultiplier = 1;
	private int resetScoreMultiplierTo = 1;
	private float incrementGlobalMultiplierAfterStage = 1f;
	private float multiplierBankRatio = 0.25f;
	private int multiplierTier = 0;
	private int[] multiplierTiers = { 1, 2, 5, 12, 25, 50, 100 };
	private float scoreTimer = 0f;
	private bool gameOver = false;

	private Dictionary<string, GameObject> miniGames;

	void Start(){
		GameManager_Meta.gameLayers = new List<string>();

		int i = 0;

		while (i<8){
			GameManager_Meta.gameLayers.Add (LayerMask.LayerToName (i));
			i++;
		}
			
		while (LayerMask.LayerToName (i) != "") {
			GameManager_Meta.gameLayers.Add (LayerMask.LayerToName (i));
			i++;
		}
	}

	public void Update(){

		if (!gameOver) {
			PassiveScoreGain ();
			scoreText.text = scoreTextHeader + score;
			CheckLifeGain ();
		}
	}

	private void PassiveScoreGain(){
		if (scoreTimer >= passiveScoreRate * scoreGivenThisSecond) {

			score += 1 + (int)(globalScoreMultiplier * limitedScoreMultiplier);
			scoreGivenThisSecond++;
		}
		if (scoreTimer >= 1f) {
			scoreTimer = 0f;
			scoreGivenThisSecond = 0;
		}

		scoreTimer += Time.deltaTime;
	}

	private void CheckLifeGain(){
		if (score >= Mathf.Pow (10, scoreMagnitude)) {
			gameTL.GainLife ();
			gameTR.GainLife ();
			gameBL.GainLife ();
			gameBR.GainLife ();
			livesTextTL.text = gameTL.GetLives().ToString ();
			livesTextTR.text = gameTR.GetLives().ToString ();
			livesTextBL.text = gameBL.GetLives().ToString ();
			livesTextBR.text = gameBR.GetLives().ToString ();

			scoreMagnitude++;
		}
	}

	public void ScorePoints(int points){
		
		score += (int)(points * globalScoreMultiplier * limitedScoreMultiplier);
		scoreText.text = scoreTextHeader + score;
	}

	public void UpdateLives(string game, int newLives, bool lostMultiplier){
		if (lostMultiplier) {
			LostMultiplier ();
		} else {
			// TEMP ODOT - remove and change if global life gain kept
			return;
		}

		switch (game) {
		case "TL":
			livesTextTL.text = newLives.ToString ();
			CheckLives ();
			break;
		case "BL":
			livesTextBL.text = newLives.ToString ();
			CheckLives ();
			break;
		case "TR":
			livesTextTR.text = newLives.ToString ();
			CheckLives ();
			break;
		case "BR":
			livesTextBR.text = newLives.ToString ();
			CheckLives ();
			break;
		}
	}

	private void LostMultiplier(){
		globalScoreMultiplier += limitedScoreMultiplier * multiplierBankRatio;
		limitedScoreMultiplier = resetScoreMultiplierTo;
		multiplierTier = 0;

		// special effects?

		UpdateMultiplierUI ();
	}

	private void UpdateMultiplierUI(){
		//multiplierText.text = multiplierTextHeader + (int)(limitedScoreMultiplier * globalScoreMultiplier);
		limitedMultiplierText.text = multiplierTextHeader + limitedScoreMultiplier.ToString();
		globalMultiplierText.text = multiplierTextHeader + globalScoreMultiplier.ToString("0.0");
	}

	private void CheckLives(){
		if(gameTL.GetLives() <= 0 || gameTR.GetLives() <= 0 || gameBL.GetLives() <= 0 || gameBR.GetLives() <= 0){
			GameOver ();
		}
	}

	private void GameOver(){
		gameOver = true;
	}

	// ODOT - implement or remove - banking multiplier may be better
	public void StageComplete(){
		globalScoreMultiplier += incrementGlobalMultiplierAfterStage;
		UpdateMultiplierUI ();
	}

	public void ObtainedMultiplier(){
		limitedScoreMultiplier++;

		while(limitedScoreMultiplier >= multiplierTiers[multiplierTier+1]){
			multiplierTier++;
		}

		UpdateMultiplierUI ();
	}

	public bool IsGameOver(){
		return gameOver;
	}
}
