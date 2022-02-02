using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainedRoom : MonoBehaviour
{
	[HideInInspector] public GlobalRefManager globalRefManager;
	[HideInInspector]public Vector2Int roomDimensions;
	[HideInInspector]public string ContainedRoomName;
	public bool activeAndEnabled;
	public List<RoomTile> containedRooms;
}