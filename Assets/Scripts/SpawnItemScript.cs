using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItemScript : MonoBehaviour {

	// public variables
	public bool isFixedItem; // Boolean for if it is a fixed item

	// private variables
	//private GameObject[] itemList = Resources.FindObjectsOfTypeAll(typeof(GameObject));

	// serialized private variables
	[SerializeField]
	private GameObject fixedItem; // Determines the fixed item

	// Use this for initialization
	void Start () {
		if (isFixedItem) { // If it is a fixed item
			Instantiate (fixedItem, gameObject.transform.position, Quaternion.identity);
			// Spawn the fixed item
			Destroy (gameObject); // Destroy the spawn point
		} else { // Otehrwise
			GameObject[] itemList = Resources.LoadAll<GameObject>("Ammo"); // Generate a list of all ammo types
			GameObject itemToBuild = itemList [Random.Range (0, itemList.Length)]; // Select a random ammo type
			Vector3 spawnLocation = new Vector3(gameObject.transform.position.x, 
				gameObject.transform.position.y + (itemToBuild.transform.localScale.y / 2), 
				gameObject.transform.position.z);
			// Figure out a spawn point based on the current 
			GameObject itemRoom = Instantiate (itemToBuild, spawnLocation, Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f)) as GameObject;
			// Spawn that ammo type
			Destroy (gameObject); // Destroy the spawn point
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
