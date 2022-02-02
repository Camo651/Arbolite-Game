using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
	[HideInInspector] public GlobalRefManager globalRefManager;
	public ContainedRoom Dirt, Bedrock;
	public int terrainWidth, terrainBottomLayer;

	public void Start()
	{
		GenerateTerrain();
	}
	public void GenerateTerrain()
	{
		for (int x = 0; x < terrainWidth; x++)
		{

			Vector2Int pos = Vector2Int.right * (x-(terrainWidth/2));
			pos.y = (int)(10f*Mathf.PerlinNoise((x * .1f) - 0.51f, 1.123f));
			pos.y += (int)(5f*Mathf.Cos((Mathf.PI / 1) * ((x-(terrainWidth/2f)) / terrainWidth)));
			globalRefManager.baseManager.TryCreateRoomAtPos(pos, Dirt);
			while(pos.y > terrainBottomLayer)
			{
				pos.y--;
				globalRefManager.baseManager.TryCreateRoomAtPos(pos, Bedrock);
			}
		}
	}
}
