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

	/// <summary>
	/// Move a pre-existing item from oldContainer to this one
	/// </summary>
	/// <param name="item">The item to be moved</param>
	/// <param name="oldContainer">The item's current container</param>
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

	/// <summary>
	/// Add a list of items to this container that are not currently in a container
	/// </summary>
	/// <param name="items">The items to be added</param>
	public void AddItemsToContainer(List<Item> items)
	{
		foreach (Item item in items)
		{
			AddItemsToContainer(item);
		}
	}

	/// <summary>
	/// Add a single item to this container that is not currently in a container
	/// </summary>
	/// <param name="items">The item to be added</param>
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

	/// <summary>
	/// Deletes the item from existence completely
	/// </summary>
	/// <param name="item">The item to be deleted</param>
	public void DestroyItemPerm(Item item)
	{
		itemsInContainer.Remove(item);
		itemManager.allItem.Remove(item);
		item.itemExists = false;
	}

	/// <summary>
	/// Get the translated string to display the capacity of this container
	/// </summary>
	/// <returns>The container capacity (xxx/xxx Items)</returns>
	public string GetCount()
	{
		return itemsInContainer.Count + " / " + containerCapacity + " " + itemManager.globalRefManager.langManager.GetTranslation("items_counter");
	}

	/// <summary>
	/// Can this container hold x more items?
	/// </summary>
	/// <param name="amount">The amount to be added</param>
	/// <returns>If the container is capable of holding amount more items</returns>
	public bool CanHoldXMoreItems(int amount)
	{
		return itemsInContainer.Count + amount <= containerCapacity;
	}
}
