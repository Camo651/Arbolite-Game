using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Stores the data of a tree node in a seprate location in the world that will never be deactived
/// </summary>
[ExecuteAlways]
[System.Serializable]
public class DataNode: MonoBehaviour
{
	/// <summary>
	/// The possible states of a data tree node
	/// </summary>
	public enum NodeState
	{
		Locked,
		Unlocked,
		Obtained
	}
	public enum RoomCheckState {Place, Break}
	public string nodeCallbackID;
	public NodeState nodeState;
	public GameObject unlocks;
	public TreeDisplayer treeDisplayer;
	public DataNode parentTreeNode;
	public List<DataNode> childedDataNodes;

	[HideInInspector] public List<GameObject> containedRoomTypeCheck; //use the callback ids for checks
	[HideInInspector] public List<int> containerRoomValueCheck;
	[HideInInspector] public List<string> statCheckCallbackIDs;
	[HideInInspector] public List<float> statCheckValues;
	[HideInInspector] public RoomCheckState roomCheckState;

	public void LateUpdate()
	{
		SetNodeData();
	}

	public bool CheckConditionsMet()
	{
		int totalConditions = containedRoomTypeCheck.Count + statCheckCallbackIDs.Count;
		int conditionsMet = 0;
		StatisticsManager s = treeDisplayer.treeManager.globalRefManager.statisticsManager;
		for (int i = 0; i < containedRoomTypeCheck.Count; i++)
		{
			switch (roomCheckState)
			{
				case RoomCheckState.Place:
					conditionsMet+= (containedRoomTypeCheck[i] && s.GetTimesPlaced(containedRoomTypeCheck[i].GetComponent<ContainedRoom>()) >= containerRoomValueCheck[i])?1:0;
					break;
				case RoomCheckState.Break:
					conditionsMet+= (containedRoomTypeCheck[i] && s.GetTimesDestroyed(containedRoomTypeCheck[i].GetComponent<ContainedRoom>()) >= containerRoomValueCheck[i])?1:0;
					break;
			}
		}
		for (int i = 0; i < statCheckCallbackIDs.Count; i++)
		{
			if(s.GetStat(statCheckCallbackIDs[i]).value >= statCheckValues[i])
			{
				conditionsMet++;
			}
		}
		return conditionsMet == totalConditions;
	}

	/// <summary>
	/// Sets the node's data and children
	/// </summary>
	public void SetNodeData()
	{
		parentTreeNode = transform.parent.GetComponent<DataNode>();
		childedDataNodes = new List<DataNode>();
		for (int i = 0; i < transform.childCount; i++)
		{
			if (transform.GetChild(i).GetComponent<DataNode>())
			{
				childedDataNodes.Add(transform.GetChild(i).GetComponent<DataNode>());
			}
		}
	}

	/// <summary>
	/// Get the name of the data value of the current node
	/// </summary>
	/// <returns>The name of the data value of the current node</returns>
	public string GetNodeName()
	{
		return treeDisplayer.treeManager.globalRefManager.langManager.GetTranslation("advnc_name_"+nodeCallbackID);
	}

	/// <summary>
	/// Get the info of the data value of the current node
	/// </summary>
	/// <returns>The info of the data value of the current node</returns>
	public string GetNodeInfo()
	{
		return treeDisplayer.treeManager.globalRefManager.langManager.GetTranslation("advnc_info_"+nodeCallbackID);
	}

	/// <summary>
	/// Get the sprite icon of the data value of the current node
	/// </summary>
	/// <returns>The sprite icon of the data value of the current node</returns>
	public Sprite GetIcon()
	{
		return null;
	}
}

[CustomEditor(typeof(DataNode))]
public class DataNodeEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		DataNode node = (DataNode)target;

		if (node.containedRoomTypeCheck == null)
			node.containedRoomTypeCheck = new List<GameObject>();
		if (node.containerRoomValueCheck == null)
			node.containerRoomValueCheck = new List<int>();
		if (node.statCheckCallbackIDs == null)
			node.statCheckCallbackIDs = new List<string>();
		if (node.statCheckValues == null)
			node.statCheckValues = new List<float>();

		GUILayout.Space(20);
		GUILayout.Label("Room Parametres");
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Add Room"))
		{
			node.containedRoomTypeCheck.Add(null);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginVertical();
		for (int i = 0; i < node.containedRoomTypeCheck.Count; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Room " + i);
			node.roomCheckState = (DataNode.RoomCheckState)EditorGUILayout.EnumPopup("",node.roomCheckState,GUILayout.MinWidth(10));
			node.containedRoomTypeCheck[i] = (GameObject)EditorGUILayout.ObjectField(node.containedRoomTypeCheck[i], typeof(GameObject), false);
			if(node.containerRoomValueCheck.Count <= i)
			{
				node.containerRoomValueCheck.Add(0);
			}
			GUILayout.Label("≥");
			node.containerRoomValueCheck[i] = EditorGUILayout.IntField(node.containerRoomValueCheck[i]);
			if (GUILayout.Button("X"))
			{
				node.containedRoomTypeCheck.RemoveAt(i);
				node.containerRoomValueCheck.RemoveAt(i);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		GUILayout.Space(10);
		GUILayout.Label("Stat Parametres");
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Stat"))
		{
			node.statCheckCallbackIDs.Add(null);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginVertical();

		for (int i = 0; i < node.statCheckCallbackIDs.Count; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Stat " + i);
			node.statCheckCallbackIDs[i] = EditorGUILayout.TextField(node.statCheckCallbackIDs[i]);
			if (node.statCheckValues.Count <= i)
			{
				node.statCheckValues.Add(0f);
			}
			GUILayout.Label("≥");
			node.statCheckValues[i] = EditorGUILayout.FloatField(node.statCheckValues[i]);
			if (GUILayout.Button("X"))
			{
				node.statCheckCallbackIDs.RemoveAt(i);
				node.statCheckValues.RemoveAt(i);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();

		if (GUI.changed)
		{
			EditorUtility.SetDirty(node);
			EditorSceneManager.MarkSceneDirty(node.gameObject.scene);
		}
	}
}
