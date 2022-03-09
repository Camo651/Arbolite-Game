using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public ItemContainer mainInventory;
	public List<ItemContainer> allItemContainers = new List<ItemContainer>();
	public List<Item> allItem = new List<Item>();
}
