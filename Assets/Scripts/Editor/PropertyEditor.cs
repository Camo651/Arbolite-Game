using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SO_Property))]
public class PropertyEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EditorUtility.SetDirty(target);
		PrefabUtility.RecordPrefabInstancePropertyModifications(target);
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
				p.SPECIES_GrowthStageChance = EditorGUILayout.Slider("Chance to grow each random tick", p.SPECIES_GrowthStageChance, 0f, 1f);
				break;

			case PropertyManager.PropertyType.Rarity:
				p.RARITY_Weight = EditorGUILayout.Slider("Rarity Weight", p.RARITY_Weight,0f,1f);
				break;
			case PropertyManager.PropertyType.Style:
				p.STYLE_GrowthStageModifier = EditorGUILayout.Slider("Growth speed modifier", p.STYLE_GrowthStageModifier, 0f, 1f);
				p.GENERAL_ItemDropMultiplier = EditorGUILayout.FloatField("Item drop multiplier", p.GENERAL_ItemDropMultiplier);
				break;
			case PropertyManager.PropertyType.Age:
				p.AGE_Value = EditorGUILayout.IntField("Age stage",p.AGE_Value);
				p.AGE_GrowthStageModifier = EditorGUILayout.Slider("Growth speed modifier", p.AGE_GrowthStageModifier, 0f, 1f);
				p.AGE_GrowthScale = EditorGUILayout.Slider("Growth scale at stage", p.AGE_GrowthScale, 0f, 1f);
				p.AGE_ColorTint = EditorGUILayout.Slider("Growth stage colour tint", p.AGE_ColorTint, -1f, 1f);
				p.AGE_HasLeaves = EditorGUILayout.Toggle("Stage has leaves?", p.AGE_HasLeaves);
				p.GENERAL_ItemDropMultiplier = EditorGUILayout.FloatField("Item drop multiplier", p.GENERAL_ItemDropMultiplier);
				break;
			case PropertyManager.PropertyType.Quality:
				p.QUALITY_itemQuality = EditorGUILayout.IntField("Item Quality", p.QUALITY_itemQuality);
				break;
		}
		GUILayout.EndVertical();
	}
}
