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

	public PropertyDisplayer itemHighlightPropertyDisp;

	[Header("These are for the bp inspector only")]
	public BlueprintManager blueprintManager;
	public GameObject validSelectionmButton;
	public TMPro.TextMeshProUGUI itemCountBox;
	public void DisplayItems(List<Item> items, string counter)
	{
		for (int i = 0; i < gridLayout.childCount; i++)
		{
			Destroy(gridLayout.GetChild(i).gameObject);
		}
		for(int i = 0; i < items.Count; i++)
		{
			ItemTag tag = Instantiate(itemUiPrefab, gridLayout).GetComponent<ItemTag>();
			tag.nameBox.text = items[i].itemName;
			tag.icon.sprite = items[i].itemIcon;
			tag.index = i;
			tag.displayer = this;
			tag.itemInfo = items[i];
		}
		System.GC.Collect();
		if(validSelectionmButton)
			validSelectionmButton.SetActive(false);
		if(itemCountBox)
			itemCountBox.text = counter;
	}

	public void HighlightItem(Item i)
	{
		if (blueprintManager)
		{
			blueprintManager.selectedItemToFillBlueprintWith = i;
			validSelectionmButton.SetActive(true);
			return;
		}

		if (!itemHighlightPropertyDisp)
			return;
		itemHighlightPropertyDisp.propertyManager.globalRefManager.interfaceManager.inspectorItemHighlight.OpenHighlight(i.GetHashCode(), i.itemName, itemHighlightPropertyDisp.propertyManager.globalRefManager.langManager.GetTranslation("item_subtitle"), "", i.itemIcon, Color.white);
		itemHighlightPropertyDisp.DisplayProperties(i.itemProperties);
	}
}
