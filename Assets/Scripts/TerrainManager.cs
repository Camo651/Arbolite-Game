using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
	[HideInInspector] public GlobalRefManager globalRefManager;
	public ContainedRoom Dirt, Bedrock;
	public SO_TileType DirtSmall, DirtLeft, DirtRight;
	public int terrainWidth, terrainBottomLayer;
	public List<GameObject> backgroundLayers;

	public void Start()
	{
		GenerateTerrain();
	}
	public void GenerateTerrain()
	{
		globalRefManager.baseManager.roomIndexingVectors = new List<List<RoomTile>>();

		for (int x = -(terrainWidth/2); x < terrainWidth/2; x++)
		{
			Vector2Int pos = Vector2Int.right * x;
			pos.y = (int)(10f*Mathf.PerlinNoise((x * .24321f) - 0.51f, 1.123f));
			pos.y += (int)(15f*Mathf.Cos((Mathf.PI) * x / terrainWidth));
			globalRefManager.baseManager.TryCreateRoomAtPos(pos, Dirt);
			while(pos.y > terrainBottomLayer)
			{
				pos.y--;
				globalRefManager.baseManager.TryCreateRoomAtPos(pos, Bedrock);
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

	private void LateUpdate()
	{
		
	}
}
