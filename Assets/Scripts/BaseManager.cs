using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
	public List<ContainedRoom> baseRooms = new List<ContainedRoom>();
	public PlayerState currentPlayerState;
	public enum PlayerState
	{
		Player,
		BuildMode,
		DestroyMode
	}
}
