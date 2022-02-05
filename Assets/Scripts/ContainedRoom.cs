using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainedRoom : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	[HideInInspector]public Vector2Int roomDimensions;
	[HideInInspector]public string ContainedRoomName;
	public bool activeAndEnabled;
	public bool isNaturalTerrainTile;
	public List<RoomTile> containedRooms;


	public void SetRoomTint(Color tint)
	{
		foreach (RoomTile tile in containedRooms)
		{
			tile.spriteRenderer.color = tint;
		}

	}
}