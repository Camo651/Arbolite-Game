using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager : MonoBehaviour
{
	[HideInInspector] public GlobalRefManager globalRefManager;
	public List<ContainedRoom> allPlaceableRooms;

	public ContainedRoom GetRoomPrefabByName(string roomName)
	{
		foreach(ContainedRoom cr in allPlaceableRooms)
		{
			if(cr != null && cr.ContainedRoomName == roomName)
			{
				return cr;
			}
		}
		return null;
	}
}
