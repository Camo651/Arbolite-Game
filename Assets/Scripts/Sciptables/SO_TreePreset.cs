using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_TreePreset: ScriptableObject
{
	//This class is for making preset plant types
	public ProceduralPlant.PlantType plantType;
	[Range(0, 1), Tooltip("Higher = more common")] public float plantScarcity;
	public PlantPart leafType;

}
