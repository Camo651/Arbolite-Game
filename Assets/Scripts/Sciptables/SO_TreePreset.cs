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
	public List<SO_Property> plantProperties;
}
