using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(ContainedRoom))]
public class RoomBuilder : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		ContainedRoom cont = (ContainedRoom)target;
		if(cont.transform.childCount == 0)
		{
			cont.tileNameCallbackID = EditorGUILayout.TextField("Catalog Callback ID", cont.tileNameCallbackID);
			GUILayout.Space(10);
			cont.roomDimensions.x = EditorGUILayout.IntSlider("Room Width", cont.roomDimensions.x, 1, 10);
			cont.roomDimensions.y = EditorGUILayout.IntSlider("Room Height", cont.roomDimensions.y, 1, 10);

			//makes a new room and preloads it with tiles
			if (GUILayout.Button("Generate Room"))
			{
				//guard clause to prevent data corruption in prefabs
				if (!((ContainedRoom)target).gameObject.scene.IsValid())
				{
					Debug.LogWarning("Cannot edit prefabs outside of the scene view");
					return;
				}

				Vector2Int size = cont.roomDimensions;
				cont.containedRooms = new List<RoomTile>();
				for (int y = size.y-1; y >= 0; y--)
				{
					for (int x = 0; x < size.x; x++)
					{
						RoomTile a = new GameObject("Room " + x + ", " + y + " of " + cont.tileNameCallbackID).AddComponent<RoomTile>();
						cont.containedRooms.Add(a);
						//a.tileType = (SO_TileType)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/TileTypes/DefaultTile.asset", typeof(SO_TileType));
						a.spriteRenderer = a.gameObject.AddComponent<SpriteRenderer>();
						a.spriteRenderer.sortingOrder = 5;
						a.spriteRenderer.sprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Images/Square.png", typeof(Sprite));
						a.transform.SetParent(cont.transform);
						a.transform.position = new Vector2(x, y);
						a.roomContainer = cont;
						a.neighborWelds = new bool[] { !(y + 1 == size.y), !(x + 1 == size.x), !(y == 0), !(x == 0) };
						a.neighborRooms = new RoomTile[4];
					}
				}
			}
		}
		else
		{
			GUILayout.Label("Named: " + cont.tileNameCallbackID + " | Width: " + cont.roomDimensions.x + " Height: " + cont.roomDimensions.y);
			GUILayout.Label("(Reset the Room To Edit)");


			if (GUILayout.Button("Reset Room"))
			{
				//guard clause to prevent data corruption in prefabs
				if (!((ContainedRoom)target).gameObject.scene.IsValid())
				{
					Debug.LogWarning("Cannot reset prefabs outside of the scene view");
					return;
				}
				ResetRoom(cont);
			}

			//gives a layout for tile types to make it easier to edit them on the fly
			//for (int y = 0; y < cont.roomDimensions.y; y++)
			//{
			//	GUILayout.BeginHorizontal();
			//	for (int x = 0; x < cont.roomDimensions.x; x++)
			//	{
			//		if(cont.containedRooms != null)
			//		{
			//			int index = x + (y * cont.roomDimensions.x);
			//			cont.containedRooms[index].tileType = (SO_TileType)EditorGUILayout.ObjectField(cont.containedRooms[index].tileType, typeof(SO_TileType), false);
			//			cont.containedRooms[index].spriteRenderer = cont.containedRooms[index].GetComponent<SpriteRenderer>();						if (cont.containedRooms[index].tileType != null)
			//				cont.containedRooms[index].spriteRenderer.sprite = cont.containedRooms[index].tileType.backgroundSprite;
			//			else
			//				cont.containedRooms[index].spriteRenderer.sprite = null;
			//		}
			//	}
			//	GUILayout.EndHorizontal();
			//}
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
		cont.tileNameCallbackID = "";
	}
}