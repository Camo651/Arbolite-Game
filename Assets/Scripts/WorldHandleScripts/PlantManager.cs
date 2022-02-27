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

	public SO_TreePreset GetTreePreset(string callbackID)
	{
		return treePresets.ContainsKey(callbackID) ? treePresets[callbackID] : defaultTreePreset;
	}
}