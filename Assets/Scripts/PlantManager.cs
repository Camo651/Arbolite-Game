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
		PlantPart[] unsorted = Resources.FindObjectsOfTypeAll<PlantPart>();
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

	/// <summary>
	/// Handles the generation of an actual plant
	/// </summary>
	/// <returns>The newley generated plant</returns>
	public ProceduralPlant GenerateNewPlant(RoomTile _parent, ProceduralPlant.PlantType _plantType, PlantPart.BaseType _baseType, SO_BiomeType _biome, List<SO_ResourceType> _resourceTypes, List<int> _resourceCounts, PlantPart.LeafType[] _leaves)
	{
		//make plant
		//set all the genes
		//generate the parts
		//finish the parts based on genes

		ProceduralPlant newPlant = Instantiate(defaultPlantPrefab).GetComponent<ProceduralPlant>();

		newPlant.plantType = _plantType;
		newPlant.basePartType = _baseType;
		newPlant.plantBiome = _biome;
		newPlant.plantResourceComposition = _resourceTypes;
		newPlant.resourceCompositionDistribution = _resourceCounts;
		newPlant.leafTypes = _leaves;
		newPlant.plantParts = new List<PlantPart>();

		newPlant.transform.SetParent(_parent.transform);

		//do the generation stuff herer

		return newPlant;
	}

	/// <summary>
	/// Get a random plant part based on its type
	/// </summary>
	/// <param name="type"></param>
	/// <returns>A plant part</returns>
	public PlantPart GetRandomPlantPartPrefab(PlantPart.PartType type)
	{
		if (plantPartCatalog.ContainsKey(type))
		{
			return plantPartCatalog[type][Random.Range(0, plantPartCatalog[type].Count)];
		}
		return null;
	}

	
	public SO_TreePreset GetTreePreset(string callbackID)
	{
		return treePresets.ContainsKey(callbackID) ? treePresets[callbackID] : defaultTreePreset;
	}
}