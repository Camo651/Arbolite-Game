using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
	[HideInInspector] public GlobalRefManager globalRefManager;
	public ContainedRoom Dirt, Bedrock;
	public SO_TileType DirtSmall, DirtLeft, DirtRight;
	public int terrainWidth, terrainBottomLayer, terrainRaiseBuffer, terrainPerlinAmplitude;
	public float terrainFalloffThresh, terrainFalloffAmp;
	public int timeOfDay, dayCycleLength, dayCount;
	public float timeOfDayNormalized;
	public List<GameObject> backgroundLayers;
	public float[] backgroundParallaxScales;
	private List<List<SpriteRenderer>> backgroundSprites;
	public Gradient dayNightCycleTints;
	public Color[] parallaxLayerBaseColours;
	public GameObject backgroundFullTint;

	public void Start()
	{
		backgroundSprites = new List<List<SpriteRenderer>>();
		foreach (GameObject layer in backgroundLayers)
		{
			backgroundSprites.Add(new List<SpriteRenderer>());
			for (int i = 0; i < layer.transform.childCount; i++)
			{
				backgroundSprites[backgroundSprites.Count - 1].Add(layer.transform.GetChild(i).GetComponent<SpriteRenderer>());
			}
		}

		GenerateTerrain();

		globalRefManager.cameraController.cameraBounds.x = terrainWidth / -2f;
		globalRefManager.cameraController.cameraBounds.y = terrainWidth / 2f;
	}
	public void GenerateTerrain()
	{
		globalRefManager.baseManager.roomIndexingVectors = new List<List<RoomTile>>();

		for (int x = -(terrainWidth/2); x < terrainWidth/2; x++)
		{
			Vector2Int pos = Vector2Int.right * x;
			pos.y = terrainRaiseBuffer + (int)(terrainPerlinAmplitude*Mathf.PerlinNoise((x * .24321f) - 0.51f, 1.123f));
			if(Mathf.Abs(x) > (terrainWidth / terrainFalloffThresh))
			{
				pos.y -=  (int)(terrainFalloffAmp * (Mathf.Abs(x) - (terrainWidth / terrainFalloffThresh)));
			}
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
		foreach(ContainedRoom tile in globalRefManager.baseManager.baseRooms)
		{
			if (tile.isNaturalTerrainTile)
			{
				tile.containedRooms[0].UpdateTile();
			}
		}
	}

	private void PeripheralWorldHandle()
	{
		timeOfDay++;
		if (timeOfDay >= dayCycleLength)
		{
			timeOfDay = 0;
			dayCount++;
		}
		timeOfDayNormalized = timeOfDay / (float)dayCycleLength;
		globalRefManager.cameraController.mainCamera.backgroundColor = dayNightCycleTints.Evaluate(timeOfDayNormalized);
		for (int layer = 0; layer < backgroundSprites.Count; layer++)
		{
			backgroundLayers[layer].transform.position = new Vector3(globalRefManager.cameraController.mainCamera.transform.position.x / backgroundParallaxScales[layer],
				(globalRefManager.cameraController.mainCamera.transform.position.y - globalRefManager.cameraController.cameraBounds.z - globalRefManager.cameraController.mainCamera.orthographicSize) / (backgroundParallaxScales[layer] * 1.25f), 0f);
			for (int i = 0; i < backgroundSprites[layer].Count; i++)
			{
				if (backgroundSprites[layer][i].sprite.name == "WhiteTriangleHalf")
				{
					backgroundSprites[layer][i].color = parallaxLayerBaseColours[layer] * dayNightCycleTints.Evaluate(timeOfDayNormalized);
				}
				else
				{
					backgroundSprites[layer][i].color = parallaxLayerBaseColours[layer];
				}
			}
		}
		backgroundFullTint.transform.position = globalRefManager.cameraController.transform.position-Vector3.back*10f;
	}

	private void FixedUpdate()
	{
		PeripheralWorldHandle();
	}
}
