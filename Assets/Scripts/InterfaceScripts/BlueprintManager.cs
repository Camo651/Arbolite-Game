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

	public void UseBlueprintInWorld(bool consumed)
	{
		if (consumed)
		{
			filledBlueprints.Remove(selectedBlueprint);
			Destroy(selectedBlueprint.gameObject);
			selectedBlueprint = null;
		}
	}
}
