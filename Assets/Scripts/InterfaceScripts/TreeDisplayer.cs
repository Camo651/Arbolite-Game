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


	public void DisplayTree()
	{
		for (int i = 0; i < displayRootNode.childHolder.transform.childCount; i++)
		{
			Destroy(displayRootNode.childHolder.transform.GetChild(i));
		}
		displayRootNode.treeDisplayer = this;
		displayRootNode.SetNodeDisplay(dataRootNode, null,-1);
	}
}
