using System.Collections;
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
	public int plantDistributionMultiplier;
	private void Awake()
	{
		PlantPart[] unsorted = Resources.LoadAll<PlantPart>("");
		plantPartCatalog = new Dictionary<SO_Property, List<PlantPart>>();
		foreach (PlantPart part in unsorted)
		{
			if (!plantPartCatalog.ContainsKey(part.propertyDependancy))
			{
				plantPartCatalog.Add(part.propertyDependancy, new List<PlantPart>());
			}
			plantPartCatalog[part.propertyDependancy].Add(part);
		}

		treePresets = new Dictionary<string, SO_TreePreset>();
		SO_TreePreset[] unsortedPresets = Resources.FindObjectsOfTypeAll<SO_TreePreset>();
		foreach(SO_TreePreset item in unsortedPresets)
		{
			if (treePresets.ContainsKey(item.callbackID))
				treePresets[item.callbackID] = item;
			else
				treePresets.Add(item.callbackID, item);
		}
	}


	public PlantObject GeneratePlant(List<SO_Property> properties, RoomTile anchor)
	{
		PlantObject newPlant = Instantiate(defaultPlantPrefab.gameObject, anchor.transform).GetComponent<PlantObject>();
		newPlant.roomTile = anchor;
		newPlant.plantProperties = properties;
		newPlant.GeneratePlant();
		return newPlant;
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
		foreach(SO_TreePreset preset in treePresets.Values)
		{
			SO_Property biome = GetPropertyFromType(preset.plantProperties, PropertyManager.PropertyType.Biome);
			if (biomeConditional == null || biome == biomeConditional)
			{
				SO_Property rarityProp = GetPropertyFromType(preset.plantProperties, PropertyManager.PropertyType.Rarity);
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
	/// Get the property from the type in this object
	/// </summary>
	/// <param name="pl">The list of properties in the object</param>
	/// <param name="type">The property type</param>
	/// <returns>The property type in the object, given it exists</returns>
	public SO_Property GetPropertyFromType(List<SO_Property> pl, PropertyManager.PropertyType type)
	{
		foreach (SO_Property p in pl)
		{
			if (p.propertyType == type)
			{
				return p;
			}
		}
		return null;
	}
}