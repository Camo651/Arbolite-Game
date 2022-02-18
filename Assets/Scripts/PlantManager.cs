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

	public Dictionary<PlantPart.PartType, List<PlantPart>> plantPartCatalog;

	private void Start()
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
		defaultPlantPrefab = (GameObject)Resources.Load("Assets/Prefabs/Default/DefaultProceduralPlant.prefab");
	}

	/// <summary>
	/// Handles the generation of an actual plant
	/// </summary>
	/// <returns>The newley generated plant</returns>
	public ProceduralPlant GenerateNewPlant(ProceduralPlant.PlantType _plantType, PlantPart.PartType _baseType, SO_BiomeType _biome, object[][] _resources, PlantPart.LeafType[] _leaves)
	{
		//make plant
		//set all the genes
		//generate the parts
		//finish the parts based on genes

		ProceduralPlant newPlant = Instantiate(defaultPlantPrefab).GetComponent<ProceduralPlant>();

		newPlant.plantType = _plantType;
		newPlant.basePartType = _baseType;
		newPlant.plantBiome = _biome;
		newPlant.resourceComposition = _resources;
		newPlant.leafTypes = _leaves;

		newPlant.plantParts = new List<PlantPart>();
		Instantiate(GetRandomPlantPartPrefab(newPlant.basePartType).gameObject).GetComponent<PlantPart>();

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
}