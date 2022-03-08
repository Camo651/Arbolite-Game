using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the data of a tree node
/// </summary>
[ExecuteAlways]
public class TreeNode : MonoBehaviour
{
	public TreeDisplayer treeDisplayer;
	public TreeNode parentTreeNode;
	public List<TreeNode> childedTreeNodes;
	public Color nodeColour;
	public bool nodeVisible, nodeObtained;
}
