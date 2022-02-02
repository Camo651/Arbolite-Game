using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
	public List<ContainedRoom> baseRooms = new List<ContainedRoom>();
	public long globalUpdateID;
	public PlayerState currentPlayerState;
	public ContainedRoom currentlySelectedRoom;
	public enum PlayerState
	{
		Player,
		BuildMode,
		DestroyMode
	}

	private void Update()
	{
		if(currentPlayerState == PlayerState.BuildMode)
		{
			
		}
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
	
	public void TryCreateRoomAtPos(Vector2Int pos, ContainedRoom roomPrefab)
	{
		ContainedRoom newGen = Instantiate(roomPrefab);
		baseRooms.Add(newGen);
		newGen.baseManager = this;
		newGen.transform.position = new Vector3(pos.x,pos.y,0f);
		newGen.containedRooms[0].UpdateTile();
		globalUpdateID++;
		newGen.containedRooms[0].UpdateNeighboringTiles(globalUpdateID);
	}
}
