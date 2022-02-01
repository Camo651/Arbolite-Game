using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
	public List<ContainedRoom> baseRooms = new List<ContainedRoom>();
	public long globalUpdateID;
	public PlayerState currentPlayerState;
	public enum PlayerState
	{
		Player,
		BuildMode,
		DestroyMode
	}

	public RoomTile GetRoomAtPosition(Vector2Int pos)
	{
		foreach(ContainedRoom cont in baseRooms)
		{
			foreach (RoomTile room in cont.containedRooms)
			{
				if (room.GetTrueTilePosision().x == pos.x && room.GetTrueTilePosision().y == pos.y)
				{
					return room;
				}
			}
		}
		return null;
	}
}
