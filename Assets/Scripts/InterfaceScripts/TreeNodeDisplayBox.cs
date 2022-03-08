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

		//if (childrenSpriteLine && c)
		//{
		//	childrenSpriteLine.GetComponent<Image>().color = treeDisplayer.nodeStateColours[(int)d.nodeState];
		//	Vector2 pos1 = transform.position;
		//	Vector2 pos2 = new Vector2(pos1.x, c.sideLine.transform.position.y);
		//	childrenSpriteLine.position = Vector2.Lerp(pos1, pos2, .5f);
		//	childrenSpriteLine.sizeDelta = new Vector2(3f,(pos1.y-pos2.y));
		//}else if (!c)
		//{
		//	childrenSpriteLine.sizeDelta = Vector2.zero;
		//}
	}
}
