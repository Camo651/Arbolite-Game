using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
	[HideInInspector]public GlobalRefManager globalRefManager;
	public List<ContainedRoom> baseRooms = new List<ContainedRoom>();
	public long globalUpdateID;
	public PlayerState currentPlayerState;
	public GameObject currentlySelectedRoom;
	public Color placementColourAllow, placementColourDeny;
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
			string selectedRoom = "Complex Room";
			if(currentlySelectedRoom.transform.childCount == 0)
			{
				currentlySelectedRoom.gameObject.SetActive(true);
				ContainedRoom cr = Instantiate(globalRefManager.contentManager.GetRoomPrefabByName(selectedRoom));
				cr.transform.SetParent(currentlySelectedRoom.transform);
				cr.activeAndEnabled = false;
				cr.transform.position = currentlySelectedRoom.transform.position;
			}
			else
			{
				Vector3 mousePos = globalRefManager.cameraController.mainCamera.ScreenToWorldPoint(Input.mousePosition);
				currentlySelectedRoom.transform.position = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), 0f);

				bool colliding = false;
				foreach (RoomTile room in currentlySelectedRoom.transform.GetChild(0).GetComponent<ContainedRoom>().containedRooms)
				{
					room.spriteRenderer.sortingOrder = 10;
					if(GetRoomAtPosition(room.GetTrueTilePosision()) != null)
					{
						colliding = true;
						room.spriteRenderer.color = placementColourDeny;
					}
					else
					{
						room.spriteRenderer.color = placementColourAllow;
					}

					//CHECK FOR NODE ATTACTCHMENT
				}
				if(Input.GetMouseButtonDown(0) && !colliding)
				{
					TryCreateRoomAtPos(new Vector2Int(Mathf.RoundToInt(currentlySelectedRoom.transform.position.x), Mathf.RoundToInt(currentlySelectedRoom.transform.position.y)), globalRefManager.contentManager.GetRoomPrefabByName(selectedRoom));
					Destroy(currentlySelectedRoom.transform.GetChild(0).gameObject);
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
		newGen.globalRefManager = globalRefManager;
		newGen.transform.position = new Vector3(pos.x, pos.y, 0f);
		newGen.activeAndEnabled = true;
		globalUpdateID++;
		foreach (RoomTile tile in newGen.containedRooms)
		{
			tile.UpdateNeighboringTiles(globalUpdateID);
			tile.UpdateTile();
		}
	}
}
