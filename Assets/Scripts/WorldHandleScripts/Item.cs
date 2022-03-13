using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
	public ItemContainer itemContainer;
	public string itemName;
	public Sprite itemIcon;
	public bool isValidBuildingMaterial;
	public List<SO_Property> itemProperties;
	public bool itemExists;

	public Item(ItemContainer container)
	{
		itemExists = true;
		itemContainer = container;
		itemProperties = new List<SO_Property>();
	}


	public void SetItemProperties(List<SO_Property> p)
	{
		itemProperties.AddRange(p);
	}
}
