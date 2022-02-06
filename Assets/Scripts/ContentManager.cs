using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager : MonoBehaviour
{
	[HideInInspector] public GlobalRefManager globalRefManager;
	public List<ContainedRoom> allPlaceableRooms;


	//returns the prefab of the room based on its name given
	//may want to optimize this with a parallel indexing array if there gets to be a lot of content
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
