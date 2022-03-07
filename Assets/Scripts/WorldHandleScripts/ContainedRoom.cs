using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainedRoom : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	[HideInInspector]public Vector2Int roomDimensions;
	public bool activeAndEnabled;
	public bool isNaturalTerrainTile;
	public string tileNameCallbackID;
	public string tileNameInfoID;
	public List<RoomTile> containedRooms;
	public RoomTile rotorRoom;
	public ItemContainer itemContainer;
	public List<SO_Property> properties;


	public void SetRoomTint(Color tint)
	{ // sets the colour of all the tiles in the room
		foreach (RoomTile tile in containedRooms)
		{
			tile.spriteRenderer.color = tint;
		}

	}

	private void Awake()
	{
		itemContainer = GetComponent<ItemContainer>();
	}
}