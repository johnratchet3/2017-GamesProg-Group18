using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

	// public variables
	public float walkSpeed; // The maximum walking speed i.e. the "speed limit" when a player is on the ground
	public float jumpHeight; // The player's jump height

	// private variables
	private Vector3 momentum; // Note: could be redundant, as the Rigidbody component of the player could be used instead
	private GameObject[] inventory; // Note: could be redundant, as we will instead be using classic doom style weapon system

	private bool canJump; // Boolean for if the player is in contact with a surface the player can jump off
	private bool hasJumped; // Boolean for if the player has jumped
	private bool devJumping; // Boolean for if we want to enable infinite jumping for level exploring purposes

	private float jumpMultiplier; // Floating point modifier for jumping height

	private float curHealth; // Current health
	private float maxHealth; // Maximum health
	private float ovmHealth; // Maximum over-max health

	// serialized private variables
	[SerializeField]
	private GameObject playerCamera; // Ease of access to camera and weapons

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the screen
		canJump = true; // Player can jump
		hasJumped = false; // Player has not yet jumped
		devJumping = false; // Disables the infinite jumping by default
		jumpMultiplier = 1.0f; // Sets the player's jump multiplier to the default of 100%. Note: 1.00f is 100% for reference
		maxHealth = 100.0f;
		curHealth = maxHealth;
		ovmHealth = 200.0f;
	}
	
	// Update is called once per frame
	void Update () {

		float relZChange = Input.GetAxis ("Vertical") * walkSpeed * Time.deltaTime;
		float relXChange = Input.GetAxis ("Horizontal") * walkSpeed * Time.deltaTime;

		transform.Translate (new Vector3 (relXChange, 0.0f, relZChange));
		// moves the player in a direction based on where they are looking, depending on how far over each axis is.

		/*
		William Palmer's code for enforcing a speed limit alongside 2 axis (x and y), borrowed from a top down space shooter in Unity that he helped to develop.
		This will need to be modified for using the x and z axis, as this will then allow equal speed in all directions.
		for reference: rb refers to the RigidBody component in this code.
		rb.AddForce(transform.up * accelerationRate); // To go "up", but not forward
		if (Mathf.Sqrt(Mathf.Abs(rb.velocity.x*rb.velocity.x + rb.velocity.y*rb.velocity.y)) > Mathf.Sqrt(maxVelocity*maxVelocity)) {
			if (rb.velocity.x < 0) {
				rb.velocity = new Vector2(rb.velocity.x + Mathf.Abs(overMaxSpeedDecelerationRate*decelerationRate), rb.velocity.y);
			} else if (rb.velocity.x > 0) {
				rb.velocity = new Vector2(rb.velocity.x - Mathf.Abs(overMaxSpeedDecelerationRate*decelerationRate), rb.velocity.y);
			}

			if (rb.velocity.y < 0) {
				rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + Mathf.Abs(overMaxSpeedDecelerationRate*decelerationRate));
			} else if (rb.velocity.y > 0) {
				rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - Mathf.Abs(overMaxSpeedDecelerationRate*decelerationRate));
			}
		}

		*/


		// keys

		if (Input.GetKeyDown("escape")) { // toggle the state of the cursor
			switch (Cursor.lockState) {
			case CursorLockMode.Locked: // if the cursor is locked to the screen
				Cursor.lockState = CursorLockMode.None; // unlock the cursor from the screen
				break;
			default: // otherwise
				Cursor.lockState = CursorLockMode.Locked; // lock the cursor to the screen
				break;
			}
		}

		if (Input.GetButtonDown("Jump") && canJump == true && hasJumped == false) {
			gameObject.GetComponent<Rigidbody> ().AddForce (Vector3.up * jumpHeight * jumpMultiplier, ForceMode.Impulse);
			// Jump into the air, based on the player's jump height and the jump multiplier
			hasJumped = true; // The player has jumped
		}

		if (Input.GetButtonDown("Jump") && devJumping) {
			gameObject.GetComponent<Rigidbody> ().AddForce (Vector3.up * jumpHeight * jumpMultiplier, ForceMode.Impulse); 
			// Jump into the air, based on the player's jump height and the jump multiplier
			//hasJumped = true;
		}

		if (Input.GetKeyDown(KeyCode.Tab)) { // Upon pressing TAB
			devJumping = !devJumping; // Toggle infinite jumping
		}

	}

	void OnCollisionEnter(Collision col) {
		if (col.collider.tag == "Floor") { // If on any floor
			canJump = true; // Player can whilst in contact with floors
			if (hasJumped) { // If the player has jumped previously
				hasJumped = false; // Enable the player to be able to jump again
			} // Bug: sometimes walking/jumping between floors can cause the player to be unable to jump
		} else if (col.collider.tag == "JumpPad") { // If the player runs into a "jump pad"
			canJump = false; // Player can not jump
			gameObject.GetComponent<Rigidbody> ().AddForce (Vector3.up * 35.0f, ForceMode.Impulse); // Add a vertical force to the player's Rigidbody
		} else if (col.collider.tag == "ThrustPad") { // If the player runs into a "thrust pad"
			canJump = false; // Player can not jump
			gameObject.GetComponent<Rigidbody> ().AddForce (Vector3.up * 15.0f, ForceMode.Impulse); // Add a vertical force to the player's Rigidbody
			Vector3 rotationVector = new Vector3(col.gameObject.transform.rotation.x, col.gameObject.transform.rotation.y, col.gameObject.transform.rotation.z);
			// Calculate the angle the "thrust pad" is facing
			rotationVector = Quaternion.Euler(rotationVector) * col.gameObject.transform.right; // Convert the "thrust pad's" rotation to a direction
			gameObject.GetComponent<Rigidbody> ().AddForce (rotationVector * 15.0f, ForceMode.Impulse); // Add a forward force to the player's Rigidbody
		}
	}

	void OnCollisionExit(Collision col) {
		if (col.collider.tag == "Floor") { // Upon no longer being in contact with the floor
			canJump = false; // Player can not jump in mid-air
		}
	}

	// Public Methods for interaction
	public void DealDamage (float damage) {
		curHealth -= damage;
		Debug.Log ("Current Health: " + curHealth);
	}

	public void PickUp (int amount, string type) {
		playerCamera.GetComponent<PlayerCameraScript>().PickupAmmo(amount, type);
		//GetComponentInChildren<PlayerCameraScript> ().PickupAmmo (amount);
	}
}
