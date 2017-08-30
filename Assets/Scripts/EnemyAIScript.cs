using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIScript : MonoBehaviour {

	// private variables
	private float curHealth; // Current health
	private float maxHealth; // Maximum health

	// Use this for initialization
	void Start () {
		maxHealth = 100.0f;
		curHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DealDamage (float damage) {
		curHealth -= damage;
		//Debug.Log ("Enemy Health: " + curHealth);

		if (curHealth <= 0.0f) {
			//Debug.Log ("Dead.");
			Destroy (gameObject);
		}
	}
}
