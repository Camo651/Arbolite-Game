using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTile : MonoBehaviour
{
	private Color[] nodeColourStates = { Color.yellow, Color.green, Color.red, Color.blue };
	[Header("0-Meh|1-Alw|2-Nev|3-Aes")]
	[Range(0, 3)] public int topNodeState;
	[Range(0,3)] public int rightNodeState, bottomNodeState, leftNodeState;
	public ContainedRoom roomContainer;
	public RoomTile[] neighborRooms;
	public bool[] neighborWelds; //up right down left
	public long previousUpdateID;
	public SpriteRenderer spriteRenderer;

	[HideInInspector] public readonly Vector2Int[] offsets = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
	[HideInInspector] public readonly int[] inverseOffsets = { 2, 3, 0, 1 };

	// updates the values for this tile based on its conditions
	public void UpdateTile(bool updateNeighbors)
	{
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

		//send the update pulse to the neighbors if permits
		if (updateNeighbors)
			UpdateNeighboringTiles();

		//as it stands: natural tiles get smoothed textures
		if (roomContainer.isNaturalTerrainTile)
		{
			Sprite newTile;
			switch (neighborRooms[0] != null, neighborRooms[1] != null, neighborRooms[2] != null, neighborRooms[3] != null)
			{
				case (false, true, true, false): newTile = roomContainer.globalRefManager.terrainManager.DirtRightSprite; break;
				case (false, false, true, true): newTile = roomContainer.globalRefManager.terrainManager.DirtLeftSprite; break;
				case (false, false, true, false): newTile = roomContainer.globalRefManager.terrainManager.DirtSmallSprite; break;
				default: newTile = roomContainer.globalRefManager.terrainManager.DirtFullSprite; break;
			}
			if (neighborRooms[0] != null)
				newTile = roomContainer.globalRefManager.terrainManager.BedrockSprite;
			spriteRenderer.sprite = newTile;
		}
	}

	//updates the 4 cardinal tiles around it (if they exist) and updates their values. Does not recursively flood the updates
	public void UpdateNeighboringTiles()
	{
		//connection w/ neighbors established earlier
		for (int i = 0; i < 4; i++)
		{
			if(neighborRooms[i] != null)
				neighborRooms[i].UpdateTile(false);
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
	}
}

