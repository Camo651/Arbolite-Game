//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(ContentManager))]
//public class ContentAdditionVerifier : Editor
//{
//	ContainedRoom cr;
//	string msg = "Verify";
//	int editorUpdateTime = 0;
//	public override void OnInspectorGUI()
//	{
//		base.OnInspectorGUI();
//		ContentManager content = (ContentManager)target;
//		GUILayout.Label("Insert object here and click verify to add it to the list");
//		GUILayout.BeginHorizontal();
//		cr = (ContainedRoom)EditorGUILayout.ObjectField(cr,typeof(ContainedRoom),false);
//		if (GUILayout.Button(msg))
//		{
//			//checks if the element exists and that it is not already in the array before adding it
//			if (cr != null)
//			{
//				if (content.GetRoomPrefabByName(cr.ContainedRoomName) == null)
//				{
//					content.allPlaceableRooms.Add(cr);
//					msg = "Added!";
//				}
//				else
//				{
//					msg = "Already Exists";
//				}
//				cr = null;
//			}
//			else
//			{
//				msg = "Nothing Added";
//			}
//		}
//		GUILayout.EndHorizontal();
//		if (msg != "Verify")
//			editorUpdateTime++;
//		if (editorUpdateTime > 10)
//		{
//			editorUpdateTime = 0;
//			msg = "Verify";
//		}
//		GUILayout.Space(20);
//		//cleans up the empty spaces from the list
//		if(GUILayout.Button("Clean Up"))
//		{
//			List<ContainedRoom> newContent = new List<ContainedRoom>();
//			foreach(ContainedRoom room in content.allPlaceableRooms)
//			{
//				if (room)
//				{
//					newContent.Add(room);
//				}
//			}
//			content.allPlaceableRooms = newContent;
//		}
//	}
//}
