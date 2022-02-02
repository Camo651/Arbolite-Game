using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainedRoom : MonoBehaviour
{
	[HideInInspector]public Vector2Int roomDimensions;
	[HideInInspector]public string ContainedRoomName;
	public BaseManager baseManager;
	public List<RoomTile> containedRooms;
}