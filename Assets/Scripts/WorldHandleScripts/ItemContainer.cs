using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
	public ItemManager itemManager;
	public int containerCapacity;
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
		if (itemsInContainer.Count == containerCapacity)
		{
			itemManager.globalRefManager.interfaceManager.EnqueueNotification("container_full", "", null);
		}
		itemsInContainer.Add(item);

		if (oldContainer)
			oldContainer.itemsInContainer.Remove(item);

		if (!itemManager.allItem.Contains(item))
			itemManager.allItem.Add(item);
	}

	public void AddItemsToContainer(List<Item> items)
	{
		foreach (Item item in items)
		{
			AddItemsToContainer(item);
		}
	}
	public void AddItemsToContainer(Item item)
	{
		if (itemsInContainer.Count == containerCapacity)
		{
			itemManager.globalRefManager.interfaceManager.EnqueueNotification("container_full", "", null);
		}
		itemsInContainer.Add(item);
		if (!itemManager.allItem.Contains(item))
			itemManager.allItem.Add(item);
	}


	public void DestroyItemPerm(Item item)
	{
		itemsInContainer.Remove(item);
		itemManager.allItem.Remove(item);
		Destroy(item.gameObject);
	}

	public string GetCount()
	{
		return itemsInContainer.Count + " / " + containerCapacity + " " + itemManager.globalRefManager.langManager.GetTranslation("items_counter");
	}
}
