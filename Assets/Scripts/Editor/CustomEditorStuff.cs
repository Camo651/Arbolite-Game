using UnityEngine;
using UnityEditor;

public class CustomEditorStuff : Editor
{
	[MenuItem("GameObject/Create Other/Plant Part")]
	public static void CreateNewPlant()
	{
		GameObject a = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Default/DefaultPlantPart.prefab", typeof(GameObject)));
		a.transform.name = "DefaultPlantPart";
		Selection.activeGameObject = a;
	}

	[MenuItem("GameObject/Create Other/Room Container")]
	public static void CreateNewRoom()
	{
		GameObject a = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Default/DefaultContainedRoom.prefab", typeof(GameObject)));
		a.transform.name = "Empty Room";
		Selection.activeGameObject = a;
	}
}
