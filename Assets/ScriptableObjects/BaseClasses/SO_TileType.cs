using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_TileType : ScriptableObject
{
	public string tileTypeName;
	[TextArea()] public string tileInformation;
	[Space(10)] public Sprite backgroundSprite;
	[Tooltip("0=indifferent | 1=connector | 2=locked"), Range(0, 2)] public int topNodeState, rightNodeState, bottomNodeState, leftNodeState;
	//use this when NPCs are actually made //[Tooltip("Can an NPC pathfind in that direction")] public bool topPassthroughState, rightPassthroughState, bottomPassthroughState, leftPassthtoughState;
}
