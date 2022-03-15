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
	[HideInInspector] public float SPECIES_GrowthStageChance;
	[HideInInspector] public float STYLE_GrowthStageModifier;
	[HideInInspector] public float AGE_GrowthStageModifier;
	[HideInInspector] public float AGE_GrowthScale;
	[HideInInspector] public bool AGE_HasLeaves;
	[HideInInspector] public float AGE_ColorTint;
	[HideInInspector] public float GENERAL_ItemDropMultiplier;
	[HideInInspector] public int QUALITY_itemQuality;
	public List<GameObject> GENERAL_PlantParts;
	[HideInInspector] public float RARITY_Weight;
	[HideInInspector] public int AGE_Value;
	[HideInInspector] public bool RES_IsValidBuildingMaterial;
	public List<SO_Property> GENERAL_Properties;

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
