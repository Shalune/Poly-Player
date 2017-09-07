using UnityEngine;
using System.Collections;

public class Player_BR : MonoBehaviour {

	public GameManager_BR gameManager;

	private Rigidbody2D rb;
	private BoxCollider2D boxCollider;
	private bool onGround = false;
	private int groundLayer;

	private Vector2 jumpV;
	private float blockWidth = 15f;
	private float ratioBlockJump = 0.75f;
	private bool freeze = false;
	private Vector3 frozenV = Vector3.zero;


	void Start(){
		rb = GetComponent<Rigidbody2D> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		groundLayer = LayerMask.NameToLayer ("Barriers");
		jumpV = (blockWidth * ratioBlockJump * Physics2D.gravity.y * -1)/gameManager.GetSpeed() * Vector2.up;
	}

	void Update(){
		if (!gameManager.gameManager.IsGameOver () && !freeze) {
			if (Input.GetKeyDown ("space") && onGround) {
				rb.velocity = jumpV;
				onGround = false;
			}

			if (rb.velocity.y < 0 && onGround) {
				onGround = false;
			}
		}
	}

	void OnCollisionEnter2D (Collision2D other){
		if (other.gameObject.layer == groundLayer) {
			onGround = true;
		}
	}

	private void CleanOther(GameObject other){
		other.SetActive (false);
		Destroy (other);
	}

	void OnTriggerEnter2D (Collider2D other){

		if (other.gameObject.CompareTag ("Cleaner") && !freeze) {
			gameManager.PlayerDied ();

			// other.gameObject.layer == GameManager_Meta.gameLayers.
		} else if (other.gameObject.layer == GameManager_Meta.gameLayers.IndexOf("Point Objects")){
			gameManager.gameManager.ScorePoints (other.gameObject.GetComponent<ScoringObject> ().points);
			CleanOther (other.gameObject);

		} else if (other.gameObject.layer == GameManager_Meta.gameLayers.IndexOf("Multipliers")){
			gameManager.ObtainedMultiplier ();
			CleanOther (other.gameObject);
		}
	}

	public void ToggleFreeze(){
		freeze = !freeze;
		if (frozenV == Vector3.zero) {
			frozenV = rb.velocity;
			rb.velocity = Vector3.zero;
		} else {
			rb.velocity = frozenV;
			frozenV = Vector3.zero;
		}
	}

	public bool IsGrounded() { return onGround; }
}
