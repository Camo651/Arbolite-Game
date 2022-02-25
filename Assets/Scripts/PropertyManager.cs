using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public List<Color> propertyColours;

	public enum PropertyType
	{
		None,
		Species,
		Resource,
		Biome,
		Style,
	}

	public Color GetColour(PropertyType a)
	{
		return propertyColours[(int)a];
	}
}
