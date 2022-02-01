using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ContainedRoom))]

public class RoomGenerator : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		ContainedRoom room = (ContainedRoom)target;
		
	}
}

