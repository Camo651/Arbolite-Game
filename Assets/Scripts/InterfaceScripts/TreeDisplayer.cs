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
	public List<TreeNode> allTreeNodesInTree;

}
