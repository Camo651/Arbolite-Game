using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_Property : ScriptableObject
{
	public string callbackID;
	public PropertyManager.PropertyType propertyType;
	[Space(20), Header("Property Attributes")]
	[HideInInspector] public Color SPECIES_BaseColour;
	[HideInInspector] public Color SPECIES_LeafColour;
	[HideInInspector] public List<GameObject> GENERAL_PlantParts;
	[HideInInspector] public float RARITY_Weight;

	public List<PlantPart> GetPlantParts()
	{
		List<PlantPart> p = new List<PlantPart>();
		foreach (GameObject item in GENERAL_PlantParts)
		{
			p.Add(item.GetComponent<PlantPart>());
		}
		return p;
	}
}
