using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	public ItemContainer itemContainer;
	public string itemName;
	public Sprite itemIcon;
	public bool isValidBuildingMaterial;
	public List<SO_Property> itemProperties;
}
