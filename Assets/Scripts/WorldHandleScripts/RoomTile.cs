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
	public List<SO_Property> properties;
	public bool canHavePlant, canHaveMech;
	public PlantObject thisRoomsPlant;
	public Rotor thisRoomsRotor;

	[HideInInspector] public readonly Vector2Int[] offsets = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
	[HideInInspector] public readonly int[] inverseOffsets = { 2, 3, 0, 1 };
	private readonly Color[] nodeColourStates = { Color.yellow, Color.green, Color.red, Color.blue };

	/// <summary>
	/// Generates the naturally generated elements of the tile
	/// </summary>
	public void StartGeneration()
	{
		childNodes = new List<Node>(GetComponentsInChildren<Node>());
		if(childNodes.Count > 0 && Random.value<1 && canHavePlant)
		{
			PlantObject p = roomContainer.globalRefManager.plantManager.GeneratePlant(roomContainer.globalRefManager.plantManager.GetRandomTreePresetWeighted(null).plantProperties, this);
			p.transform.SetParent(childNodes[0].transform);
			p.transform.localPosition = Vector3.zero;
			thisRoomsPlant = p;
			p.roomTile = this;
		}
	}


	public void Init()
	{
		thisRoomsRotor = GetComponent<Rotor>();
		if (thisRoomsRotor)
		{
			roomContainer.rotorRoom = this;
			thisRoomsRotor.UpdateSystemEnergy();
		}
	}


	/// <summary>
	/// Updates the values for this tile based on its current conditions. Also sets the neighboring rooms
	/// </summary>
	/// <param name="updateNeighbors">Should the 4 neighboring tiles have this method called on them?</param>
	/// <param name="updateOrigin">Where this update pulse is originating from (use obj hashes)</param>
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

	/// <summary>
	/// Changes a room to be of type s
	/// </summary>
	/// <param name="s">The callback id of the room to be set to</param>
	private void ChangeRoomTo(string s)
	{
		if(roomContainer.tileNameCallbackID != s)
			roomContainer.globalRefManager.baseManager.ChangeRoom(GetTrueTilePosition(), roomContainer.globalRefManager.baseManager.GetRoomPrefab(s));
	}

	/// <summary>
	/// updates the 4 cardinal tiles around it (if they exist) and updates their values. Does not recursively flood the updates
	/// </summary>
	/// <param name="iter">The iteration of the pulse</param>
	/// <param name="ID">The ID of the origin of the pulse</param>
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


	/// <summary>
	/// Get the tile's position in real world space
	/// </summary>
	/// <returns>The tiles worldspace position</returns>
	public Vector2Int GetTrueTilePosition()
	{
		return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
	}

	/// <summary>
	/// Get the tiles position in the storage matrix
	/// </summary>
	/// <returns>The tiles position in matrix space</returns>
	public Vector2Int GetIndexdTilePosition()
	{
		return new Vector2Int(Mathf.RoundToInt(transform.position.x + (roomContainer.globalRefManager.terrainManager.terrainWidth / 2)), Mathf.RoundToInt(transform.position.y + roomContainer.globalRefManager.terrainManager.terrainBottomLayer));
	}

	public void OnDrawGizmos()
	{
		float dist = .7f, rad = .08f;
		Gizmos.color = neighborWelds[0]?Color.black:nodeColourStates[topNodeState];
		Gizmos.DrawSphere(Vector3.up*dist + transform.position, rad);

		Gizmos.color = neighborWelds[1] ? Color.black : nodeColourStates[rightNodeState];
		Gizmos.DrawSphere(Vector3.right* dist + transform.position, rad);

		Gizmos.color = neighborWelds[2] ? Color.black : nodeColourStates[bottomNodeState];
		Gizmos.DrawSphere(Vector3.down* dist + transform.position, rad);

		Gizmos.color = neighborWelds[3] ? Color.black : nodeColourStates[leftNodeState];
		Gizmos.DrawSphere(Vector3.left* dist + transform.position, rad);

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, Vector2.one);
	}
}

