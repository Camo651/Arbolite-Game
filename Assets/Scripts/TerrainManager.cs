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
	public Vector2[] backgroundParallaxScales;
	private List<List<SpriteRenderer>> backgroundSprites;
	public Gradient dayNightCycleTints;
	public Color[] parallaxLayerBaseColours;
	public Color skytint;
	public UnityEngine.UI.Image tintOverlay;
	public GameObject sunmoon;
	public SO_BiomeType currentActiveBiome;
	public SO_BiomeType defaultBiomeType;
	public Dictionary<string, SO_BiomeType> allBiomes;
	public List<Sprite> cloudSprites;
	public GameObject cloudPrefab;
	public List<CloudObj> clouds;
	public int cloudCount;
	public float cloudHeightMin, cloudHeightMax;
	public float rightBound, leftBound;
	public float cloudiness;
	public float windspeed;

	public void Start()
	{
		allBiomes = new Dictionary<string, SO_BiomeType>();
		SO_BiomeType[] biomes = Resources.FindObjectsOfTypeAll<SO_BiomeType>();
		foreach(SO_BiomeType item in biomes)
		{
			if (allBiomes.ContainsKey(item.biomeNameCallbackID))
				allBiomes[item.biomeNameCallbackID] = item;
			else
				allBiomes.Add(item.biomeNameCallbackID, item);
		}

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
		leftBound = globalRefManager.cameraController.cameraBounds.x = terrainWidth / -2f;
		rightBound = -leftBound;
		globalRefManager.cameraController.cameraBounds.y = terrainWidth / 2f;
		GenerateTerrain();


		//generate clouds
		clouds = new List<CloudObj>();
		for (int i = 0; i < cloudCount; i++)
		{
			CloudObj c = new CloudObj();
			c.rend = Instantiate(cloudPrefab).GetComponent<SpriteRenderer>();
			c.layer = Random.value > .5f ? 0 : Random.value > .5 ? 1 : 2;
			c.rend.transform.SetParent(backgroundLayers[c.layer].transform);
			c.rend.sortingOrder = backgroundLayers[c.layer].transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder + 1;
			RandomizeCloud(c);
			c.rend.transform.position = new Vector3(leftBound + ((Mathf.Abs(leftBound) + Mathf.Abs(rightBound)) * (i / (float)cloudCount)), c.rend.transform.position.y, 0f);
			clouds.Add(c);
		}
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
			bkgCol.a = .3f;
			tintOverlay.color = bkgCol;
			bkgCol.a = 1;
		}

		//update the position of the background layers based on the position of the camera to make the parallax effect
		for (int layer = 0; layer < backgroundSprites.Count; layer++)
		{
			if(globalRefManager.settingsManager.parallaxBackground)
				backgroundLayers[layer].transform.localPosition = new Vector3(globalRefManager.cameraController.mainCamera.transform.position.x / backgroundParallaxScales[layer].x,
				(globalRefManager.cameraController.mainCamera.transform.position.y - globalRefManager.cameraController.cameraBounds.z - globalRefManager.cameraController.mainCamera.orthographicSize) / (backgroundParallaxScales[layer].y * 1.25f), 0f);

			//update the bkg colour layers based on the time of day
			if(globalRefManager.settingsManager.doDaynightCycle)
				for (int i = 0; i < backgroundSprites[layer].Count; i++)
				{
					backgroundSprites[layer][i].color = Color.Lerp(bkgCol, parallaxLayerBaseColours[layer], .4f);
				}
		}

		//rotate the sun - moon pivot axis to increments time of day (tuning done by sun/mon normal pos from pivot in editor)
		if (globalRefManager.settingsManager.doDaynightCycle)
		{
			sunmoon.transform.localEulerAngles = Vector3.forward * -360 * timeOfDayNormalized;
			sunmoon.transform.position = new Vector3(globalRefManager.cameraController.mainCamera.transform.position.x, globalRefManager.cameraController.mainCamera.transform.position.y-20, 0);
		}

		//move the clouds
		for(int i = 0; i< clouds.Count; i++)
		{
			CloudObj c = clouds[i];
			if(c.rend.transform.position.x > rightBound)
			{
				RandomizeCloud(c);
			}
			else
			{
				c.rend.transform.Translate(Vector3.right * c.speed * Time.deltaTime * windspeed);
			}
		}
	}

	public void RandomizeCloud(CloudObj c)
	{
		c.speed = (c.layer == 2 ? Random.Range(20, 70) : c.layer == 1 ? Random.Range(40, 90) : Random.Range(60, 110)) / 100f;
		c.active = Random.value < cloudiness;
		c.rend.sprite = cloudSprites[Random.Range(0, cloudSprites.Count)];
		c.rend.color = new Color(c.layer==0?.85f:c.layer==1?.9f:.95f, c.layer == 0 ? .85f : c.layer == 1 ? .9f : .95f, c.layer == 0 ? .95f : c.layer == 1 ? .98f : 1f, c.active ? .5f : 0f);
		float x = leftBound;
		float y = Random.Range(cloudHeightMin, cloudHeightMax);
		float s = (c.layer == 2 ? Random.Range(80, 130) : c.layer == 1 ? Random.Range(60, 110) : Random.Range(45, 80)) / 100f;
		c.rend.transform.localScale = Vector3.one * s;
		c.rend.transform.position = new Vector3(x, y, 0);
	}

	private void Update()
	{
		PeripheralWorldHandle();
	}

	public SO_BiomeType GetBiomeType(string callbackID)
	{
		return allBiomes.ContainsKey(callbackID) ? allBiomes[callbackID] : defaultBiomeType;
	}


}
public class CloudObj
{
	public SpriteRenderer rend;
	public float speed;
	public bool active;
	public int layer;
}
