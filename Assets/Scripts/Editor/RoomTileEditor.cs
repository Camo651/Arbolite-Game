using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomTile))]
public class RoomTileEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if(GUILayout.Button("Create New Plant Node"))
		{
			Node node = ((GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Default/DefaultNode.prefab", typeof(GameObject)), ((RoomTile)target).transform)).GetComponent<Node>();
			node.nodeType = Node.NodeType.PlantSpot;
		}
	}
}
