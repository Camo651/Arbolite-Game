using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Stores the data of a tree node in a seprate location in the world that will never be deactived
/// </summary>
[ExecuteAlways]
public class DataNode: MonoBehaviour
{
	public enum NodeState
	{
		Visible,
		Unlocked,
		Obtained
	}
	public NodeState nodeState;
	public GameObject unlocks;
	public TreeDisplayer treeDisplayer;
	public DataNode parentTreeNode;
	public List<DataNode> childedDataNodes;
	public Color nodeColour;

	public void LateUpdate()
	{
		SetNodeData();
	}

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
}
