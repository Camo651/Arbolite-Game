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
	public string selectedRoomName;
	private bool colliding, nodeConditionsMet;
	public RoomTile editModeSelectedRoomTile, editModePermSelectedRoomTile;
	public SelectionBox hoverSelect, clickedSelect;
	private Vector2Int currentSelectionCoords;
	public bool gameIsActivelyFrozen;
	public ContainedRoom[] roomsToDelete;
	Vector2 smoothCursorMovementDamp;
	public Dictionary<string, ContainedRoom> roomPrefabCatalog;
	public ContainedRoom defaultRoomTile;

	public enum PlayerState
	{
		PlayerMode,
		BuildMode,
		EditMode
	}

	private void Start()
	{

		ContainedRoom[] unsortedRooms = Resources.LoadAll<ContainedRoom>("");
		roomPrefabCatalog = new Dictionary<string, ContainedRoom>();
		foreach (ContainedRoom item in unsortedRooms)
		{
			roomPrefabCatalog.Add(item.tileNameCallbackID.ToLower(), item);
		}
	}

	/// <summary>
	/// Set the state of the player
	/// </summary>
	/// <param name="state">The state to be set to</param>
	public void SetPlayerState(PlayerState state)
	{
		if (state != currentPlayerState)
		{
			globalRefManager.interfaceManager.SetWorldPositionViewerState(false, null);
			if (editModePermSelectedRoomTile != null)
				clickedSelect.ClearSelection();
			editModePermSelectedRoomTile = null;
			if (editModeSelectedRoomTile != null)
				hoverSelect.ClearSelection();
			editModeSelectedRoomTile = null;

			globalRefManager.playerManager.SetPhysicalPlayerState(state == PlayerState.PlayerMode);

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
						ContainedRoom prefab = GetRoomPrefab(selectedRoomName);
						if(prefab == null)
						{
							SetPlayerState(PlayerState.PlayerMode);
							return;
						}
						ContainedRoom cr = Instantiate(prefab.gameObject).GetComponent<ContainedRoom>();
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
								int[] roomNodeStates = { room.topNodeState, room.rightNodeState, room.bottomNodeState, room.leftNodeState };
								for (int i = 0; i < 4; i++)
								{
									RoomTile other = GetRoomAtPosition(room.GetTrueTilePosition() + room.offsets[i]);
									bool hasRoomOnLockedSide = other && roomNodeStates[i] == 2;
									bool connectorNodeMet = (roomNodeStates[i] == 1 && other) || roomNodeStates[i] != 1;
									bool otherHasLockedSideOnRoom = false;
									if (other != null)
									{

										int[] otherNodeStates = { other.bottomNodeState, other.leftNodeState, other.topNodeState, other.rightNodeState };
										if (otherNodeStates[i] == 2)
										{
											otherHasLockedSideOnRoom = true;
										}
									}

									if (hasRoomOnLockedSide || !connectorNodeMet || otherHasLockedSideOnRoom)
									{
										nodeConditionsMet = false;
										room.spriteRenderer.color = globalRefManager.settingsManager.buildModeDeny;
									}
									else
									{
										room.spriteRenderer.color = globalRefManager.settingsManager.buildModeAllow;
									}
								}
								if (nodeConditionsMet)
								{
									if (GetRoomAtPosition(room.GetTrueTilePosition()) != null)
									{
										colliding = true;
										room.spriteRenderer.color = globalRefManager.settingsManager.buildModeDeny;
									}
									else
									{
										room.spriteRenderer.color = globalRefManager.settingsManager.buildModeAllow;
									}
								}
							}
						}
						if (Input.GetMouseButtonDown(0) && !colliding && nodeConditionsMet) //place a copy of the ghost room at the postion if possible
						{
							globalRefManager.audioManager.Play(AudioManager.AudioClipType.Ambient,"little_click");
							ContainedRoom cr = TryCreateRoomAtPos(new Vector2Int(Mathf.RoundToInt(currentlySelectedRoom.transform.position.x), Mathf.RoundToInt(currentlySelectedRoom.transform.position.y)), GetRoomPrefab(selectedRoomName),true);
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
								hoverSelect.ClearSelection();
							editModeSelectedRoomTile = roomAtSelectionCoords;
							if (editModeSelectedRoomTile != null && ((editModePermSelectedRoomTile == null) || (editModePermSelectedRoomTile && editModeSelectedRoomTile && editModePermSelectedRoomTile.roomContainer != editModeSelectedRoomTile.roomContainer)))
								hoverSelect.SetSelection(editModeSelectedRoomTile.roomContainer);
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

	/// <summary>
	/// Initiates the UI modal for confirming if the tiles should be deleted
	/// </summary>
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
				deletion += globalRefManager.langManager.GetTranslation("name_"+c.tileNameInfoID.ToLower()) + ", ";
			}
			deletion = deletion.Substring(0, deletion.Length - 2);
			globalRefManager.interfaceManager.SetMajorInterface("ConfirmDelete");
			globalRefManager.interfaceManager.activeUserInterface.interfaceDescription.text = globalRefManager.langManager.GetTranslation("delete_modal_info") + ": " + deletion;
			roomsToDelete = roomsThatWillBeDestroyed.ToArray();
		}
	}

	/// <summary>
	/// either acutally goes through the list and deletes the tiles, or cancels the action
	/// </summary>
	/// <param name="delete">Should the tiles in the list be deleted</param>
	public void ActuallyDeleteRooms(bool delete)
	{
		if (delete)
		{
			StartCoroutine(FadeOutDestroyedTiles());
			clickedSelect.ClearSelection();
		}
		else
		{
			for (int i = 0; i < roomsToDelete.Length; i++)
			{
				foreach (RoomTile tr in roomsToDelete[i].containedRooms)
					tr.canBeUpdated = true;
			}
			roomsToDelete = null;
			System.GC.Collect();
		}
	}

	/// <summary>
	/// Slows the destruction sequence of the tiles to make it more applealing
	/// </summary>
	/// <returns>Nothing</returns>
	IEnumerator FadeOutDestroyedTiles()
	{
		int count = roomsToDelete.Length;
		globalRefManager.interfaceManager.SetWorldPositionViewerState(false, null);
		for (int i = 0; i < roomsToDelete.Length; i++)
		{
			DeleteRoom(roomsToDelete[i]);
			globalRefManager.audioManager.Play(AudioManager.AudioClipType.Ambient, "destroy");
			yield return new WaitForSeconds(.04f);
		}

		globalRefManager.interfaceManager.EnqueueNotification("tiles_deleted", "successfully_destroyed_tiles", new string[] { "" + count });

	}

	/// <summary>
	/// Hotswaps the room at a position
	/// </summary>
	/// <param name="pos">The worldspace coordinates of the tile</param>
	/// <param name="newRoomPrefab">The prefab object of the tile to replace it. Should be the same size as the tile at pos</param>
	public void ChangeRoom(Vector2Int pos, ContainedRoom newRoomPrefab)
	{
		RoomTile _oldRoom = GetRoomAtPosition(pos);
		if(_oldRoom == null)
		{
			return;
		}
		ContainedRoom oldRoom = _oldRoom.roomContainer;
		foreach (RoomTile rt in oldRoom.containedRooms)
		{
			roomIndexingVectors[rt.GetIndexdTilePosition().y][rt.GetIndexdTilePosition().x] = null;
		}

		Destroy(oldRoom.gameObject);
		ContainedRoom room = TryCreateRoomAtPos(pos, newRoomPrefab, false);
		foreach (RoomTile rt in room.containedRooms)
		{
			roomIndexingVectors[rt.GetIndexdTilePosition().y][rt.GetIndexdTilePosition().x] = rt;
			rt.UpdateTile(false, room.GetHashCode()+"Change");
		}
	}

	/// <summary>
	/// Standard immidiate room deletion
	/// </summary>
	/// <param name="room">The controom to be removed</param>
	public void DeleteRoom(ContainedRoom room)
	{
		foreach(RoomTile rt in room.containedRooms)
		{
			roomIndexingVectors[rt.GetIndexdTilePosition().y][rt.GetIndexdTilePosition().x] = null;
		}
		foreach(RoomTile rt in room.containedRooms)
		{
			rt.UpdateNeighboringTiles(0,room.GetHashCode()+"Delete");
		}

		Destroy(room.gameObject);
	}

	/// <summary>
	/// recursively calls itself to get all the tiles that would be deleted given the initaial location
	/// </summary>
	/// <param name="prev">The previous room in the recursion chain</param>
	/// <param name="roomsThatWillBeDestroyed">The list of all the rooms that will be removed</param>
	/// <param name="iteration">The iteration of the recursion. Will throw an error after 1 million iterations</param>
	public void RoomsThatWillBeDestroyedOnCall(ContainedRoom prev, List<ContainedRoom> roomsThatWillBeDestroyed, int iteration)
	{
		foreach (RoomTile room in prev.containedRooms)
		{
			for (int i = 0; i < 4; i++)
			{
				RoomTile other = GetRoomAtPosition(room.GetTrueTilePosition() + room.offsets[i]);
				if (other != null)
				{
					int[] otherNodeStates = { other.bottomNodeState, other.leftNodeState, other.topNodeState, other.rightNodeState };
					if (otherNodeStates[i] == 1)
					{
						if (!roomsThatWillBeDestroyed.Contains(other.roomContainer))
						{
							roomsThatWillBeDestroyed.Add(other.roomContainer);
							other.canBeUpdated = false;
							if (iteration < 100000)
								RoomsThatWillBeDestroyedOnCall(other.roomContainer, roomsThatWillBeDestroyed, iteration++);
							else
								globalRefManager.interfaceManager.ThrowErrorMessage("overflow_error_message");
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Handles the player clicking on a tile in the worldspace
	/// </summary>
	public void OnClickOnTile()
	{
		if(editModePermSelectedRoomTile == editModeSelectedRoomTile)
		{
			return;
		}
		if (editModePermSelectedRoomTile != null)
			clickedSelect.ClearSelection();
		editModePermSelectedRoomTile = editModeSelectedRoomTile;
		if (editModePermSelectedRoomTile != null)
		{
			clickedSelect.SetSelection(editModePermSelectedRoomTile.roomContainer);
			globalRefManager.interfaceManager.SetWorldPositionViewerState(true, editModePermSelectedRoomTile);
			globalRefManager.audioManager.Play(AudioManager.AudioClipType.Ambient, "dirt_select");
		}
		else
			globalRefManager.interfaceManager.CloseWorldPositionViewer();
	}

	/// <summary>
	/// Get the room at a position
	/// </summary>
	/// <param name="pos">The worldspace integer coordinates of the room tile</param>
	/// <returns>The room tile if it exists at that location</returns>
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

	/// <summary>
	/// instantiates and preloads a contained room at a given position. Does not check if that place is a valid spot
	/// </summary>
	/// <param name="pos">The worldspace position of the place to instantiate at</param>
	/// <param name="roomPrefab">The prefab to instatniate</param>
	/// <param name="updateNeighbors">Should the neighboring tiles be updated upon instatniation</param>
	/// <returns>The newly name contained room</returns>
	public ContainedRoom TryCreateRoomAtPos(Vector2Int pos, ContainedRoom roomPrefab, bool updateNeighbors)
	{
		ContainedRoom newGen = Instantiate(roomPrefab);
		baseRooms.Add(newGen);
		newGen.globalRefManager = globalRefManager;
		newGen.transform.position = new Vector3(pos.x, pos.y, 0f);
		newGen.transform.SetParent(transform);
		newGen.activeAndEnabled = true;
		newGen.isNaturalTerrainTile = roomPrefab.isNaturalTerrainTile;
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
			tile.transform.name = "Room " + tile.GetTrueTilePosition().x + ", " + tile.GetTrueTilePosition().y + " of " + tile.roomContainer.tileNameCallbackID;
			tile.canBeUpdated = true;
		}
		//auto updates the tiles around it, but not the full map
		foreach (RoomTile tile in newGen.containedRooms)
		{
			tile.UpdateTile(updateNeighbors, newGen.GetHashCode()+"Create");
			tile.StartGeneration();
		}

		return newGen;
	}

	/// <summary>
	/// Get a room by its callback ID. Will throw a modal error if the ID is invalid
	/// </summary>
	/// <param name="calllbackID"></param>
	/// <returns>the room, given that it exists</returns>
	public ContainedRoom GetRoomPrefab(string calllbackID)
	{
		calllbackID = calllbackID.ToLower();
		if (roomPrefabCatalog.ContainsKey(calllbackID))
			return roomPrefabCatalog[calllbackID];
		else
		{
			globalRefManager.interfaceManager.SetMajorInterface("Error");
			return null;
		}
	}
}