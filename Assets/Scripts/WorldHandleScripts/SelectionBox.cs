using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBox : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public List<GameObject> activeNodes = new List<GameObject>(), inactiveNodes = new List<GameObject>();
	public GameObject selectionNodePrefab;
	public float nodesPerSide;
	public Color boxColour;

	/// <summary>
	/// Sets the selection box around a room
	/// </summary>
	/// <param name="cont">The room to select. Not nullable</param>
	public void SetSelection(ContainedRoom cont)
	{
		ClearSelection();
		GenerateBordure(new Vector2(cont.transform.position.x-.5f, cont.transform.position.y - .5f), new Vector2(cont.transform.position.x-.5f, cont.transform.position.y-.5f) + cont.roomDimensions);
	}

	/// <summary>
	/// Generates the dotted line around the room
	/// </summary>
	/// <param name="bottomLeft">The bottom left most corner of the room tiles</param>
	/// <param name="topRight">The top right most corner of the room tiles</param>
	public void GenerateBordure(Vector2 bottomLeft, Vector2 topRight)
	{

		for (int x = 0; x < nodesPerSide * Mathf.Abs(topRight.x-bottomLeft.x) + 1; x++)
		{
			float normal = x / (nodesPerSide * Mathf.Abs(topRight.x - bottomLeft.x));
			SetNode(new Vector2(Mathf.Lerp(bottomLeft.x, topRight.x, normal), topRight.y));
			SetNode(new Vector2(Mathf.Lerp(bottomLeft.x, topRight.x, normal), bottomLeft.y));
		}

		for (int x = 0; x < nodesPerSide * Mathf.Abs(topRight.y - bottomLeft.y); x++)
		{
			float normal = x / (nodesPerSide * Mathf.Abs(topRight.y - bottomLeft.y));
			if (x > 0 && x < (nodesPerSide * Mathf.Abs(topRight.y - bottomLeft.y)))
			{
				SetNode(new Vector2(bottomLeft.x, Mathf.Lerp(bottomLeft.y, topRight.y, normal)));
				SetNode(new Vector2(topRight.x, Mathf.Lerp(bottomLeft.y, topRight.y, normal)));
			}
		}
	}

	private void FixedUpdate()
	{
		if(activeNodes.Count > 0 && !globalRefManager.baseManager.gameIsActivelyFrozen)
		{
			foreach (GameObject item in activeNodes)
			{
				item.transform.localScale = Vector3.one * .075f * (Mathf.Abs(Mathf.Sin(Time.time)) + .5f);
			}
		}
	}

	/// <summary>
	/// Sets a node at a position. Makes a new node if there is not enough spares
	/// </summary>
	/// <param name="pos">The position to be set to</param>
	public void SetNode(Vector2 pos)
	{
		if (inactiveNodes.Count == 0)
		{
			GameObject a = Instantiate(selectionNodePrefab, transform);
			a.GetComponent<SpriteRenderer>().color = boxColour;
			inactiveNodes.Add(a);
		}
		GameObject node = inactiveNodes[0];
		inactiveNodes.RemoveAt(0);
		node.transform.position = pos;
		node.SetActive(true);
		activeNodes.Add(node);
	}

	/// <summary>
	/// Removes the current selection box
	/// </summary>
	public void ClearSelection()
	{
		while(activeNodes.Count > 0)
		{
			activeNodes[0].SetActive(false);
			activeNodes[0].transform.position = Vector3.zero;
			inactiveNodes.Add(activeNodes[0]);
			activeNodes.RemoveAt(0);
		}
		activeNodes.Clear();
	}
}
