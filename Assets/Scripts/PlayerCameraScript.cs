using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour {

	// public variables
	public float sensitivity = 5.0f; // Used for determining the mouse sensitivity
	public float smoothing = 2.0f; // Used for smoothing out mouselooking

	// private variables
	private Vector2 mouseLook; // Mouse direction
	private Vector2 smoothV; // Mouse smoothing
	private GameObject player; // Player
	private bool hasFired; // Have you fired?
	private int[] ammo = new int[2] {20, 0}; // Used for storing ammo
	// Corresponding Ammo Types: 9mm, HEG

	// serialized private variables
	[SerializeField]
	private GameObject hand; // Used for interacting with weapons

	// Use this for initialization
	void Start () {
		player = this.transform.parent.gameObject; // assign the player as the ... player. Ease of access.
		hasFired = false; // The player has not yet fired
	}
	
	// Update is called once per frame
	void Update () {
		var md = new Vector2 (Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")); 
		// Mouse Direction is a Vector2 consisting of how much movement the mouse has made
		md = Vector2.Scale (md, new Vector2 (sensitivity*smoothing, sensitivity*smoothing));
		// Scale the Mouse Direction according to sensitivity and smoothing
		smoothV.x = Mathf.Lerp (smoothV.x, md.x, 1.0f/sensitivity); // Linearly interpolate X mouse movement
		smoothV.y = Mathf.Lerp (smoothV.y, md.y, 1.0f/sensitivity); // Linearly interpolate Y mouse movement
		mouseLook = mouseLook + smoothV; // Add the Mouselook with the Smoothing

		transform.localRotation = Quaternion.AngleAxis (-mouseLook.y, Vector3.right); // Get the camera to look up and down axis
		player.transform.localRotation = Quaternion.AngleAxis (mouseLook.x, player.transform.up); // Lock the player rotation to the x axis

		if (Input.GetButtonDown("Fire1") && hasFired == false) { // If the "fire1" button has been pushed and we are yet to fire 
			hasFired = true; // The player has fired
			if (!hand.GetComponent<GunScript> ().GetClipUsage ()) {
				switch (hand.GetComponent<GunScript> ().GetAmmoType ()) {
				case "9mm":
					if (ammo [0] >= 1) { // If we have at least one 9mm round
						ammo [0] -= 1; // Reduce number of 9mm rounds we have
						hand.GetComponent<GunScript> ().FireWeapon (); // Fire!
					}
					break;
				case "HEG":
					if (ammo [1] >= 1) {
						ammo [1] -= 1;
						hand.GetComponent<GunScript> ().FireWeapon ();
					}
					break;
				default:
					break;
				}
			} else {
				hand.GetComponent<GunScript> ().FireWeapon ();
			}
			//Debug.Log ("Fire1 has been pressed!");
		}

		if (Input.GetButtonUp("Fire1")) { // If the "Fire1" button has been released
			hasFired = false; // Player can fire again
		}

		if (Input.GetButtonDown("Fire2")) {
			ReloadWeapon ();
		}

	}

	public void PickupAmmo(int amount) {
		hand.GetComponent<GunScript> ().AddAmmo (amount);
	}

	public void PickupAmmo(int amount, string type) {
		switch (type) {
		case "9mm":
			ammo [0] += amount;
			break;
		case "HEG":
			ammo [1] += amount;
			break;
		default:
			break;
		}
	}

	public void ReloadWeapon() {
		if (hand.GetComponent<GunScript> ().GetClipUsage()) {
			int maxClip = hand.GetComponent<GunScript> ().GetClipSize ();
			int curClip = hand.GetComponent<GunScript> ().GetAmmo ();
			int maxGive = maxClip - curClip;
			if (maxGive > 0) { // If the weapon doesn't use infinite ammo
				int giveAmmo = 0; // Amount of ammo that can be given to the weapon
				switch (hand.GetComponent<GunScript> ().GetAmmoType ()) {
				case "9mm":
					if (maxGive >= ammo [0]) { // If we have less than the maximum amount of ammo that can be reloaded into the weapon
						giveAmmo = ammo [0];
						ammo [0] = 0;
					} else {
						giveAmmo = maxGive;
						ammo [0] -= maxGive;
					}
					break;
				case "HEG":
					if (maxGive >= ammo [1]) { // If we have less than the maximum amount of ammo that can be reloaded into the weapon
						giveAmmo = ammo [1];
						ammo [1] = 0;
					} else {
						giveAmmo = maxGive;
						ammo [1] -= maxGive;
					}
					break;
				default:
					break;
				}
				hand.GetComponent<GunScript> ().AddAmmo (giveAmmo);
			}
		}
	}
}
