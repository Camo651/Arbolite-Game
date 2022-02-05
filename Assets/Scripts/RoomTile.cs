using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTile : MonoBehaviour
{
	public SO_TileType tileType;
	public ContainedRoom roomContainer;
	public RoomTile[] neighborRooms;
	public bool[] neighborWelds; //up right down left
	public long previousUpdateID;
	public SpriteRenderer spriteRenderer;

	[HideInInspector] public readonly Vector2Int[] offsets = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
	[HideInInspector] public readonly int[] inverseOffsets = { 2, 3, 0, 1 };
	public void UpdateTile()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = tileType.backgroundSprite;

		UpdateNeighboringTiles();
	}
	public void UpdateNeighboringTiles()
	{
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
		if (roomContainer.isNaturalTerrainTile)
		{
			SO_TileType newTile;
			switch(neighborRooms[0] != null, neighborRooms[1] != null, neighborRooms[2] != null, neighborRooms[3] != null)
			{
				case (false, true, true, false) : newTile = roomContainer.globalRefManager.terrainManager.DirtRight; break;
				case (false, false, true, true) : newTile = roomContainer.globalRefManager.terrainManager.DirtLeft; break;
				case (false, false, true, false) : newTile = roomContainer.globalRefManager.terrainManager.DirtSmall; break;
				default: newTile = roomContainer.globalRefManager.terrainManager.Dirt.containedRooms[0].tileType; break;
			}
			if (neighborRooms[0] != null)
				newTile = roomContainer.globalRefManager.terrainManager.Bedrock.containedRooms[0].tileType;
			tileType = newTile;
			spriteRenderer.sprite = newTile.backgroundSprite;
		}
	}

	public Vector2Int GetTrueTilePosition()
	{
		return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
	}
	public Vector2Int GetIndexdTilePosition()
	{
		return new Vector2Int(Mathf.RoundToInt(transform.position.x + (roomContainer.globalRefManager.terrainManager.terrainWidth / 2)), Mathf.RoundToInt(transform.position.y + roomContainer.globalRefManager.terrainManager.terrainBottomLayer));
	}
}

