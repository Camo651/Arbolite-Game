using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
	//generates the initial map and handles all 'world' stuff

	[HideInInspector] public GlobalRefManager globalRefManager;
	public ContainedRoom Dirt, Bedrock;
	public Sprite BedrockSprite, DirtFullSprite, DirtSmallSprite, DirtLeftSprite, DirtRightSprite;
	public int terrainWidth, terrainBottomLayer, terrainRaiseBuffer, terrainPerlinAmplitude;
	public float terrainFalloffThresh, terrainFalloffAmp;
	public float timeOfDay, dayCycleLength, dayCount;
	public float timeOfDayNormalized;
	public List<GameObject> backgroundLayers;
	public float[] backgroundParallaxScales;
	private List<List<SpriteRenderer>> backgroundSprites;
	public Gradient dayNightCycleTints;
	public Color[] parallaxLayerBaseColours;
	public Color skytint;
	public GameObject sunmoon;


	public void Start()
	{
		//index all the background parallax layers
		backgroundSprites = new List<List<SpriteRenderer>>();
		foreach (GameObject layer in backgroundLayers)
		{
			backgroundSprites.Add(new List<SpriteRenderer>());
			for (int i = 0; i < layer.transform.childCount; i++)
			{
				backgroundSprites[backgroundSprites.Count - 1].Add(layer.transform.GetChild(i).GetComponent<SpriteRenderer>());
			}
		}
		globalRefManager.cameraController.cameraBounds.x = terrainWidth / -2f;
		globalRefManager.cameraController.cameraBounds.y = terrainWidth / 2f;
		GenerateTerrain();

	}

	//generate the initial tilemap with terrain tiles
	public void GenerateTerrain()
	{
		globalRefManager.baseManager.roomIndexingVectors = new List<List<RoomTile>>();

		for (int x = -(terrainWidth/2); x < terrainWidth/2; x++)
		{
			Vector2Int pos = Vector2Int.right * x;
			pos.y = terrainRaiseBuffer + (int)(terrainPerlinAmplitude*Mathf.PerlinNoise((x * .24321f) - 0.51f, 1.123f));

			//if the tile is well inside the terrain space
			if(Mathf.Abs(x) > (terrainWidth / terrainFalloffThresh))
			{
				pos.y -=  (int)(terrainFalloffAmp * (Mathf.Abs(x) - (terrainWidth / terrainFalloffThresh)));
			}

			//generate stone under the dirt if there is space to do so
			if (pos.y >= terrainBottomLayer)
			{
				globalRefManager.baseManager.TryCreateRoomAtPos(pos, Dirt);
				while (pos.y > terrainBottomLayer)
				{
					pos.y--;
					globalRefManager.baseManager.TryCreateRoomAtPos(pos, Bedrock);
				}
			}
		}

		//update the tiles to update the textured based on the surrounding blocks
		foreach(ContainedRoom tile in globalRefManager.baseManager.baseRooms)
		{
			if (tile.isNaturalTerrainTile)
			{
				tile.containedRooms[0].UpdateTile(true);
			}
		}
	}

	//handles the flow of time and ambiance
	private void PeripheralWorldHandle()
	{
		//increment time based on the update cycle
		timeOfDay += Time.deltaTime;
		if (timeOfDay >= dayCycleLength)
		{
			timeOfDay = 0;
			dayCount++;
		}
		timeOfDayNormalized = timeOfDay / dayCycleLength;

		Color bkgCol = dayNightCycleTints.Evaluate(timeOfDayNormalized);
		bkgCol.a = 1f;
		//set the background colour based on the time of day
		if (globalRefManager.settingsManager.doDaynightCycle)
		{
			globalRefManager.cameraController.mainCamera.backgroundColor = bkgCol + skytint;
		}

		//update the position of the background layers based on the position of the camera to make the parallax effect
		for (int layer = 0; layer < backgroundSprites.Count; layer++)
		{
			if(globalRefManager.settingsManager.parallaxBackground)
				backgroundLayers[layer].transform.localPosition = new Vector3(globalRefManager.cameraController.mainCamera.transform.position.x / backgroundParallaxScales[layer],
				(globalRefManager.cameraController.mainCamera.transform.position.y - globalRefManager.cameraController.cameraBounds.z - globalRefManager.cameraController.mainCamera.orthographicSize) / (backgroundParallaxScales[layer] * 1.25f), 0f);

			//update the bkg colour layers based on the time of day
			if(globalRefManager.settingsManager.doDaynightCycle)
				for (int i = 0; i < backgroundSprites[layer].Count; i++)
				{
					backgroundSprites[layer][i].color = Color.Lerp(bkgCol, parallaxLayerBaseColours[layer], .2f);
				}
		}

		//rotate the sun - moon pivot axis to increments time of day (tuning done by sun/mon normal pos from pivot in editor)
		if (globalRefManager.settingsManager.doDaynightCycle)
		{
			sunmoon.transform.localEulerAngles = Vector3.forward * -360 * timeOfDayNormalized;
			sunmoon.transform.position = (Vector3.right * globalRefManager.cameraController.mainCamera.transform.position.y / 11f) + (Vector3.up * globalRefManager.cameraController.mainCamera.transform.position.y / 10f); ;
		}
	}

	private void Update()
	{
		PeripheralWorldHandle();
	}
}
