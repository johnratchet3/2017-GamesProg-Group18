using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevelScript : MonoBehaviour {

	// public variables
	public string generationMethod; // String used to determine the generation method
	public int numOfRooms; // Variable used for the "default", unused generation. Used to determine how many rooms to be generated.
	public int numOfSecrets; // Legacy Variable. Unused. Originally, was going to be used for the generation of secrets.
	public int numOfRoomsWSecrets; // Legacy Variable. Unused. Originally was going to be used for the generation of rooms with secrets.

	public int[] numOfRoomsPerFloor; // List of integers for how many floors, and how many rooms per floor
	public int maxNumOfLargeRooms; // Maximum number of large rooms generated 

	// private variables
	//private int roomLeft;
	//private int secretLeft;
	//private int roomSecretLeft;

	private GameObject[] roomList; // Used for storing rooms to be potentially generated

	// serialized private fields
	[SerializeField]
	private GameObject startingRoom; // Used for instantiating the starting room
	[SerializeField]
	private GameObject interRoomWall; // Used for instantiating inter room walls


	// Use this for initialization
	void Start () {
		//roomLeft = numOfRooms;
		//secretLeft = numOfSecrets;
		//roomSecretLeft = numOfRoomsWSecrets;

		switch (generationMethod) {
		case "map": // If the Generation method is set to "map"
			mapGeneration (10, 10, numOfRoomsPerFloor.Length); // Generate a map of size 10, 10 and number of floors,
			// as indicated by the length of the List of integers numOfRoomsPerFloor
			break;
		default:
			defaultGeneration (); // Invoke default generation otherwise
			break;
		}
	}

	void mapGeneration(int length, int width, int height) {
		string[,,] map = new string[length,width,height]; // Create a 3 dimensional array of strings for storing the map
		int largeRoomsLeft = maxNumOfLargeRooms; // Establish an integer to store the number of large rooms left to generate

		// Fill the map with 0 for each string
		for (int i = 0; i < length; i++) {
			for (int j = 0; j < width; j++) {
				for (int k = 0; k < height; k++) {
					map[i,j,k] = "0";
					//Debug.Log (i + "," + j + "," + k + "," + map [i, j, k]);
				}
			}
		}
		/* Map codes, for reference
		 * a : Entrance
		 * p : "Empty space"
		 * l : Center Point for a Large Room
		 * s : "Stair" Room
		 * 1 : "Regular" Room
		 * 0 : No Room
		 * 
		 * 
		 */


		// Generate a position for a generic room on the first floor
		int ranX = Random.Range (0, length);
		int ranZ = Random.Range (0, width);
		int ranY = 0;//Random.Range (0, height);

		map [ranX, ranZ, ranY] = "1"; // Generate a regular room

		int currentFloor = ranY; // Set the current floor being generated as the floor with the entrance

		while (currentFloor < height) { // While there are still floors to generate
			int roomsMade = 0; // Set the number of rooms generated to 0
			while (roomsMade < numOfRoomsPerFloor[currentFloor]) { // While there are less than the maximum number of generated rooms for this floor
				bool hasBeenMade = false; // Control boolean, used for checking if the room has been generated
				bool generateLargeRoom = false; // Control boolean, used for checking whether to generate a large room
				if (Random.Range (0, numOfRoomsPerFloor [currentFloor] - roomsMade + 2) == 0 && largeRoomsLeft > 0) { 
					// If the random number generator decides that a large room is to be generated, whilst taking into consideration how many could be generated
					// Chance is 1 in (maximum number of rooms - number of rooms made + 2)
					// E.g. If the maximum number of rooms for the floor is 47, and there are 13 generated rooms,
					// The chance will be 1 in (47 - 13 + 2), or a 1 in 36 chance at that point
					// At most, 1 in 3 chance of a large room being made, combined with the restrictions placed when the room needs to be made
					generateLargeRoom = true; // Set the control boolean for making a large room to true
					//Debug.Log ("LargeRoom Time!");
					largeRoomsLeft--; // Reduce the number of large rooms that can be generated left
				}

				while (!hasBeenMade) { // While a room has not been made

					if (generateLargeRoom) { // If we are generating a large room
						// Generate test co-ordinates
						ranX = Random.Range (1, length - 1);
						ranZ = Random.Range (1, width - 1);
						ranY = currentFloor;//Random.Range (0, height);

						if (generateLargePosition (length, width, currentFloor, map, ranX, ranZ, ranY) && map [ranX, ranZ, ranY] == "0") {
							// Can the current location generated support a large room

							// If so, set the center co-ordinates to "l" and surrounding areas to "p"
							map [ranX, ranZ, ranY] = "l";
							map [ranX - 1, ranZ - 1, ranY] = "p";
							map [ranX - 1, ranZ, ranY] = "p";
							map [ranX - 1, ranZ + 1, ranY] = "p";
							map [ranX, ranZ - 1, ranY] = "p";
							map [ranX, ranZ + 1, ranY] = "p";
							map [ranX + 1, ranZ - 1, ranY] = "p";
							map [ranX + 1, ranZ, ranY] = "p";
							map [ranX + 1, ranZ + 1, ranY] = "p";

							hasBeenMade = true; // Room has been made
							//Debug.Log (ranX + ", " + ranZ + ", " + ranY + " has been converted to large room" );
						}


					} else { // Otherwise
						// Generate test co-ordinates
						ranX = Random.Range (0, length);
						ranZ = Random.Range (0, width);
						ranY = currentFloor;//Random.Range (0, height);

						if (hasAdjancency (ranX, ranZ, ranY, length, width, map) && map [ranX, ranZ, ranY] == "0") {
							// Can the current location support a regular room

							// If so, set the location as a regular room
							map [ranX, ranZ, ranY] = "1";
							hasBeenMade = true; // Room has been made
							//Debug.Log (ranX + ", " + ranZ + ", " + ranY + " has been converted to room" );
						}
					}

				}

				if (generateLargeRoom) { // If a large room has been generated
					roomsMade += 9; // Increase the number of generated rooms by 9 ( Large rooms occupy a 3x3 space )
				} else { // Otherwise
					roomsMade++; // Increment the number of rooms made
				}
			}

			bool hasStairsMade = false; // Control boolean for making "stairs"
			if (currentFloor == height - 1) { // If we are on the top floor, do not make stairs
				hasStairsMade = true; // "Stairs" should already exist by the time we get to the top floor
			}

			while (!hasStairsMade) { // While "stairs" have not been generated OR not on the top floor
				// Generate a location for the "stairs"
				ranX = Random.Range (0, length);
				ranZ = Random.Range (0, width);
				ranY = currentFloor;//Random.Range (0, height);

				// If the current location can support "stairs"
				if (hasAdjancency(ranX, ranZ, ranY, length, width, map) && map[ranX, ranZ, ranY] == "0") {
					map[ranX, ranZ, ranY] = "s"; // Generate "stairs"
					map[ranX, ranZ, ranY+1] = "s"; // Set the position above this location to "stairs"

					hasStairsMade = true; // "Stairs" have been made
					//Debug.Log (ranX + ", " + ranZ + ", " + ranY + " has been converted to stairs" );
				}
			}

			// Other special room generation functionality to be added here in the future

			currentFloor++; // Once finished with this floor, move up to the next floor


		}

		bool entranceMade = false;
		while (!entranceMade) {
			ranX = Random.Range (0, length);
			ranZ = Random.Range (0, width);
			ranY = Random.Range (0, height);

			if (hasAdjancency(ranX, ranZ, ranY, length, width, map) && map[ranX, ranZ, ranY] == "0") {
				map[ranX, ranZ, ranY] = "a"; // denotes entrance
				entranceMade = true;
				//Debug.Log (ranX + ", " + ranZ + ", " + ranY + " has been converted to room" );
			}
		}

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < width; j++) {
				for (int k = 0; k < height; k++) {
					switch (map[i,j,k]) {
					case "a": // i.e. entrance
						generateStartingRoom (i, j, k);
						generateInterRoomWalls (i, j, k, length, width, map);
						break;
					case "1": // i.e. regular room
						generateMapRoom (i, j, k);
						generateInterRoomWalls (i, j, k, length, width, map);
						break;
					case "s": // i.e. "stairs" rooms
						if (k != height - 1) { // i.e. on the top floor
							generateStairRoom(i, j, k);
							generateInterRoomWalls (i, j, k, length, width, map);
							map [i, j, k + 1] = "p"; // Convert the space above the "stairs" to an "empty space"
						}
						break;
					case "l": // i.e. "large room". No need to generate interroom walls, due to the unique way large rooms are done
						generateLargeRoom (i, j, k);
						break;
					case "p": // i.e. "empty space". Top part of stairs
						generateInterRoomWalls (i, j, k, length, width, map);
						break;
					default:
						break;
					}
					//map[i,j,k] = "0";
				}
			}
		}

		//Debug.Log (map);
	}

	bool generateLargePosition(int xWidth, int zWidth, int yLevel, string[,,] mapRef, int ranX, int ranZ, int ranY) {
		bool canBeDone = false;

		bool passedOverlapTests = true;
		bool passedAdjacentTests = false;

		// cluster of ifs to determine if the room can be placed

		if (adjacencyOverlapTest(ranX-1, ranZ-1, yLevel, xWidth, zWidth, mapRef, 1) == 2) {
			passedOverlapTests = false;
		} 
		if (adjacencyOverlapTest(ranX-1, ranZ-1, yLevel, xWidth, zWidth, mapRef, 1) == 0) {
			passedAdjacentTests = true;
		}

		if (adjacencyOverlapTest(ranX-1, ranZ, yLevel, xWidth, zWidth, mapRef, 2) == 2) {
			passedOverlapTests = false;
		} 
		if (adjacencyOverlapTest(ranX-1, ranZ, yLevel, xWidth, zWidth, mapRef, 2) == 0) {
			passedAdjacentTests = true;
		}

		if (adjacencyOverlapTest(ranX-1, ranZ+1, yLevel, xWidth, zWidth, mapRef, 3) == 2) {
			passedOverlapTests = false;
		} 
		if (adjacencyOverlapTest(ranX-1, ranZ+1, yLevel, xWidth, zWidth, mapRef, 3) == 0) {
			passedAdjacentTests = true;
		}

		if (adjacencyOverlapTest(ranX, ranZ-1, yLevel, xWidth, zWidth, mapRef, 4) == 2) {
			passedOverlapTests = false;
		} 
		if (adjacencyOverlapTest(ranX, ranZ-1, yLevel, xWidth, zWidth, mapRef, 4) == 0) {
			passedAdjacentTests = true;
		}

		if (adjacencyOverlapTest(ranX, ranZ+1, yLevel, xWidth, zWidth, mapRef, 6) == 2) {
			passedOverlapTests = false;
		} 
		if (adjacencyOverlapTest(ranX, ranZ+1, yLevel, xWidth, zWidth, mapRef, 6) == 0) {
			passedAdjacentTests = true;
		}

		if (adjacencyOverlapTest(ranX+1, ranZ-1, yLevel, xWidth, zWidth, mapRef, 7) == 2) {
			passedOverlapTests = false;
		} 
		if (adjacencyOverlapTest(ranX+1, ranZ-1, yLevel, xWidth, zWidth, mapRef, 7) == 0) {
			passedAdjacentTests = true;
		}

		if (adjacencyOverlapTest(ranX+1, ranZ, yLevel, xWidth, zWidth, mapRef, 8) == 2) {
			passedOverlapTests = false;
		} 
		if (adjacencyOverlapTest(ranX+1, ranZ, yLevel, xWidth, zWidth, mapRef, 8) == 0) {
			passedAdjacentTests = true;
		}

		if (adjacencyOverlapTest(ranX+1, ranZ+1, yLevel, xWidth, zWidth, mapRef, 9) == 2) {
			passedOverlapTests = false;
		} 
		if (adjacencyOverlapTest(ranX+1, ranZ+1, yLevel, xWidth, zWidth, mapRef, 9) == 0) {
			passedAdjacentTests = true;
		}

		// end of cluster of ifs



		if (passedOverlapTests && passedAdjacentTests) {
			canBeDone = true;
		}

		return canBeDone;
	}

	int adjacencyOverlapTest(int xPos, int zPos, int yPos, int lengthRef, int widthRef, string[,,]mapRef, int ruleSet) { 
		// variation on adjacency tests, with ruleset to be applied
		// used for making large rooms, and also tests overlap on the tile

		/*
		 *      1  2  3
		 *      4  5  6
		 *      7  8  9
		 *    0 is default behaviour
		 */

		bool testUp = true; // can test "up"
		bool testDown = true; // can test "down"
		bool testRight = true; // can test "right"
		bool testLeft = true; // can test "left"

		switch (ruleSet) {
		case 1:
			testDown = false;
			testRight = false;
			break;
		case 2:
			testDown = false;
			testLeft = false;
			testRight = false;
			break;
		case 3:
			testDown = false;
			testLeft = false;
			break;
		case 4:
			testRight = false;
			testUp = false;
			testDown = false;
			break;
		case 5:
			testUp = false;
			testDown = false;
			testLeft = false;
			testRight = false;
			break;
		case 6:
			testLeft = false;
			testUp = false;
			testDown = false;
			break;
		case 7:
			testUp = false;
			testRight = false;
			break;
		case 8:
			testUp = false;
			testLeft = false;
			testRight = false;
			break;
		case 9:
			testLeft = false;
			testUp = false;
			break;
		default:
			break;
		}

		if (xPos == lengthRef - 1) {
			testUp = false; // can no longer test "up"
		}
		if (xPos == 0) {
			testDown = false; // can no longer test "down"
		}
		if (zPos == widthRef - 1) {
			testRight = false; // can no longer test "right"
		}
		if (zPos == 0) {
			testLeft = false; // can no longer test "left"
		}

		bool hasAdjacency = false;
		if (testUp) {
			if (mapRef[xPos + 1, zPos, yPos] != "0") { // i.e. room exists "up"
				hasAdjacency = true;
			}
		}

		if (testDown) {
			if (mapRef[xPos - 1, zPos, yPos] != "0") { // i.e. room exists "down"
				hasAdjacency = true;
			}
		}

		if (testRight) {
			if (mapRef[xPos, zPos + 1, yPos] != "0") { // i.e. room exists "right"
				hasAdjacency = true;
			}
		}

		if (testLeft) {
			if (mapRef[xPos, zPos - 1, yPos] != "0") { // i.e. room exists "left"
				hasAdjacency = true;
			}
		}

		if (mapRef[xPos, zPos, yPos] != "0") { // i.e. is NOT an empty tile
			return 2; // i.e. automatic fail
		}

		if (hasAdjacency) {
			return 0; // i.e. potential pass
		}
			
		return 1; // i.e. potential pass

	}

	bool hasAdjancency(int xPos, int zPos, int yPos, int lengthRef, int widthRef, string[,,]mapRef) { // checks for adjacency of rooms in mapGeneration
		bool testUp = true; // can test "up"
		bool testDown = true; // can test "down"
		bool testRight = true; // can test "right"
		bool testLeft = true; // can test "left"

		if (xPos == lengthRef - 1) {
			testUp = false; // can no longer test "up"
		}
		if (xPos == 0) {
			testDown = false; // can no longer test "down"
		}
		if (zPos == widthRef - 1) {
			testRight = false; // can no longer test "right"
		}
		if (zPos == 0) {
			testLeft = false; // can no longer test "left"
		}

		if (testUp) {
			if (mapRef[xPos + 1, zPos, yPos] != "0") { // i.e. room exists "up"
				return true;
			}
		}

		if (testDown) {
			if (mapRef[xPos - 1, zPos, yPos] != "0") { // i.e. room exists "down"
				return true;
			}
		}

		if (testRight) {
			if (mapRef[xPos, zPos + 1, yPos] != "0") { // i.e. room exists "right"
				return true;
			}
		}

		if (testLeft) {
			if (mapRef[xPos, zPos - 1, yPos] != "0") { // i.e. room exists "left"
				return true;
			}
		}

		return false;

	}

	void generateStartingRoom(int xPos, int zPos, int yPos) {
		GameObject baseRoom = Instantiate (startingRoom, new Vector3(xPos * 30.0f, yPos*50.0f, zPos * 30.0f), Quaternion.identity);
	}
	/*
	void generateMapRoom(int xPos, int zPos, int yPos, int lengthRef, int widthRef, string[,,]mapRef) {
		roomList = Resources.LoadAll<GameObject>("Rooms");
		GameObject roomToBuild = roomList [Random.Range (0, roomList.Length)];
		GameObject newRoom = Instantiate (roomToBuild, new Vector3(xPos * 30.0f, yPos*50.0f, zPos * 30.0f), Quaternion.Euler(0.0f, 90.0f * Random.Range(0, 3), 0.0f)) as GameObject;

		bool testUp = true; // can test "up"
		bool testDown = true; // can test "down"
		bool testRight = true; // can test "right"
		bool testLeft = true; // can test "left"

		if (xPos == lengthRef - 1) {
			testUp = false; // can no longer test "up"
		}
		if (xPos == 0) {
			testDown = false; // can no longer test "down"
		}
		if (zPos == widthRef - 1) {
			testRight = false; // can no longer test "right"
		}
		if (zPos == 0) {
			testLeft = false; // can no longer test "left"
		}

		if (testUp) {
			if (mapRef [xPos + 1, zPos, yPos] == "0") { // i.e. room does not exist "up"
				GameObject newInterRoomWall = Instantiate (interRoomWall, new Vector3 (xPos * 30.0f + 15.0f, yPos * 50.0f + 2.15f, zPos * 30.0f), Quaternion.Euler (0.0f, 0.0f, 0.0f));
			}
		} else {
			GameObject newInterRoomWall = Instantiate (interRoomWall, new Vector3 (xPos * 30.0f + 15.0f, yPos * 50.0f + 2.15f, zPos * 30.0f), Quaternion.Euler (0.0f, 0.0f, 0.0f));
		}

		if (testDown) {
			//Debug.Log ("Map Position of Generated Room: " + xPos + "," + yPos + "," + zPos + "\nWhat's 'down': " + mapRef[xPos - 1, zPos, yPos]);
			if (mapRef [xPos - 1, zPos, yPos] == "0") { // i.e. room does not exist "down"
				GameObject newInterRoomWall = Instantiate (interRoomWall, new Vector3 (xPos * 30.0f - 15.0f, yPos * 50.0f + 2.15f, zPos * 30.0f), Quaternion.Euler (0.0f, 0.0f, 0.0f));
			}
		} else {
			GameObject newInterRoomWall = Instantiate(interRoomWall, new Vector3(xPos * 30.0f - 15.0f, yPos*50.0f + 2.15f, zPos * 30.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
		}

		if (testRight) {
			if (mapRef [xPos, zPos + 1, yPos] == "0") { // i.e. room does not exist "right"
				GameObject newInterRoomWall = Instantiate (interRoomWall, new Vector3 (xPos * 30.0f, yPos * 50.0f + 2.15f, zPos * 30.0f + 15.0f), Quaternion.Euler (0.0f, 90.0f, 0.0f));
			}
		} else {
			GameObject newInterRoomWall = Instantiate(interRoomWall, new Vector3(xPos * 30.0f, yPos*50.0f + 2.15f, zPos * 30.0f + 15.0f), Quaternion.Euler(0.0f, 90.0f, 0.0f));
		}

		if (testLeft) {
			if (mapRef [xPos, zPos - 1, yPos] == "0") { // i.e. room does not exist "left"
				GameObject newInterRoomWall = Instantiate (interRoomWall, new Vector3 (xPos * 30.0f, yPos * 50.0f + 2.15f, zPos * 30.0f - 15.0f), Quaternion.Euler (0.0f, 90.0f, 0.0f));
			}
		} else {
			GameObject newInterRoomWall = Instantiate(interRoomWall, new Vector3(xPos * 30.0f, yPos*50.0f + 2.15f, zPos * 30.0f - 15.0f), Quaternion.Euler(0.0f, 90.0f, 0.0f));
		}

	}
	*/

	void generateInterRoomWalls(int xPos, int zPos, int yPos, int lengthRef, int widthRef, string[,,]mapRef) {
		bool testUp = true; // can test "up"
		bool testDown = true; // can test "down"
		bool testRight = true; // can test "right"
		bool testLeft = true; // can test "left"

		if (xPos == lengthRef - 1) {
			testUp = false; // can no longer test "up"
		}
		if (xPos == 0) {
			testDown = false; // can no longer test "down"
		}
		if (zPos == widthRef - 1) {
			testRight = false; // can no longer test "right"
		}
		if (zPos == 0) {
			testLeft = false; // can no longer test "left"
		}

		if (testUp) {
			if (mapRef [xPos + 1, zPos, yPos] == "0") { // i.e. room does not exist "up"
				GameObject newInterRoomWall = Instantiate (interRoomWall, new Vector3 (xPos * 30.0f + 15.0f, yPos * 50.0f + 2.15f, zPos * 30.0f), Quaternion.Euler (0.0f, 0.0f, 0.0f));
			}
		} else {
			GameObject newInterRoomWall = Instantiate (interRoomWall, new Vector3 (xPos * 30.0f + 15.0f, yPos * 50.0f + 2.15f, zPos * 30.0f), Quaternion.Euler (0.0f, 0.0f, 0.0f));
		}

		if (testDown) {
			//Debug.Log ("Map Position of Generated Room: " + xPos + "," + yPos + "," + zPos + "\nWhat's 'down': " + mapRef[xPos - 1, zPos, yPos]);
			if (mapRef [xPos - 1, zPos, yPos] == "0") { // i.e. room does not exist "down"
				GameObject newInterRoomWall = Instantiate (interRoomWall, new Vector3 (xPos * 30.0f - 15.0f, yPos * 50.0f + 2.15f, zPos * 30.0f), Quaternion.Euler (0.0f, 0.0f, 0.0f));
			}
		} else {
			GameObject newInterRoomWall = Instantiate(interRoomWall, new Vector3(xPos * 30.0f - 15.0f, yPos*50.0f + 2.15f, zPos * 30.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
		}

		if (testRight) {
			if (mapRef [xPos, zPos + 1, yPos] == "0") { // i.e. room does not exist "right"
				GameObject newInterRoomWall = Instantiate (interRoomWall, new Vector3 (xPos * 30.0f, yPos * 50.0f + 2.15f, zPos * 30.0f + 15.0f), Quaternion.Euler (0.0f, 90.0f, 0.0f));
			}
		} else {
			GameObject newInterRoomWall = Instantiate(interRoomWall, new Vector3(xPos * 30.0f, yPos*50.0f + 2.15f, zPos * 30.0f + 15.0f), Quaternion.Euler(0.0f, 90.0f, 0.0f));
		}

		if (testLeft) {
			if (mapRef [xPos, zPos - 1, yPos] == "0") { // i.e. room does not exist "left"
				GameObject newInterRoomWall = Instantiate (interRoomWall, new Vector3 (xPos * 30.0f, yPos * 50.0f + 2.15f, zPos * 30.0f - 15.0f), Quaternion.Euler (0.0f, 90.0f, 0.0f));
			}
		} else {
			GameObject newInterRoomWall = Instantiate(interRoomWall, new Vector3(xPos * 30.0f, yPos*50.0f + 2.15f, zPos * 30.0f - 15.0f), Quaternion.Euler(0.0f, 90.0f, 0.0f));
		}

	}

	void generateMapRoom(int xPos, int zPos, int yPos) { // original algorithm, kept for reference
		roomList = Resources.LoadAll<GameObject>("Rooms");
		GameObject roomToBuild = roomList [Random.Range (0, roomList.Length)];
		GameObject newRoom = Instantiate (roomToBuild, new Vector3(xPos * 30.0f, yPos*50.0f, zPos * 30.0f), Quaternion.Euler(0.0f, 90.0f * Random.Range(0, 3), 0.0f)) as GameObject;
	}

	void generateStairRoom(int xPos, int zPos, int yPos) { // original algorithm, kept for reference
		roomList = Resources.LoadAll<GameObject>("Stairs");
		GameObject roomToBuild = roomList [Random.Range (0, roomList.Length)];
		GameObject newRoom = Instantiate (roomToBuild, new Vector3(xPos * 30.0f, yPos*50.0f, zPos * 30.0f), Quaternion.Euler(0.0f, 90.0f * Random.Range(0, 3), 0.0f)) as GameObject;
	}

	void generateLargeRoom(int xPos, int zPos, int yPos) { // original algorithm, kept for reference
		roomList = Resources.LoadAll<GameObject>("LargeRooms");
		GameObject roomToBuild = roomList [Random.Range (0, roomList.Length)];
		GameObject newRoom = Instantiate (roomToBuild, new Vector3(xPos * 30.0f, yPos*50.0f, zPos * 30.0f), Quaternion.Euler(0.0f, 90.0f * Random.Range(0, 3), 0.0f)) as GameObject;
	}

	void defaultGeneration() {

		GameObject baseRoom = Instantiate (startingRoom, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
		GameObject prevRoom = baseRoom;
		//Vector3 roomPos = new Vector3(0.0f, 0.0f, 0.0f);
		Debug.Log ("Room -1 " + startingRoom.transform.position);
		for (int i = 0 ; i < numOfRooms-1 ; i++ ) {
			prevRoom = generateRoom (prevRoom.transform.position, prevRoom.transform.localScale);
			Debug.Log ("Room " + i + " " + prevRoom.transform.position);
		}
	}

	Vector3 skewDirection(Vector3 lastRoomPosition, Vector3 lastRoomScale, GameObject refObject) {

		//psudo code for picking x or z first
		float chosenXValue = (refObject.transform.localScale.x/2) + (lastRoomScale.x/2);
		float chosenZValue = (refObject.transform.localScale.z/2) + (lastRoomScale.z/2);

		//deciding if + or -
		if (Random.value <= 0.5f) {
			chosenXValue = chosenXValue * -1;
			chosenZValue = chosenZValue * -1;
		}

		if (Random.value <= 0.5f) {
			return new Vector3(lastRoomPosition.x + chosenXValue, lastRoomPosition.y, lastRoomPosition.z);
		} else {
			return new Vector3(lastRoomPosition.x, lastRoomPosition.y, lastRoomPosition.z + chosenZValue);
		}
		//Vector3 positionToBuild = new Vector3(lastRoomPosition.x + chosenValue, lastRoomPosition.y, lastRoomPosition.z);

		return new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
	}

	GameObject generateRoom(Vector3 lastRoomPosition, Vector3 lastRoomScale) {
		roomList = Resources.LoadAll<GameObject>("Rooms");
		GameObject roomToBuild = roomList [Random.Range (0, roomList.Length)];
		//roomToBuild.transform.localScale.x;
		Vector3 positionToBuild = skewDirection(lastRoomPosition, lastRoomScale, roomToBuild);
		GameObject newRoom = Instantiate (roomToBuild, positionToBuild, Quaternion.Euler(0.0f, 90.0f * Random.Range(0, 3), 0.0f)) as GameObject;
		Debug.Log ("thingy width? " + newRoom.transform.localScale.x);
		return newRoom; //new Vector3(newRoom.transform.position.x + (newRoom.transform.localScale.x/2), 0.0f, 0.0f);

	}

	void generateRoom() {
		roomList = Resources.LoadAll<GameObject>("Rooms");
		GameObject roomToBuild = roomList [Random.Range (0, roomList.Length)];
		GameObject newRoom = Instantiate (roomToBuild, gameObject.transform.position, Quaternion.identity) as GameObject;

	}

	// Update is called once per frame
	void Update () {
		
	}
}
