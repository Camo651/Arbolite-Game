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
				EditorGUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Add Leaf Type"))
					p.GENERAL_PlantParts.Add(null);
				if (GUILayout.Button("Remove Leaf Type") && p.GENERAL_PlantParts.Count > 0)
					p.GENERAL_PlantParts.RemoveAt(p.GENERAL_PlantParts.Count-1);
				EditorGUILayout.EndHorizontal();
				for (int i = 0; i < p.GENERAL_PlantParts.Count; i++)
					p.GENERAL_PlantParts[i] = (GameObject)EditorGUILayout.ObjectField("Leaf Part "+(i+1),p.GENERAL_PlantParts[i],typeof(GameObject),false);
				break;

			case PropertyManager.PropertyType.Rarity:
				p.RARITY_Weight = EditorGUILayout.Slider("Rarity Weight", p.RARITY_Weight,0f,1f);
				break;
			case PropertyManager.PropertyType.Style:
				EditorGUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Add Base Type"))
					p.GENERAL_PlantParts.Add(null);
				if (GUILayout.Button("Remove Base Type") && p.GENERAL_PlantParts.Count > 0)
					p.GENERAL_PlantParts.RemoveAt(p.GENERAL_PlantParts.Count - 1);
				EditorGUILayout.EndHorizontal();
				for (int i = 0; i < p.GENERAL_PlantParts.Count; i++)
					p.GENERAL_PlantParts[i] = (GameObject)EditorGUILayout.ObjectField("Base Part "+(i+1), p.GENERAL_PlantParts[i], typeof(GameObject),false);
				break;
		}
		GUILayout.EndVertical();
	}
}
