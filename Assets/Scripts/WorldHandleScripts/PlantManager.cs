using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// the manager that handles plants being generated, growing, and whatnot
/// </summary>
public class PlantManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public PlantObject defaultPlantPrefab;
	public SO_TreePreset defaultTreePreset;
	public Dictionary<string, SO_TreePreset> treePresets;
	public Dictionary<SO_Property, List<PlantPart>> plantPartCatalog;
	public List<PlantObject> allPlantObjects;
	public int plantDistributionMultiplier;
	private void Awake()
	{
		PlantPart[] unsorted = Resources.LoadAll<PlantPart>("");
		plantPartCatalog = new Dictionary<SO_Property, List<PlantPart>>();
		//foreach (PlantPart part in unsorted)
		//{
		//	if (!plantPartCatalog.ContainsKey(part.propertyDependancy))
		//	{
		//		plantPartCatalog.Add(part.propertyDependancy, new List<PlantPart>());
		//	}
		//	plantPartCatalog[part.propertyDependancy].Add(part);
		//}

		treePresets = new Dictionary<string, SO_TreePreset>();
		SO_TreePreset[] unsortedPresets = Resources.FindObjectsOfTypeAll<SO_TreePreset>();
		foreach (SO_TreePreset item in unsortedPresets)
		{
			if (treePresets.ContainsKey(item.callbackID))
				treePresets[item.callbackID] = item;
			else
				treePresets.Add(item.callbackID, item);
		}
	}


	/// <summary>
	/// Method to generate a plant. All plants should generate from here
	/// </summary>
	/// <param name="_props">A list of properties that should be passed to the plant object</param>
	/// <param name="anchor">The RoomTile that the plant should generate on</param>
	/// <returns>The newly generated plant object</returns>
	public PlantObject GeneratePlant(List<SO_Property> _props, RoomTile anchor)
	{
		List<SO_Property> properties = new List<SO_Property>();
		properties.AddRange(_props);
		SO_Property _age = globalRefManager.propertyManager.GetProperty(PropertyManager.PropertyType.Age, "age0");
		properties.Add(_age);
		PlantObject newPlant = Instantiate(defaultPlantPrefab.gameObject, anchor.transform).GetComponent<PlantObject>();
		newPlant.roomTile = anchor;
		newPlant.plantProperties = properties;
		newPlant.GeneratePlant();
		allPlantObjects.Add(newPlant);
		return newPlant;
	}

	/// <summary>
	/// Safely destroys a plant object
	/// </summary>
	/// <param name="plant">The plant to be destroyed</param>
	public void DestroyPlant(PlantObject plant)
	{
		plant.roomTile.thisRoomsPlant = null;
		allPlantObjects.Remove(plant);
		Destroy(plant.gameObject);
	}

	/// <summary>
	/// Get a random plant part based on its type
	/// </summary>
	/// <param name="type"></param>
	/// <returns>A plant part</returns>
	public GameObject GetRandomPlantPartPrefab(SO_Property type)
	{
		if (plantPartCatalog.ContainsKey(type))
		{
			return plantPartCatalog[type][Random.Range(0, plantPartCatalog[type].Count)].gameObject;
		}
		return null;
	}

	/// <summary>
	/// Get a tree preset from the lookup table
	/// </summary>
	/// <param name="callbackID">The callback ID of the preset</param>
	/// <returns>The tree preset, given it exists</returns>
	public SO_TreePreset GetTreePreset(string callbackID)
	{
		return treePresets.ContainsKey(callbackID) ? treePresets[callbackID] : defaultTreePreset;
	}

	/// <summary>
	/// Get a random tree type based on its rarity
	/// May become a slightly expensive way to get presets if there are a lot of types in the future
	/// </summary>
	/// <param name="biomeConditional">The biome it should be located in (can be nulled if it doesnt matter)</param>
	/// <returns>A tree preset, given there are any for that biome</returns>
	public SO_TreePreset GetRandomTreePresetWeighted(SO_Property biomeConditional)
	{
		List<SO_TreePreset> weights = new List<SO_TreePreset>();
		foreach (SO_TreePreset preset in treePresets.Values)
		{
			SO_Property biome = globalRefManager.propertyManager.GetPropertyFromType(preset.plantProperties, PropertyManager.PropertyType.Biome);
			if (biomeConditional == null || biome == biomeConditional)
			{
				SO_Property rarityProp = globalRefManager.propertyManager.GetPropertyFromType(preset.plantProperties, PropertyManager.PropertyType.Rarity);
				float rarity = rarityProp ? rarityProp.RARITY_Weight : 0f;
				for (int i = 0; i < rarity * plantDistributionMultiplier; i++)
				{
					weights.Add(preset);
				}
			}
		}
		return weights.Count > 0 ? weights[Random.Range(0, weights.Count)] : defaultTreePreset;
	}

	/// <summary>
	/// Tries to age each plant on a random cycle
	/// </summary>
	public void PlantAgingRandomTick()
	{
		for (int i = 0; i < allPlantObjects.Count; i++)
		{
			if (allPlantObjects[i])
			{
				allPlantObjects[i].TryGrowPlant();
			}
			else
			{
				allPlantObjects.RemoveAt(i);
			}
		}
	}

	public void HarvestSelectedPlant(bool fullyDestroy)
	{
		PlantObject toHarvest = globalRefManager.baseManager.editModePermSelectedRoomTile.thisRoomsPlant;
		HarvestPlant(toHarvest, fullyDestroy);
	}

	/// <summary>
	/// Harvests the currently selected plant object
	/// </summary>
	/// <param name="fullyDestroy">Should the plant attempt to save the base plant and harvest or simply remove it all together?</param>
	public void HarvestPlant(PlantObject toHarvest, bool fullyDestroy)
	{
		if (toHarvest)
		{
			List<Item> resourceGain = toHarvest.GetPlantResources(!fullyDestroy && toHarvest.canBeHarvestedWithoutDestroy);
			if (globalRefManager.itemManager.mainInventory.CanHoldXMoreItems(resourceGain.Count))
			{
				globalRefManager.itemManager.mainInventory.AddItemsToContainer(resourceGain);

				if (toHarvest.canBeHarvestedWithoutDestroy && !fullyDestroy)
				{
					toHarvest.HarvestWithoutDestroy();
				}
				else
				{
					DestroyPlant(toHarvest);
				}
				globalRefManager.statisticsManager.GetStat("plants_harvested").AddStatValue(1);
				globalRefManager.interfaceManager.CloseAllInterfaces();
			}
		}
	}


	private void FixedUpdate()
	{
		if (Random.value < .005 && !globalRefManager.baseManager.gameIsActivelyFrozen)
		{
			PlantAgingRandomTick();
		}
	}
}