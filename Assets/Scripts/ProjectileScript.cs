using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour {

	// private variables
	private float maxRange = 0.1f; // Maximum range
	private float damage = 0.0f; // Damage dealt
	private float speed = 0.0f; // Speed of the projectile

	private float totalDistance; // Total distance covered
	private float totalTime; // Total time existed

	// serialized private variables
	[SerializeField]
	private bool explodes = false; // Does the projectile explode?
	[SerializeField]
	private float explosionRadius = 0.0f; // The radius of the explosion
	[SerializeField]
	private float expDamage = 0.1f; // Maximum damage caused by the explosion, which decreases as distance from center increases

	[SerializeField]
	private bool usesTimer = false; // Does the projectile use a timer instead of range?
	[SerializeField]
	private float timer = 0.1f; // How long the "timer" is for the projectile

	// Use this for initialization
	void Start () {
		totalDistance = 0.0f; // Total distance = 0
		totalTime = 0.0f;
		//Debug.Log("Rx: " + gameObject.transform.rotation.x + "\nRy: " + gameObject.transform.rotation.y + "\nRz: " + gameObject.transform.rotation.z);
	}
	
	// Update is called once per frame
	void Update () {
		if (!usesTimer) {
			if (totalDistance >= maxRange) { // If the total distance covered is more than the maximum range
				if (explodes) { // If the projectile explodes
					//gameObject explosion = Instantiate(explosionGO, gameObject.transform.position, Quaternion.Euler(new Vector3(0,0,0)));
					//explosion.setParameters(expDamage, explosionRadius);
				}
				Destroy (gameObject); // Destroy the projectile
				//Debug.Log("Projectile Removed");
			}

			totalDistance += speed * Time.deltaTime;
			//Debug.Log ("Distance: " + totalDistance);
			// Increase the total distance by the amount covered per frame
		} else {
			if (totalTime >= timer) { // If the time limit has run out
				if (explodes) {
					//gameObject explosion = Instantiate(explosionGO, gameObject.transform.position, Quaternion.Euler(new Vector3(0,0,0)));
					//explosion.setParameters(expDamage, explosionRadius);
				}
				Destroy(gameObject); // Destroy the porjectile
			}
			totalTime += Time.deltaTime; // Increase the totalTime spent
		}


	}

	// set up the firing parameters for the projectile
	public void SetFiringParameters (float newRange, float newDamage, float newSpeed) {
		maxRange = newRange; // The range of the projectile is set
		damage = newDamage; // The damage of the projectile is set
		speed = newSpeed; // The speed of the projectile is set
	}

	// set up the firing parameters for the projectile, with parameters for explosions
	public void SetFiringParameters (float newRange, float newDamage, float newSpeed, float newExpDam, float expRad) {
		maxRange = newRange; // The range of the projectile is set
		damage = newDamage; // The damage of the projectile is set
		speed = newSpeed; // The speed of the projectile is set
		explodes = true; // This projectile explodes
		explosionRadius = expRad; // Radius of the explosion
		expDamage = newExpDam; // Maximum damage of the explosion
	}

	void OnCollisionEnter(Collision col) {
		if (!usesTimer) {
			if (col.collider.tag == "Player") {
				col.gameObject.GetComponent<PlayerControllerScript> ().DealDamage (damage);
			} else if (col.collider.tag == "Enemy") {
				col.gameObject.GetComponent<EnemyAIScript> ().DealDamage (damage);
			}

			Destroy (gameObject); // Destroy the projectile upon collision
		}
	}

}
