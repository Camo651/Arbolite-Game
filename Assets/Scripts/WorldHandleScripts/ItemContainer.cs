using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
	public ItemManager itemManager;
	public List<Item> itemsInContainer = new List<Item>();

	public List<List<Item>> GetItemsGrouped()
	{
		//Dictionary<string, List<Item>> sort;
		//make a string of the props for each item
		//check if in dict, add, else, add new
		//retun vals
		return null;
	}

	public void MoveItemToContainer(Item item, ItemContainer oldContainer)
	{
		itemsInContainer.Add(item);

		if (oldContainer)
			oldContainer.itemsInContainer.Remove(item);

		if (!itemManager.allItem.Contains(item))
			itemManager.allItem.Add(item);
	}


	public void DestroyItemPerm(Item item)
	{
		itemsInContainer.Remove(item);
		itemManager.allItem.Remove(item);
		Destroy(item.gameObject);
	}
}
