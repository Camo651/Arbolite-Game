using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(ContainedRoom))]
public class RoomBuilder : Editor
{
	[MenuItem("GameObject/Create Other/Room Container")]
	public static void CreateNewRoom()
	{
		Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Default/EMPTY ROOM.prefab", typeof(GameObject))).transform.name = "Empty Room";
	}
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		ContainedRoom cont = (ContainedRoom)target;
		if(cont.transform.childCount == 0)
		{
			cont.ContainedRoomName = EditorGUILayout.TextField("Room Name", cont.ContainedRoomName);
			GUILayout.Space(10);
			cont.roomDimensions.x = EditorGUILayout.IntSlider("Room Width", cont.roomDimensions.x, 1, 10);
			cont.roomDimensions.y = EditorGUILayout.IntSlider("Room Height", cont.roomDimensions.y, 1, 10);

			if (GUILayout.Button("Generate Room"))
			{
				cont.transform.name = "[CONTROOM] " + cont.ContainedRoomName;

				Vector2Int size = cont.roomDimensions;
				cont.containedRooms = new List<RoomTile>();
				for (int y = size.y-1; y >= 0; y--)
				{
					for (int x = 0; x < size.x; x++)
					{
						RoomTile a = new GameObject("Room " + x + ", " + y + " of " + cont.ContainedRoomName).AddComponent<RoomTile>();
						cont.containedRooms.Add(a);
						a.tileType = (SO_TileType)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/TileTypes/DefaultTile.asset", typeof(SO_TileType));
						a.spriteRenderer = a.gameObject.AddComponent<SpriteRenderer>();
						a.spriteRenderer.sprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Images/Square.png", typeof(Sprite));
						a.transform.SetParent(cont.transform);
						a.transform.position = new Vector2(x, y);
						a.roomContainer = cont;
						a.neighborWelds = new bool[] { !(y + 1 == size.y), !(x + 1 == size.x), !(y == 0), !(x == 0) };
					}
				}
			}
		}
		else
		{
			GUILayout.Label("Named: " + cont.ContainedRoomName + " | Width: " + cont.roomDimensions.x + " Height: " + cont.roomDimensions.y);
			GUILayout.Label("(Reset the Room To Edit)");


			if (GUILayout.Button("Reset Room"))
			{
				ResetRoom(cont);
			}

			for (int y = 0; y < cont.roomDimensions.y; y++)
			{
				GUILayout.BeginHorizontal();
				for (int x = 0; x < cont.roomDimensions.x; x++)
				{
					if(cont.containedRooms != null)
					{
						int index = x + (y * cont.roomDimensions.x);
						cont.containedRooms[index].tileType = (SO_TileType)EditorGUILayout.ObjectField(cont.containedRooms[index].tileType, typeof(SO_TileType), false);
						if (cont.containedRooms[index].tileType != null)
							cont.containedRooms[index].spriteRenderer.sprite = cont.containedRooms[index].tileType.backgroundSprite;
						else
							cont.containedRooms[index].spriteRenderer.sprite = null;
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.FlexibleSpace();
		}

	}
	private void ResetRoom(ContainedRoom cont)
	{
		cont.containedRooms.Clear();
		while (cont.transform.childCount > 0)
			DestroyImmediate(cont.transform.GetChild(0).gameObject, false);
		cont.transform.name = "EMPTY ROOM";
		cont.roomDimensions = Vector2Int.one;
		cont.ContainedRoomName = "";
	}
}