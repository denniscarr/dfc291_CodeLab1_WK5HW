using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class LevelSetup : MonoBehaviour
{

	// Level chunks
	public GameObject regularFloor;
	public GameObject wall;
	public GameObject grabbableFloor;
	public GameObject lava;

	// Player/Skeleton prefabs
	public GameObject skeleton;

	public float chunkSize = 2.5f;

	// Characters used in text file
	const char REGULAR_FLOOR_CHAR = '.';
	const char WALL_CHAR = 'x';
	const char GRABBABLE_FLOOR_CHAR = '/';
	const char LAVA_CHAR = 'o';
	const char PLAYER_START_CHAR = 'p';
	const char SKELETON_START_CHAR = 's';

    float floorHeight;
	float lavaHeight;
	float wallHeight;

    public int levelNumber;



	public void SetupLevel(int levelNum)
	{
        // Clear the scene of any previously created level
        DestroyImmediate(GameObject.Find("Level"));

        // Get proper heights for each block type:
        wallHeight = chunkSize / 5;
        floorHeight = -chunkSize / 2;
        lavaHeight = -2f;

		StreamReader streamReader = new StreamReader (Application.dataPath + "/Levels/Level" + levelNum + ".txt");

		// The current position in world space.
		float levelPosX = 0f;
        float levelPosZ = 0f;

		// Create empty game objects to hold the pieces of the level.
		GameObject levelHolder = new GameObject ("Level");

		GameObject wallHolder = new GameObject ("Walls");
		GameObject regFloorHolder = new GameObject ("Regular Floors");
		GameObject grabFloorHolder = new GameObject ("Grabbable Floors");
		GameObject lavaHolder = new GameObject ("Lava");

        wallHolder.transform.parent = levelHolder.transform;
        regFloorHolder.transform.parent = levelHolder.transform;
        grabFloorHolder.transform.parent = levelHolder.transform;
        lavaHolder.transform.parent = levelHolder.transform;

		// Go through the text file character by character to create the level.
		while (!streamReader.EndOfStream)
		{
			GameObject newChunk = new GameObject ("BIRD ANUS");

			string line = streamReader.ReadLine ();

			for (int i = 0; i < line.Length; i++)
			{
				Vector3 newChunkPosition = new Vector3 (
					levelPosX,
					0.0f,
					levelPosZ
				);

                if (line[i] == WALL_CHAR)
                {
                    // Walls should be raised slightly above other pieces.
                    newChunkPosition.y = wallHeight;

                    newChunk = (GameObject)Instantiate(wall);
                    newChunk.transform.parent = wallHolder.transform;
                }
                else if (line[i] == REGULAR_FLOOR_CHAR)
                {
                    newChunkPosition.y = floorHeight;
                    newChunk = (GameObject)Instantiate(regularFloor);
                    newChunk.transform.parent = regFloorHolder.transform;
                }
                else if (line[i] == GRABBABLE_FLOOR_CHAR)
                {
                    newChunkPosition.y = floorHeight;
                    newChunk = (GameObject)Instantiate(grabbableFloor);
                    newChunk.transform.parent = grabFloorHolder.transform;
                }
                else if (line[i] == LAVA_CHAR)
                {
                    newChunkPosition.y = lavaHeight;
                    newChunk = (GameObject)Instantiate(lava);
                    newChunk.transform.parent = lavaHolder.transform;
                }
                else if (line[i] == PLAYER_START_CHAR)
                {
                    // Instantiate a regular floor.
                    newChunkPosition.y = floorHeight;
                    newChunk = (GameObject)Instantiate(regularFloor);
                    newChunk.transform.parent = regFloorHolder.transform;

                    // Instantiate the player.
                    GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                    playerObj.transform.root.position = new Vector3(newChunkPosition.x, 1f, newChunkPosition.z-0.5f);
                   

                }
                else if (line[i] == SKELETON_START_CHAR)
                {
                    // Instantiate a regular floor.
                    newChunkPosition.y = floorHeight;
                    newChunk = (GameObject)Instantiate(regularFloor);
                    newChunk.transform.parent = regFloorHolder.transform;

                    // Instantiate the skeleton.
                    GameObject skelly = (GameObject)Instantiate(skeleton);
                    skelly.transform.position = new Vector3(newChunkPosition.x, 1f, newChunkPosition.z);
                    skelly.transform.parent = levelHolder.transform;
                }

				// Set the position of the new chunk.
				newChunk.transform.position = newChunkPosition;

				// Get the position of the next chunk.
				levelPosX += chunkSize;
			}

			levelPosX = 0f;
			levelPosZ -= chunkSize;
		}

        // Delete all rogue game objects. (Hacky solution because this script was generating blank game objects & I can't figure out why or be bothered to care.
        GameObject[] fuckEverything = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject penis in fuckEverything)
        {
            if (penis.name == "BIRD ANUS")
            {
                DestroyImmediate(penis);
            }
        }
	}
}
