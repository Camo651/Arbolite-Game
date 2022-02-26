using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// the manager that handles plants being generated, growing, and whatnot
/// </summary>
public class PlantManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public GameObject defaultPlantPrefab;
	public SO_TreePreset defaultTreePreset;
	public Dictionary<string, SO_TreePreset> treePresets;
	public Dictionary<PlantPart.PartType, List<PlantPart>> plantPartCatalog;

	private void Awake()
	{
		PlantPart[] unsorted = Resources.LoadAll<PlantPart>("");
		plantPartCatalog = new Dictionary<PlantPart.PartType, List<PlantPart>>();
		foreach (PlantPart part in unsorted)
		{
			if (!plantPartCatalog.ContainsKey(part.partType))
			{
				plantPartCatalog.Add(part.partType, new List<PlantPart>());
			}
			plantPartCatalog[part.partType].Add(part);
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


	public Color GetColourLookup(Dictionary<PlantPart.PartType, Color> l, PlantPart.PartType t)
	{
		float o = Random.Range(-1, 2) / 40f;
		return l.ContainsKey(t) ? (l[t] + new Color(o,o,o,0f)) : Color.white;
	}

	/// <summary>
	/// Gets the part from the array based on its indexer in the enum
	/// </summary>
	/// <param name="parts"></param>
	/// <param name="indexer"></param>
	/// <returns>A list idk</returns>
	public List<PlantPart.PartType> GetPartTypesFromRange(List<PlantPart.PartType> parts, PlantPart.PartTypeRangeIndexer indexer)
	{
		List<PlantPart.PartType> a = new List<PlantPart.PartType>();
		foreach(PlantPart.PartType item in parts)
		{
			if((int)item >= (int)indexer && (int)item < ((int)item+1000))
			{
				a.Add(item);
			}
		}
		return a;
	}

	/// <summary>
	/// Get a random plant part based on its type
	/// </summary>
	/// <param name="type"></param>
	/// <returns>A plant part</returns>
	public GameObject GetRandomPlantPartPrefab(PlantPart.PartType type)
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