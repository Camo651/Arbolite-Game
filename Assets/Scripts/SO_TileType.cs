using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SO_TileType : ScriptableObject
{
	public string tileTypeName;
	public bool topNode, rightNode, bottomNode, leftNode;
	public Sprite backgroundSprite;
}
