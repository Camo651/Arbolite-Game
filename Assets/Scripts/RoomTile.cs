using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTile : MonoBehaviour
{
	[Header("0-Meh|1-Alw|2-Nev|3-Aes")]
	[Range(0, 3)] public int topNodeState;
	[Range(0,3)] public int rightNodeState, bottomNodeState, leftNodeState;
	public ContainedRoom roomContainer;
	public RoomTile[] neighborRooms;
	public bool[] neighborWelds; //up right down left
	public string lastUpdateOrigin;
	public SpriteRenderer spriteRenderer;
	public List<Node> childNodes;
	public bool canBeUpdated;


	[HideInInspector] public readonly Vector2Int[] offsets = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
	[HideInInspector] public readonly int[] inverseOffsets = { 2, 3, 0, 1 };
	private readonly Color[] nodeColourStates = { Color.yellow, Color.green, Color.red, Color.blue };


	public void StartGeneration()
	{
		childNodes = new List<Node>(GetComponentsInChildren<Node>());
		if(childNodes.Count > 0 && Random.value<.5f)
		{
			SO_TreePreset preset = roomContainer.globalRefManager.plantManager.GetTreePreset("Default");
			Dictionary<PlantPart.PartType, Color> colours = new Dictionary<PlantPart.PartType, Color>();
			for(int i = 0; i < preset._partTypes.Count; i++)
			{
				colours.Add(preset._partTypes[i], preset._biome.biomeColourPalette[(int)preset._partColours[i].x].Evaluate(preset._partColours[i].y));
			}
			ProceduralPlant plant = roomContainer.globalRefManager.plantManager.GenerateNewPlant(childNodes[0],this, preset._plantType, preset._partTypes, preset._biome, preset.GetResourceDistr(), colours);
			//set the plants pos to be linked to the nodes here
		}
	}


	// updates the values for this tile based on its conditions
	public void UpdateTile(bool updateNeighbors, string updateOrigin)
	{
		//guard clause to prevent unwanted updates
		if (!canBeUpdated)
			return;
		//set the current room's sprite
		spriteRenderer = GetComponent<SpriteRenderer>();

		//re-establish connection with the surrounding tiles
		neighborRooms = new RoomTile[4];
		for (int i = 0; i < 4; i++)
		{
			RoomTile other = roomContainer.globalRefManager.baseManager.GetRoomAtPosition(GetTrueTilePosition() + offsets[i]);
			if (other != null)
			{
				other.neighborRooms[inverseOffsets[i]] = this;
				neighborRooms[i] = other;
			}
		}

		//as it stands: natural tiles get smoothed textures
		if (roomContainer.isNaturalTerrainTile && updateOrigin != lastUpdateOrigin)
		{
			lastUpdateOrigin = updateOrigin;
			if(neighborRooms[0] != null)
			{
				switch (neighborRooms[0] != null ? neighborRooms[0].roomContainer.tileNameInfoID:"",
						neighborRooms[1] != null ? neighborRooms[1].roomContainer.tileNameInfoID:"",
						neighborRooms[2] != null ? neighborRooms[2].roomContainer.tileNameInfoID:"",
						neighborRooms[3] != null ? neighborRooms[3].roomContainer.tileNameInfoID:"")
				{
					case ("tile_grass", "tile_grass", "tile_bedrock", "tile_grass"):
						//small
						ChangeRoomTo("tile_bedrock_small");break;
					case ("tile_grass", "tile_grass", "tile_bedrock", "tile_bedrock"):
						//right
						ChangeRoomTo("tile_bedrock_right");break;
					case ("tile_grass", "tile_bedrock", "tile_bedrock", "tile_grass"):
						//left
						ChangeRoomTo("tile_bedrock_left");break;
					default:
						//full
						ChangeRoomTo("tile_bedrock_full");break;
				}
			}
			else
			{
				switch (neighborRooms[0] != null, neighborRooms[1] != null, neighborRooms[2] != null, neighborRooms[3] != null)
				{
					case (false, true, true, true):
						//full grass
						ChangeRoomTo("tile_grass_full");break;
					case (false, false, true, true):
						//left grass
						ChangeRoomTo("tile_grass_left");break;
					case (false, true, true, false):
						//right grass
						ChangeRoomTo("tile_grass_right");break;
					case (false, false, true, false):
						//right grass
						ChangeRoomTo("tile_grass_small");break;
				}
			}
		}


		//send the update pulse to the neighbors if permits
		if (updateNeighbors)
			UpdateNeighboringTiles(0, updateOrigin);
	}

	private void ChangeRoomTo(string s)
	{
		if(roomContainer.tileNameCallbackID != s)
			roomContainer.globalRefManager.baseManager.ChangeRoom(GetTrueTilePosition(), roomContainer.globalRefManager.baseManager.GetRoomPrefab(s));
	}

	//updates the 4 cardinal tiles around it (if they exist) and updates their values. Does not recursively flood the updates
	public void UpdateNeighboringTiles(int iter, string ID)
	{
		//connection w/ neighbors established earlier
		for (int i = 0; i < 4; i++)
		{
			if(neighborRooms[i] != null && neighborRooms[i] != this)
				neighborRooms[i].UpdateTile(false, ID);
		}
		if (iter <= 0)
		{
			for (int i = 0; i < 4; i++)
			{
				if (neighborRooms[i] != null && neighborRooms[i] != this)
					neighborRooms[i].UpdateNeighboringTiles(iter + 1, ID);
			}
		}
	}


	//returns the tile's position in real world space
	public Vector2Int GetTrueTilePosition()
	{
		return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
	}

	//returns the tile's position in matrix indexed space
	public Vector2Int GetIndexdTilePosition()
	{
		return new Vector2Int(Mathf.RoundToInt(transform.position.x + (roomContainer.globalRefManager.terrainManager.terrainWidth / 2)), Mathf.RoundToInt(transform.position.y + roomContainer.globalRefManager.terrainManager.terrainBottomLayer));
	}

	public void OnDrawGizmos()
	{
		float dist = .7f, rad = .08f;
		Gizmos.color = neighborWelds[0]?Color.black:nodeColourStates[topNodeState];
		Gizmos.DrawSphere(Vector3.up*dist, rad);

		Gizmos.color = neighborWelds[1] ? Color.black : nodeColourStates[rightNodeState];
		Gizmos.DrawSphere(Vector3.right*dist, rad);

		Gizmos.color = neighborWelds[2] ? Color.black : nodeColourStates[bottomNodeState];
		Gizmos.DrawSphere(Vector3.down*dist, rad);

		Gizmos.color = neighborWelds[3] ? Color.black : nodeColourStates[leftNodeState];
		Gizmos.DrawSphere(Vector3.left*dist, rad);

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(Vector2.zero, Vector2.one);
	}
}

