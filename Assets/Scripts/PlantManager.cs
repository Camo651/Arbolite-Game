using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// the manager that handles plants being generated, growing, and whatnot
/// </summary>
public class PlantManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;

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
	}

	public PlantPart GetPlantPartPrefab(PlantPart.PartType type)
	{
		if (plantPartCatalog.ContainsKey(type))
		{
			return plantPartCatalog[type][Random.Range(0, plantPartCatalog[type].Count)];
		}
		return null;
	}
}