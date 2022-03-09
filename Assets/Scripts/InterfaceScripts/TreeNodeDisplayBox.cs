using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeNodeDisplayBox : MonoBehaviour
{
	public TreeDisplayer treeDisplayer;
	public TreeNodeDisplayBox parentTreeNodeDisplay;
	public List<TreeNodeDisplayBox> childedDisplayNodes;
	public Image backGroundImage;
	public RectTransform childrenSpriteLine;
	public Image sideLine;
	public GameObject childHolder;
	public DataNode dataNode;


	public void SetNodeDisplay(DataNode d, TreeNodeDisplayBox parent, int depth)
	{
		if (parent)
		{
			parentTreeNodeDisplay = parent;
			treeDisplayer = parent.treeDisplayer;
			transform.localPosition = new Vector3(0, -75 * (transform.GetSiblingIndex()+1f), 0f);
			transform.GetChild(0).localPosition = Vector3.right * 100f * depth;

		}

		dataNode = d;
		backGroundImage.color = treeDisplayer.nodeStateColours[(int)d.nodeState];
		sideLine.color = treeDisplayer.nodeStateColours[(int)d.nodeState];
		TreeNodeDisplayBox c = null;
		for (int i = 0; i < d.childedDataNodes.Count; i++)
		{
			c = Instantiate(treeDisplayer.treeManager.defaultTreeNodeDisplayBoxPrefab, treeDisplayer.displayRootNode.childHolder.transform).GetComponent<TreeNodeDisplayBox>();
			childedDisplayNodes.Add(c);
			c.SetNodeDisplay(d.childedDataNodes[i], this, depth+1);
		}
	}

	public void SetHighlight(bool state)
	{
		treeDisplayer.SetHighlightDisplay(state, dataNode.GetNodeName(), dataNode.GetNodeInfo(), dataNode.GetIcon(), dataNode.nodeState, new Vector3(transform.position.x +250, transform.position.y, 0f));
	}
}
