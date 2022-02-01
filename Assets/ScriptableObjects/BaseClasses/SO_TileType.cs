using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_TileType : ScriptableObject
{
	public string tileTypeName;
	[Tooltip("0=indifferent | 1=connector | 2=locked"),Range(0,2)]public int topNodeState, rightNodeState, bottomNodeState, leftNodeState;
	public Sprite backgroundSprite;
}
