using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SO_TreePreset))]
public class TreePresetEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		SO_TreePreset a = (SO_TreePreset)target;

		GUILayout.Space(15);
		GUILayout.Label("Resource Distribution");
		if(GUILayout.Button("Add New Resource"))
		{
			a._resourcesTypes.Add(null);
			a._resourcesDistributions.Add(0);
		}
		if(GUILayout.Button("Remove Resource"))
		{
			if(a._resourcesTypes.Count == 0) { return; }
			a._resourcesTypes.RemoveAt(a._resourcesTypes.Count - 1);
			a._resourcesDistributions.RemoveAt(a._resourcesDistributions.Count - 1);
		}
		EditorGUILayout.BeginVertical();
		for(int i = 0; i < a._resourcesTypes.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			a._resourcesTypes[i] = (SO_ResourceType)EditorGUILayout.ObjectField(a._resourcesTypes[i],typeof(SO_ResourceType),true);
			EditorGUILayout.Space(5);
			a._resourcesDistributions[i] = EditorGUILayout.IntSlider(a._resourcesDistributions[i], 0, 10);
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
	}
}
