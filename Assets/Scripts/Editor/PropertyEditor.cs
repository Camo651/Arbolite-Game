using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SO_Property))]
public class PropertyEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		SO_Property p = (SO_Property)target;
		GUILayout.BeginVertical();
		GUILayout.Space(30);
		GUILayout.Label("Property Values:");
		GUILayout.Space(5);
		switch (p.propertyType)
		{
			case PropertyManager.PropertyType.None:
				break;

			case PropertyManager.PropertyType.Species:
				p.SPECIES_BaseColour = EditorGUILayout.ColorField("Base Colour", p.SPECIES_BaseColour + new Color(0, 0, 0, 1f));
				p.SPECIES_LeafColour = EditorGUILayout.ColorField("Leaf Colour", p.SPECIES_LeafColour + new Color(0, 0, 0, 1f));
				p.SPECIES_PlantPart = (PlantPart)EditorGUILayout.ObjectField("Plant Part",p.SPECIES_PlantPart,typeof(PlantPart));
				break;

			case PropertyManager.PropertyType.Rarity:
				p.RARITY_Weight = EditorGUILayout.Slider("Rarity Weight", p.RARITY_Weight,0f,1f);
				break;
		}
		GUILayout.EndVertical();
	}
}
