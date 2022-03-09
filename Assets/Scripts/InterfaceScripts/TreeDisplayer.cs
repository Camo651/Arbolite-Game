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
	public GameObject NodeInfoHighlight;


	public void DisplayTree()
	{
		SetHighlightDisplay(false, "", "", null, DataNode.NodeState.Locked, Vector3.zero);
		for (int i = 0; i < displayRootNode.childHolder.transform.childCount; i++)
		{
			Destroy(displayRootNode.childHolder.transform.GetChild(i));
		}
		displayRootNode.treeDisplayer = this;
		displayRootNode.SetNodeDisplay(dataRootNode, null,-1);
	}

	public void SetHighlightDisplay(bool state, string title, string desc, Sprite icon, DataNode.NodeState node, Vector3 pos)
	{
		NodeInfoHighlight.SetActive(state);
		NodeInfoHighlight.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = title;
		NodeInfoHighlight.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = desc;
		NodeInfoHighlight.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = node+"";
		NodeInfoHighlight.transform.GetChild(3).GetComponent<UnityEngine.UI.Image>().sprite = icon;
		NodeInfoHighlight.transform.position = pos;
	}
}
