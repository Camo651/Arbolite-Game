using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_TileType : ScriptableObject
{
	public string tileTypeName;
	[TextArea()] public string tileInformation;
	[Space(10)] public Sprite backgroundSprite;
	[Tooltip("0=indifferent | 1=Has to connect | 2=cant connect | 3=Sprite connector"), Range(0, 3)] public int topNodeState, rightNodeState, bottomNodeState, leftNodeState;
}
