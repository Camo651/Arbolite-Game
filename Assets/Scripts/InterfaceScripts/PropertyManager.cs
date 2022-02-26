using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public GameObject propertyTagPrefab;
	public List<Color> propertyColours;
	public List<Sprite> propertyIcons;
	public Dictionary<string, SO_Property> propertyTagLookup;
	public Dictionary<string, PropertyDisplayer> propertyDisplays;
	public SO_Property defaultProperty;

	private void Awake()
	{
		SO_Property[] unsorted = Resources.LoadAll<SO_Property>("");
		propertyTagLookup = new Dictionary<string, SO_Property>();
		foreach(SO_Property property in unsorted)
		{
			string key = ("prop_" + property.propertyType + "_" + property.callbackID).ToLower();
			if (propertyTagLookup.ContainsKey(key))
			{
				propertyTagLookup[key] = property;
			}
			else
			{
				propertyTagLookup.Add(key, property);
			}
		}

#pragma	warning disable
		PropertyDisplayer[] allDisplayers = (PropertyDisplayer[])FindObjectsOfTypeAll(typeof(PropertyDisplayer));
#pragma warning enable
		propertyDisplays = new Dictionary<string, PropertyDisplayer>();
		foreach(PropertyDisplayer displayer in allDisplayers)
		{
			if (propertyDisplays.ContainsKey(displayer.propertyDisplayCallbackID.ToLower()))
				propertyDisplays[displayer.propertyDisplayCallbackID.ToLower()] = displayer;
			else
				propertyDisplays.Add(displayer.propertyDisplayCallbackID.ToLower(), displayer);
		}
	}

	public enum PropertyType
	{
		None,
		Species,
		Resource,
		Biome,
		Style,
		Rarity,
	}

	public SO_Property GetBiome(string callbackID)
	{
		return propertyTagLookup.ContainsKey("prop_biome_" + callbackID) ? propertyTagLookup["prop_biome_" + callbackID] : null;
	}

	public Color GetColour(PropertyType a)
	{
		return propertyColours[(int)a];
	}
	public Sprite GetIcon(PropertyType a)
	{
		return propertyIcons[(int)a];
	}

	public SO_Property GetProperty(PropertyType type, string indexID)
	{
		return GetProperty("prop_" + type + "_" + indexID);
	}
	public SO_Property GetProperty(string callbackID)
	{
		return propertyTagLookup.ContainsKey(callbackID.ToLower()) ? propertyTagLookup[callbackID.ToLower()] : null;
	}

	public PropertyDisplayer GetPropertyDisplayer(string callbackID)
	{
		return propertyDisplays.ContainsKey(callbackID.ToLower()) ? propertyDisplays[callbackID.ToLower()] : null;
	}
}
