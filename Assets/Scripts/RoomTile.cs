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
		//UpdateNeighboringTiles(0L);
	}
	public void UpdateNeighboringTiles(long updateID)
	{
		neighborRooms = new RoomTile[4];
		previousUpdateID = updateID;
		for (int i = 0; i < 4; i++)
		{
			RoomTile rt = roomContainer.globalRefManager.baseManager.GetRoomAtPosition(GetTrueTilePosision() + offsets[i]);
			if (rt != null)
			{
				rt.neighborRooms[inverseOffsets[i]] = this;
				neighborRooms[i] = rt;
			}
		}
	}

	public Vector2Int GetTrueTilePosision()
	{
		return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
	}
}

