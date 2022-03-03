using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Not a manager class. Can be instanced multilple times
/// </summary>
public class TreeHandler : MonoBehaviour
{
	public TreeManager treeManager;
	public string treeCallbackID;
	public List<TreeNode> allTreeNodesInTree;

	public void AddNode(TreeNode parent)
	{
		TreeNode newNode = Instantiate(treeManager.TreeNodePrefab, parent.transform).GetComponent<TreeNode>();
		parent.childedTreeNodes.Add(newNode);
		newNode.parentTreeNode = parent;
	}
}
