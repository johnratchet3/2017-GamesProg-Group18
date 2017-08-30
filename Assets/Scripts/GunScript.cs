using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {

	// public variables

	// private variables

	// serialized private variables
	[SerializeField]
	private bool infiniteAmmo = false; // Used to distinguish if the weapon has inifinite ammo
	[SerializeField]
	private bool isLaser = false; // Used to distinguish "projectiles" from "lasers"
	[SerializeField]
	private GameObject bullet; // Used for the projectile fired by the gun
	[SerializeField]
	private GameObject firingPoint; // Used for the point at which a projectile is fired from
	[SerializeField]
	private float damageOutput; // Damage the weapon deals
	[SerializeField]
	private float maxRange; // Maximum range of the projectile
	[SerializeField]
	private float projectileSpeed; // Speed of the projectile
	[SerializeField]
	private string ammoType; // Ammo type used by the gun
	[SerializeField]
	private int ammoLeft; // Ammo the gun has access to
	[SerializeField]
	private bool usesClips = false; // Does the weapon use clips
	[SerializeField]
	private int clipSize = 1; // Ammo in the clip

	// Use this for initialization
	void Start () {
		if (infiniteAmmo) { // If the gun has infinite ammo usage
			usesClips = true; // Disable the usage of clips
			clipSize = 0;
		}
		if (usesClips) { // If the weapon uses clips
			ammoLeft = clipSize; // Fill the magazine up
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FireWeapon() {
		if (infiniteAmmo) { // If the gun has infinite ammo
			ammoLeft = 1; // Give the gun exactly one round
		}

		if (ammoLeft >= 1) { // If we have at least one round
			if (!isLaser) { // If not a laser
				GameObject projectile = Instantiate (bullet, firingPoint.transform.position, Quaternion.Euler(firingPoint.transform.right)); 
				// Create a new bullet at the player's position and facing where the player is facing
				projectile.GetComponent<ProjectileScript> ().SetFiringParameters(maxRange, damageOutput, projectileSpeed);
				// Tell the projectile it's parameters, Range, Damage and Speed
				projectile.GetComponent<Rigidbody> ().AddForce (gameObject.transform.forward*projectileSpeed, ForceMode.Impulse);
				// Propel the projectile
			}
			if (!infiniteAmmo) { // If infinite ammo does not apply
				if (usesClips) {
					ammoLeft--; // Take some ammo away
				}
				Debug.Log("Rounds Left: " + ammoLeft); // Debug Log the ammo left - will be replaced by a UI call.
			}


		} else {
			ammoLeft = 0;
			Debug.Log ("No rounds remaining");
		}

		//Debug.Log ("Should have fired."); // Debug message for telling us if the player has fired
	}

	public void AddAmmo(int amount) { // Adds the specified amount of ammo to the gun
		ammoLeft += amount;
		Debug.Log ("Got Ammo: " + amount);
	}

	public string GetAmmoType() { // Returns the ammo type
		return ammoType;
	}

	public bool GetClipUsage() { // Returns if the weapon uses clips, switch for if to use own ammo supply
		return usesClips;
	}

	public int GetAmmo() { // Returns the amount of ammo left, used for reloading purposes
		return ammoLeft;
	}

	public int GetClipSize() { // Returns the clip size of the weapon
		return clipSize;
	}
}
