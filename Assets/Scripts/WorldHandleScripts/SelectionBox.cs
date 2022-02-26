using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBox : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public Queue<GameObject> activeNodes = new Queue<GameObject>(), inactiveNodes = new Queue<GameObject>();
	public GameObject selectionNodePrefab;
	public float nodesPerSide;
	public Color boxColour;
	public void SetSelection(ContainedRoom cont)
	{
		ClearSelection();
		GenerateBordure(new Vector2(cont.transform.position.x-.5f, cont.transform.position.y - .5f), new Vector2(cont.transform.position.x-.5f, cont.transform.position.y-.5f) + cont.roomDimensions);
	}
	public void GenerateBordure(Vector2 bottomLeft, Vector2 topRight)
	{
		//set top and bottom lines, then left and right w/o corners
		for (int i = 0; i < nodesPerSide+1; i++)
		{
			float normal = i / nodesPerSide;
			SetNode(new Vector2(Mathf.Lerp(bottomLeft.x, topRight.x, normal), topRight.y));
			SetNode(new Vector2(Mathf.Lerp(bottomLeft.x, topRight.x, normal), bottomLeft.y));
			if(i > 0 && i < nodesPerSide)
			{
				SetNode(new Vector2(bottomLeft.x, Mathf.Lerp(bottomLeft.y, topRight.y, normal)));
				SetNode(new Vector2(topRight.x, Mathf.Lerp(bottomLeft.y, topRight.y, normal)));
			}
		}
	}
	public void SetNode(Vector2 pos)
	{
		if (inactiveNodes.Count == 0)
		{
			GameObject a = Instantiate(selectionNodePrefab, transform);
			a.GetComponent<SpriteRenderer>().color = boxColour;
			inactiveNodes.Enqueue(a);
		}
		GameObject node = inactiveNodes.Dequeue();
		node.transform.position = pos;
		node.SetActive(true);
		activeNodes.Enqueue(node);
	}
	public void ClearSelection()
	{
		while(activeNodes.Count > 0)
		{
			activeNodes.Peek().SetActive(false);
			activeNodes.Peek().transform.position = Vector3.zero;
			inactiveNodes.Enqueue(activeNodes.Dequeue());
		}
		activeNodes.Clear();
	}
}
