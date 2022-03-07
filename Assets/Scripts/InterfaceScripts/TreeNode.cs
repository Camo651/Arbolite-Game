using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the data of a tree node
/// </summary>
public class TreeNode : MonoBehaviour
{
	public TreeDisplayer treeDisplayer;
	public TreeNode parentTreeNode;
	public List<TreeNode> childedTreeNodes;
	public Color nodeColour;
	public bool nodeVisible, nodeObtained;
	public UiLineRenderer lineRenderer;
	public Vector3 outgoingLineOffset, incomingLineOffset;

	public void UpdateLine()
	{
		lineRenderer.pos1 = transform.position + outgoingLineOffset;
		if (parentTreeNode)
			lineRenderer.pos2 = parentTreeNode.transform.position + incomingLineOffset;
		else
			lineRenderer.pos2 = lineRenderer.pos1;
		lineRenderer.UpdateLine();
	}
}
