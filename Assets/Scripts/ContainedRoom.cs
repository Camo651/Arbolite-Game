using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainedRoom : MonoBehaviour
{
 public GlobalRefManager globalRefManager;
	[HideInInspector]public Vector2Int roomDimensions;
	[HideInInspector]public string ContainedRoomName;
	public bool activeAndEnabled;
	public List<RoomTile> containedRooms;
}