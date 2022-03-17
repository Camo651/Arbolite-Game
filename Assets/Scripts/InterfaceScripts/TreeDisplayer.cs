using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Not a manager class. Can be instanced multilple times
/// </summary>

public class TreeDisplayer : MonoBehaviour
{
	public TreeManager treeManager;
	public string treeCallbackID;
	public List<TreeNodeDisplayBox> allTreeNodesInTree;
	public int currentLineDepth;
	public DataNode dataRootNode;
	public TreeNodeDisplayBox displayRootNode;
	public Color[] nodeStateColours;
	public InformationHighlighter NodeInfoHighlight;

	/// <summary>
	/// Displays all the nodes in the tree starting from the dataRoot. Clears the current nodes first
	/// </summary>
	public void DisplayTree()
	{
		SetHighlightDisplay(false, null);
		for (int i = 0; i < displayRootNode.childHolder.transform.childCount; i++)
		{
			Destroy(displayRootNode.childHolder.transform.GetChild(i).gameObject);
		}
		displayRootNode.treeDisplayer = this;
		displayRootNode.SetNodeDisplay(dataRootNode, null,-1);
	}

	/// <summary>
	/// Sets the data of the node highlight box
	/// </summary>
	/// <param name="state">The state of the node</param>
	/// <param name="node">The node to be displayed</param>
	public void SetHighlightDisplay(bool state, TreeNodeDisplayBox node)
	{
		if (!state || !node)
		{
			NodeInfoHighlight.CloseHighlight();
			return;
		}
		NodeInfoHighlight.OpenHighlight(node.GetHashCode(), node.dataNode.GetNodeName(), node.dataNode.nodeState + "", node.dataNode.GetNodeInfo(), node.dataNode.GetIcon(), Color.white);
		isHovered = state;
	}
	private bool isHovered;
	private void Update()
	{
		if (isHovered)
		{
			NodeInfoHighlight.transform.position = Input.mousePosition;
		}
	}
}
