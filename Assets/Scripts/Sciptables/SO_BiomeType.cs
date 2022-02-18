using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]
public class SO_BiomeType : ScriptableObject
{
	public string biomeNameCallbackID;
	[Tooltip("A way to notate the resources specific to this biome")]public string biomePrefix;
	public Color[] biomeColourPalette;
	public SO_TreePreset[] treePresets; 

	public SO_TreePreset GetRandomPlantFromBiome()
	{
		List<SO_TreePreset> arr = new List<SO_TreePreset>();
		foreach(SO_TreePreset preset in treePresets)
		{
			for (int i = 0; i < preset.plantScarcity*20; i++)
			{
				arr.Add(preset);
			}
		}
		return arr[Random.Range(0, arr.Count)];
	}
}
