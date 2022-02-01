using UnityEditor;
using UnityEngine;

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
		cont.baseManager = GameObject.FindGameObjectWithTag("Base").GetComponent<BaseManager>();

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
				cont.roomCompositionMatrix = new RoomTile[size.x][];
				for (int x = 0; x < size.x; x++)
				{
					cont.roomCompositionMatrix[x] = new RoomTile[size.y];
					for (int y = 0; y < size.y; y++)
					{
						RoomTile a = cont.roomCompositionMatrix[x][y] = new GameObject("Room " + x + ", " + y + " of " + cont.ContainedRoomName).AddComponent<RoomTile>();
						a.tileType = (SO_TileType)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/TileTypes/DefaultTile.asset", typeof(SO_TileType));
						a.gameObject.AddComponent<SpriteRenderer>().sprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Images/Square.png", typeof(Sprite));
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

			for (int y = cont.roomDimensions.y-1; y >= 0; y--)
			{
				GUILayout.BeginHorizontal();
				for (int x = 0; x < cont.roomDimensions.x; x++)
				{
					if(cont.roomCompositionMatrix != null)
					{
						cont.roomCompositionMatrix[x][y].tileType = (SO_TileType)EditorGUILayout.ObjectField(cont.roomCompositionMatrix[x][y].tileType, typeof(SO_TileType), false);
						if (cont.roomCompositionMatrix[x][y].tileType != null)
							cont.roomCompositionMatrix[x][y].GetComponent<SpriteRenderer>().sprite = cont.roomCompositionMatrix[x][y].tileType.backgroundSprite;
						else
							cont.roomCompositionMatrix[x][y].GetComponent<SpriteRenderer>().sprite = null;
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.FlexibleSpace();
		}

	}
	private void ResetRoom(ContainedRoom cont)
	{
		cont.roomCompositionMatrix = null;
		while (cont.transform.childCount > 0)
			DestroyImmediate(cont.transform.GetChild(0).gameObject, false);
		cont.transform.name = "EMPTY ROOM";
		cont.roomDimensions = Vector2Int.one;
		cont.ContainedRoomName = "";
	}
}