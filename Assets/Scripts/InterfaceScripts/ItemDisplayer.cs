using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To be instanced on the tab, simply displays the items
/// </summary>
public class ItemDisplayer : MonoBehaviour
{
	public GameObject itemUiPrefab;
	public Transform gridLayout;
	public void DisplayItems(List<Item> items)
	{
		for(int i = 0; i < items.Count; i++)
		{
			ItemTag tag = Instantiate(itemUiPrefab, gridLayout).GetComponent<ItemTag>();
			tag.nameBox.text = "";
			tag.icon = null;
			tag.index = i;
		}
	}
}
