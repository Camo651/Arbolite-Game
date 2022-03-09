using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public List<Blueprint> allBlueprints;
	public List<Blueprint> filledBlueprints;
	public Color lockedBlueprintColour, unlockedBluebrintColour, newBlueprintColour;
	public GameObject blueprintGridLayout, buildingGridLayout;
	public Blueprint selectedBlueprint;
	public Item selectedItemToFillBlueprintWith;
	public UserInterface blueprintInspectorModal, resourceSelectorModal;

	/// <summary>
	/// Called upon the opening of the buildng tab being opened.
	/// Initizlizes the values of the blueprints and sets their displays
	/// </summary>
	public void Initialize()
	{
		if (allBlueprints == null || allBlueprints.Count == 0)
		{
			allBlueprints = new List<Blueprint>();
			allBlueprints.AddRange(blueprintGridLayout.GetComponentsInChildren<Blueprint>(true));
			foreach (Blueprint blueprint in allBlueprints)
			{
				if (blueprint.blueprintManager == null)
					blueprint.blueprintManager = this;
				blueprint.titleBox.text = blueprint.GetBlueprintName();
			}
			SortBlueprints();
		}
	}

	/// <summary>
	/// Sorts all the blueprints to show new, then unlocked, then locked.
	/// </summary>
	public void SortBlueprints()
	{
		List<Blueprint> n = new List<Blueprint>();
		List<Blueprint> u = new List<Blueprint>();
		List<Blueprint> l = new List<Blueprint>();
		foreach(Blueprint b in allBlueprints)
		{
			if (b.isLocked)
			{
				l.Add(b);
				b.background.color = lockedBlueprintColour;
			}
			else if (b.hasBeenSeenUnlocked)
			{
				u.Add(b);
				b.background.color = unlockedBluebrintColour;
			}
			else
			{
				n.Add(b);
				b.background.color = newBlueprintColour;
			}
		}
		allBlueprints.Clear();
		allBlueprints.AddRange(n);
		allBlueprints.AddRange(u);
		allBlueprints.AddRange(l);
		for(int i = 0; i < allBlueprints.Count; i++)
		{
			allBlueprints[i].transform.SetSiblingIndex(i);
		}
	}

	/// <summary>
	/// Attempts to fill the currently selected blueprint with an item value, then move it to the building tab
	/// </summary>
	public void FillBlueprint()
	{
		if (selectedBlueprint && selectedItemToFillBlueprintWith)
		{
			Blueprint a = Instantiate(selectedBlueprint, buildingGridLayout.transform).GetComponent<Blueprint>();
			filledBlueprints.Add(a);
			a.isFilled = true;
			a.hasBeenSeenUnlocked = true;
			a.isLocked = false;
			a.properties = selectedItemToFillBlueprintWith.itemProperties;
			selectedItemToFillBlueprintWith.itemContainer.DestroyItemPerm(selectedItemToFillBlueprintWith);
			selectedItemToFillBlueprintWith = null;
			a.transform.SetAsFirstSibling();
			a.background.color = unlockedBluebrintColour;
		}
	}

	/// <summary>
	/// Selects or deselects the blueprint sel
	/// </summary>
	/// <param name="sel">The blueprint to be selected</param>
	public void SelectBlueprint(Blueprint sel)
	{
		selectedBlueprint = sel;
		if (!sel)
		{
			blueprintInspectorModal.gameObject.SetActive(false);
			globalRefManager.interfaceManager.CloseAllInterfaces();
			return;
		}
		if (sel.isLocked)
			return;

		blueprintInspectorModal.gameObject.SetActive(true);
		blueprintInspectorModal.interfaceName.text = sel.GetBlueprintName();
		blueprintInspectorModal.interfaceDescription.text = sel.GetBlueprintInfo();

	}

	/// <summary>
	/// Called from inspector when the 'Use' button of the blueprint inspector modal is pressed
	/// Can either display the valid building materials in the resource highlight, or set the player to
	/// build mode and close the UI so they can place the tile
	/// </summary>
	public void OnClickUseOnBlueprintInspectorModal()
	{
		if (!selectedBlueprint.isFilled)
		{
			resourceSelectorModal.gameObject.SetActive(true);
			ItemDisplayer resDisp = resourceSelectorModal.GetComponent<ItemDisplayer>();
			List<Item> validResources = new List<Item>();
			foreach (Item item in globalRefManager.itemManager.allItem)
			{
				if (item && item.isValidBuildingMaterial)
				{
					validResources.Add(item);
				}
			}
			resDisp.DisplayItems(validResources, "");
		}
		else
		{
			globalRefManager.baseManager.selectedRoomToBuild = selectedBlueprint.GetContainedRoom();
			globalRefManager.baseManager.SetPlayerState(BaseManager.PlayerState.BuildMode);
			globalRefManager.interfaceManager.CloseAllInterfaces();
		}
	}

	/// <summary>
	/// Deselects and removes the current blueprint or just deselects it
	/// </summary>
	/// <param name="consumed">Should the current blueprint be used up and removed or just placed and ignored</param>
	public void UseBlueprintInWorld(bool consumed)
	{
		if (consumed)
		{
			filledBlueprints.Remove(selectedBlueprint);
			Destroy(selectedBlueprint.gameObject);
		}
		selectedBlueprint = null;
	}
}
