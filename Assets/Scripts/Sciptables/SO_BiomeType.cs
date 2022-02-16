using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]
public class SO_BiomeType : ScriptableObject
{
	public string biomeNameCallbackID;
	[Tooltip("A way to notate the resources specific to this biome")]public string biomePrefix;
	public Color[] biomeColourPalette;
}
