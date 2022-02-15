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
	public string selectedRoomName;
	private bool colliding, nodeConditionsMet;
	public RoomTile editModeSelectedRoomTile, editModePermSelectedRoomTile;
	private Vector2Int currentSelectionCoords;
	public bool gameIsActivelyFrozen;
	public ContainedRoom[] roomsToDelete;
	Vector2 smoothCursorMovementDamp;
	public Dictionary<string, ContainedRoom> roomPrefabCatalog;

	public enum PlayerState
	{
		PlayerMode,
		BuildMode,
		EditMode
	}

	private void Awake()
	{
		ContainedRoom[] unsortedRooms = Resources.FindObjectsOfTypeAll<ContainedRoom>();
		roomPrefabCatalog = new Dictionary<string, ContainedRoom>();
		foreach (ContainedRoom item in unsortedRooms)
		{
			roomPrefabCatalog.Add(item.ContainedRoomName, item);
		}
	}

	//prepares and consolidates switching modes
	public void SetPlayerState(PlayerState state)
	{
		if (state != currentPlayerState)
		{
			globalRefManager.interfaceManager.SetWorldPositionViewerState(false, null);
			if (editModePermSelectedRoomTile != null)
				editModePermSelectedRoomTile.roomContainer.SetRoomTint(Color.white);
			editModePermSelectedRoomTile = null;
			if (editModeSelectedRoomTile != null)
				editModeSelectedRoomTile.roomContainer.SetRoomTint(Color.white);
			editModeSelectedRoomTile = null;

			currentPlayerState = state;
		}
	}


	//standard unity update cycle
	private void Update()
	{
		if (!gameIsActivelyFrozen && !globalRefManager.interfaceManager.userIsHoveredOnInterfaceElement)
		{
			//build mode is for making new rooms
			if (currentPlayerState == PlayerState.BuildMode)
			{
				if (selectedRoomName == "")
				{
					if (currentlySelectedRoom.transform.childCount > 0)
					{
						Destroy(currentlySelectedRoom.transform.GetChild(0).gameObject);
					}
				}
				else
				{
					//if there is no ghost room, make one
					if (currentlySelectedRoom.transform.childCount == 0)
					{
						currentlySelectedRoom.gameObject.SetActive(true);
						ContainedRoom cr = Instantiate(GetRoomPrefab(selectedRoomName));
						cr.globalRefManager = globalRefManager;
						cr.transform.SetParent(currentlySelectedRoom.transform);
						cr.activeAndEnabled = false;
						cr.transform.position = currentlySelectedRoom.transform.position;
					}
					else
					{
						//move the ghost room to the mouse position
						Vector3 mousePos = globalRefManager.cameraController.mainCamera.ScreenToWorldPoint(Input.mousePosition);
						if (currentlySelectedRoom.transform.position.x != Mathf.Round(mousePos.x) || currentlySelectedRoom.transform.position.y != Mathf.Round(mousePos.y))
						{
							float smoothX = Mathf.SmoothDamp(currentlySelectedRoom.transform.position.x, Mathf.Round(mousePos.x), ref smoothCursorMovementDamp.x, .025f);
							float smoothY = Mathf.SmoothDamp(currentlySelectedRoom.transform.position.y, Mathf.Round(mousePos.y), ref smoothCursorMovementDamp.y, .025f);
							currentlySelectedRoom.transform.position = new Vector3(smoothX, smoothY, 0f);

							colliding = false;
							nodeConditionsMet = true;
							ContainedRoom contRoom = currentlySelectedRoom.transform.GetChild(0).GetComponent<ContainedRoom>();
							// \/ checks if all the tiles in the room's conditions are met and the room can be placed there
							foreach (RoomTile room in contRoom.containedRooms)
							{
								room.spriteRenderer.sortingOrder = 10;
								int[] roomNodeStates = { room.tileType.topNodeState, room.tileType.rightNodeState, room.tileType.bottomNodeState, room.tileType.leftNodeState };
								for (int i = 0; i < 4; i++)
								{
									RoomTile other = GetRoomAtPosition(room.GetTrueTilePosition() + room.offsets[i]);
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
									if (GetRoomAtPosition(room.GetTrueTilePosition()) != null)
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
						}
						if (Input.GetMouseButtonDown(0) && !colliding && nodeConditionsMet) //place a copy of the ghost room at the postion if possible
						{
							globalRefManager.audioManager.Play("little_click");
							ContainedRoom cr = TryCreateRoomAtPos(new Vector2Int(Mathf.RoundToInt(currentlySelectedRoom.transform.position.x), Mathf.RoundToInt(currentlySelectedRoom.transform.position.y)), GetRoomPrefab(selectedRoomName));
							Destroy(currentlySelectedRoom.transform.GetChild(0).gameObject);
						}
					}
				}
			}
			else if (currentPlayerState == PlayerState.EditMode) //edit mode is to change values or delete rooms
			{
				if (currentlySelectedRoom.transform.childCount > 0)
				{
					Destroy(currentlySelectedRoom.transform.GetChild(0).gameObject);
				}
				else
				{
					//click on a room to perm select it
					if (Input.GetMouseButtonDown(0))
					{
						OnClickOnTile();
					}
					//move the hover tint around to the currently hivered room
					Vector3 mousePos = globalRefManager.cameraController.mainCamera.ScreenToWorldPoint(Input.mousePosition);
					if (currentSelectionCoords.x != Mathf.Round(mousePos.x) || currentSelectionCoords.y != Mathf.Round(mousePos.y))
					{
						currentSelectionCoords.x = Mathf.RoundToInt(mousePos.x);
						currentSelectionCoords.y = Mathf.RoundToInt(mousePos.y);
						RoomTile roomAtSelectionCoords = GetRoomAtPosition(currentSelectionCoords);
						if (roomAtSelectionCoords != editModeSelectedRoomTile)
						{
							if (editModeSelectedRoomTile != null && ((editModePermSelectedRoomTile == null) || (editModePermSelectedRoomTile && editModeSelectedRoomTile && editModePermSelectedRoomTile.roomContainer != editModeSelectedRoomTile.roomContainer)))
								editModeSelectedRoomTile.roomContainer.SetRoomTint(Color.white);
							editModeSelectedRoomTile = roomAtSelectionCoords;
							if (editModeSelectedRoomTile != null && ((editModePermSelectedRoomTile == null) || (editModePermSelectedRoomTile && editModeSelectedRoomTile && editModePermSelectedRoomTile.roomContainer != editModeSelectedRoomTile.roomContainer)))
								editModeSelectedRoomTile.roomContainer.SetRoomTint(Color.gray);
						}

					}
				}
			}
			else if (currentPlayerState == PlayerState.PlayerMode) //player mode is for the physical character walking around and doing stuff that cant be automated
			{
				if (currentlySelectedRoom.transform.childCount > 0)
				{
					Destroy(currentlySelectedRoom.transform.GetChild(0).gameObject);
				}
			}
		}
	}

	//Initiates the UI modal for confirming if the tiles should be deleted
	public void TryDestroyCurrentlySelectedTile()
	{
		if (editModePermSelectedRoomTile)
		{
			ContainedRoom contRoom = editModePermSelectedRoomTile.roomContainer;
			List<ContainedRoom> roomsThatWillBeDestroyed = new List<ContainedRoom>();
			roomsThatWillBeDestroyed.Add(contRoom);
			RoomsThatWillBeDestroyedOnCall(contRoom, roomsThatWillBeDestroyed, 0);
			string deletion = "";
			foreach (ContainedRoom c in roomsThatWillBeDestroyed)
			{
				deletion += c.containedRooms[0].tileType.tileTypeName + ", ";
			}
			deletion = deletion.Substring(0, deletion.Length - 2);
			globalRefManager.interfaceManager.SetMajorInterface("ConfirmDelete");
			globalRefManager.interfaceManager.activeUserInterface.interfaceDescription.text = globalRefManager.langManager.GetTranslation("delete_modal_info") + ": " + deletion;
			roomsToDelete = roomsThatWillBeDestroyed.ToArray();
		}
	}

	//either acutally goes through the list and deletes the tiles, or cancels the action
	public void ActuallyDeleteRooms(bool delete)
	{
		if (delete)
		{
			int count = roomsToDelete.Length;
			globalRefManager.interfaceManager.SetWorldPositionViewerState(false, null);
			for (int i = 0; i < roomsToDelete.Length; i++)
			{
				DeleteRoom(roomsToDelete[i]);
			}

			globalRefManager.interfaceManager.EnqueueNotification("tiles_deleted", "", "successfully_destroyed_tiles", new string[]{ "" + count});

		}
		else
		{
			roomsToDelete = null;
		}
	}

	//standardizes the removal of rooms to ensure all reqs met
	public void DeleteRoom(ContainedRoom room)
	{
		foreach(RoomTile rt in room.containedRooms)
		{
			roomIndexingVectors[rt.GetIndexdTilePosition().y][rt.GetIndexdTilePosition().x] = null;
			rt.UpdateTile(true);
		}

		Destroy(room.gameObject);
	}

	//recursively calls itself to get all the tiles that would be deleted given the initaial location
	public void RoomsThatWillBeDestroyedOnCall(ContainedRoom prev, List<ContainedRoom> roomsThatWillBeDestroyed, int iteration)
	{
		foreach (RoomTile room in prev.containedRooms)
		{
			for (int i = 0; i < 4; i++)
			{
				RoomTile other = GetRoomAtPosition(room.GetTrueTilePosition() + room.offsets[i]);
				if (other != null)
				{
					int[] otherNodeStates = { other.tileType.bottomNodeState, other.tileType.leftNodeState, other.tileType.topNodeState, other.tileType.rightNodeState };
					if (otherNodeStates[i] == 1)
					{
						if (!roomsThatWillBeDestroyed.Contains(other.roomContainer))
						{
							roomsThatWillBeDestroyed.Add(other.roomContainer);
;							if (iteration < 100000)
								RoomsThatWillBeDestroyedOnCall(other.roomContainer, roomsThatWillBeDestroyed, iteration++);
							else
								globalRefManager.interfaceManager.SetMajorInterface("Error");
						}
					}
				}
			}
		}
	}

	//to be called when the player clicks on a tile in edit mode
	public void OnClickOnTile()
	{
		if (editModePermSelectedRoomTile != null)
			editModePermSelectedRoomTile.roomContainer.SetRoomTint(Color.white);
		editModePermSelectedRoomTile = editModeSelectedRoomTile;
		if (editModePermSelectedRoomTile != null)
		{
			editModePermSelectedRoomTile.roomContainer.SetRoomTint(Color.cyan);
			globalRefManager.interfaceManager.SetWorldPositionViewerState(true, editModePermSelectedRoomTile);
		}
		else
			globalRefManager.interfaceManager.CloseWorldPositionViewer();
	}

	//returns the room at the world coordinate space position
	public RoomTile GetRoomAtPosition(Vector2Int pos)//always call with the real world position
	{

		//transforms the position to be relative to the indexing array
		pos.x += (globalRefManager.terrainManager.terrainWidth / 2);
		pos.y += globalRefManager.terrainManager.terrainBottomLayer;

		//called by the indexing array to save on proccessing time cause loops suck
		if (roomIndexingVectors.Count > pos.y && pos.y >=0 && roomIndexingVectors[pos.y].Count > pos.x && pos.x>=0)
		{
			return roomIndexingVectors[pos.y][pos.x];
		}
		else
		{
			return null;
		}
	}

	//instantiates and preloads a contained room at a given position. Does not check if that place is a valid spot
	public ContainedRoom TryCreateRoomAtPos(Vector2Int pos, ContainedRoom roomPrefab) //call with the real world position
	{
		ContainedRoom newGen = Instantiate(roomPrefab);
		baseRooms.Add(newGen);
		newGen.globalRefManager = globalRefManager;
		newGen.transform.position = new Vector3(pos.x, pos.y, 0f);
		newGen.transform.SetParent(transform);
		newGen.activeAndEnabled = true;
		newGen.isNaturalTerrainTile = (newGen.containedRooms[0].tileType == globalRefManager.terrainManager.Dirt.containedRooms[0].tileType) || (newGen.containedRooms[0].tileType == globalRefManager.terrainManager.Bedrock.containedRooms[0].tileType);
		globalUpdateID++;
		foreach (RoomTile tile in newGen.containedRooms)
		{
			//corrects the indexing based on the new placements
			while(roomIndexingVectors.Count <= tile.GetIndexdTilePosition().y)
			{
				roomIndexingVectors.Add(new List<RoomTile>());
				for (int i = 0; i < globalRefManager.terrainManager.terrainWidth; i++)
					roomIndexingVectors[roomIndexingVectors.Count - 1].Add(null);
			}
			roomIndexingVectors[tile.GetIndexdTilePosition().y][tile.GetIndexdTilePosition().x] = tile;
			tile.transform.name = "Room " + tile.GetTrueTilePosition().x + ", " + tile.GetTrueTilePosition().y + " of " + tile.roomContainer.ContainedRoomName;
		}
		//auto updates the tiles around it, but not the full map
		foreach (RoomTile tile in newGen.containedRooms)
		{
			tile.UpdateTile(true);
		}

		return newGen;
	}

	public ContainedRoom GetRoomPrefab(string calllbackID)
	{
		return roomPrefabCatalog.ContainsKey(calllbackID) ? roomPrefabCatalog[calllbackID] : null;
	}
}