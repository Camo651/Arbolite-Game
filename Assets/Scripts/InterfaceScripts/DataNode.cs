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
	/// <summary>
	/// The possible states of a data tree node
	/// </summary>
	public enum NodeState
	{
		Locked,
		Unlocked,
		Obtained
	}
	public NodeState nodeState;
	public GameObject unlocks;
	public TreeDisplayer treeDisplayer;
	public DataNode parentTreeNode;
	public List<DataNode> childedDataNodes;

	public void LateUpdate()
	{
		SetNodeData();
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
		return "Name";
	}

	/// <summary>
	/// Get the info of the data value of the current node
	/// </summary>
	/// <returns>The info of the data value of the current node</returns>
	public string GetNodeInfo()
	{
		return "Info";
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
