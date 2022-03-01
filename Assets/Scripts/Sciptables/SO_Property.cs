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
	[HideInInspector] public PlantPart SPECIES_PlantPart;
	[HideInInspector] public float RARITY_Weight;
}
