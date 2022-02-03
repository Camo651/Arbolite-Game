using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
	[HideInInspector]public GlobalRefManager globalRefManager;
	public List<ContainedRoom> baseRooms = new List<ContainedRoom>();
	public List<List<RoomTile>> roomIndexingVectors; //instantiated in TerrainManager after world is generated
	public long globalUpdateID;
	public PlayerState currentPlayerState;
	public GameObject currentlySelectedRoom;
	public Color placementColourAllow, placementColourDeny;
	public string selectedRoom;
	public enum PlayerState
	{
		Player,
		BuildMode,
		EditMode,
		DestroyMode
	}

	private void Update()
	{
		foreach (var item in roomIndexingVectors)
		{
			foreach (var gaeg in item)
			{
				print(gaeg);
			}
			print("--------");
		}
		if(currentPlayerState == PlayerState.BuildMode)
		{
			if(currentlySelectedRoom.transform.childCount == 0)
			{
				currentlySelectedRoom.gameObject.SetActive(true);
				ContainedRoom cr = Instantiate(globalRefManager.contentManager.GetRoomPrefabByName(selectedRoom));
				cr.globalRefManager = globalRefManager;
				cr.transform.SetParent(currentlySelectedRoom.transform);
				cr.activeAndEnabled = false;
				cr.transform.position = currentlySelectedRoom.transform.position;
			}
			else
			{
				Vector3 mousePos = globalRefManager.cameraController.mainCamera.ScreenToWorldPoint(Input.mousePosition);

				if (currentlySelectedRoom.transform.position.x != Mathf.Round(mousePos.x) && currentlySelectedRoom.transform.position.y != Mathf.Round(mousePos.y))
				{
					currentlySelectedRoom.transform.position = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), 0f);

					bool colliding = false, nodeConditionsMet = true;
					foreach (RoomTile room in currentlySelectedRoom.transform.GetChild(0).GetComponent<ContainedRoom>().containedRooms)
					{
						room.spriteRenderer.sortingOrder = 10;
						int[] roomNodeStates = { room.tileType.topNodeState, room.tileType.rightNodeState, room.tileType.bottomNodeState, room.tileType.leftNodeState };
						for (int i = 0; i < 4; i++)
						{
							RoomTile other = GetRoomAtPosition(room.GetIndexdTilePosition() + room.offsets[i]);
							bool hasRoomOnLockedSide = other && roomNodeStates[i] == 2;
							bool connectorNodeMet = (roomNodeStates[i] == 1 && other) || roomNodeStates[i] != 1;
							bool otherHasLockedSideOnRoom = false;
							if (other != null)
							{

								int[] otherNodeStates = { other.tileType.bottomNodeState, other.tileType.leftNodeState, other.tileType.topNodeState, other.tileType.rightNodeState };
								if (otherNodeStates[i] == 2)
								{
									otherHasLockedSideOnRoom = true;
								}
							}

							if (hasRoomOnLockedSide || !connectorNodeMet || otherHasLockedSideOnRoom)
							{
								nodeConditionsMet = false;
								room.spriteRenderer.color = placementColourDeny;
							}
							else
							{
								room.spriteRenderer.color = placementColourAllow;
							}
						}
						if (nodeConditionsMet)
						{
							if (GetRoomAtPosition(room.GetIndexdTilePosition()) != null)
							{
								colliding = true;
								room.spriteRenderer.color = placementColourDeny;
							}
							else
							{
								room.spriteRenderer.color = placementColourAllow;
							}
						}
					}
					if (Input.GetMouseButtonDown(0) && !colliding && nodeConditionsMet)
					{
						TryCreateRoomAtPos(new Vector2Int(Mathf.RoundToInt(currentlySelectedRoom.transform.position.x), Mathf.RoundToInt(currentlySelectedRoom.transform.position.y)), globalRefManager.contentManager.GetRoomPrefabByName(selectedRoom));
						Destroy(currentlySelectedRoom.transform.GetChild(0).gameObject);
					}
				}
			}

		}
		else
		{
			if(currentlySelectedRoom.transform.childCount > 0)
			{
				Destroy(currentlySelectedRoom.transform.GetChild(0).gameObject);
			}
		}
	}

	public RoomTile GetRoomAtPosition(Vector2Int pos)
	{
		//foreach(ContainedRoom cont in baseRooms)
		//{
		//	foreach (RoomTile room in cont.containedRooms)
		//	{
		//		if (room.GetTrueTilePosition().x == pos.x && room.GetTrueTilePosition().y == pos.y)
		//		{
		//			return room;
		//		}
		//	}
		//}
		//return null;

		if(roomIndexingVectors.Count > pos.y && pos.y >=0 && roomIndexingVectors[pos.y].Count > pos.x && pos.x>=0)
		{
			return roomIndexingVectors[pos.y][pos.x];
		}
		else
		{
			return null;
		}

	}

	public void TryCreateRoomAtPos(Vector2Int pos, ContainedRoom roomPrefab)
	{
		ContainedRoom newGen = Instantiate(roomPrefab);
		baseRooms.Add(newGen);
		newGen.globalRefManager = globalRefManager;
		newGen.transform.position = new Vector3(pos.x, pos.y, 0f);
		newGen.activeAndEnabled = true;
		globalUpdateID++;
		foreach (RoomTile tile in newGen.containedRooms)
		{
			while(roomIndexingVectors.Count <= tile.GetIndexdTilePosition().y)
			{
				roomIndexingVectors.Add(new List<RoomTile>());
				for (int i = 0; i < globalRefManager.terrainManager.terrainWidth; i++)
					roomIndexingVectors[roomIndexingVectors.Count - 1].Add(null);
			}
			roomIndexingVectors[tile.GetIndexdTilePosition().y][tile.GetIndexdTilePosition().x] = tile;
		}
		foreach (RoomTile tile in newGen.containedRooms)
		{
			tile.UpdateNeighboringTiles(globalUpdateID);
			tile.UpdateTile();
		}
	}
}

//TRY CREATE ROOM IS ALWAYS ADDING ROOM 0, 0 TO THE ARRAY IN PLAXCE OF ALL OTHER ONES????
