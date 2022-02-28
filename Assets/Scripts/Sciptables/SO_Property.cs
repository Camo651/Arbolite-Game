using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_Property : ScriptableObject
{
	public string callbackID;
	public PropertyManager.PropertyType propertyType;
	[Space(20), Header("Property Attributes")]
	public Color SPECIES_BaseColour;
	public Color SPECIES_LeafColour;
	[Range(0,1)]public float RARITY_Weight;
}
