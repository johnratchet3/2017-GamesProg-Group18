using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleScript : MonoBehaviour {

	// serialized private variables
	[SerializeField]
	private string ammoType;
	[SerializeField]
	private int ammoAmount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col) {
		if (col.collider.tag == "Player") {
			col.gameObject.GetComponent<PlayerControllerScript> ().PickUp(ammoAmount, ammoType);
			Destroy (gameObject); // Destroy the projectile upon collision
			
		}


	}
}
