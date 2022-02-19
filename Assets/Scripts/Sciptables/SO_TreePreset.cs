using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
///<summary>
/// Defines the preset trees that will generate when a new game is started of a new biome is generated
/// </summary>
public class SO_TreePreset: ScriptableObject
{
	public string callbackID;
	[Range(0, 1), Tooltip("Higher = more common")] public float plantScarcity;
	public ProceduralPlant.PlantType _plantType;
	public SO_BiomeType _biome;
	public List<PlantPart.PartType> _partTypes;
	[HideInInspector] public List<SO_ResourceType> _resourcesTypes = new List<SO_ResourceType>();
	[HideInInspector] public List<int> _resourcesDistributions = new List<int>();

	public ProceduralPlant.ResourceDistr GetResourceDistr()
	{
		ProceduralPlant.ResourceDistr a = new ProceduralPlant.ResourceDistr();
		a.plantResourceComposition = _resourcesTypes;
		a.resourceCompositionDistribution = _resourcesDistributions;
		return a;
	}
}
