using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;


/// <summary>
/// a prefab container for holding individual parts of plants (braches, leaves, flowers etc)
/// </summary>
public class PlantPart : MonoBehaviour
{
	public string partName;
	public PartType partType;
	public List<Node> nodes;

	public enum PartType
	{
		//Base Types
		___BASE_TYPES___ = 0,
		Base_None,
		Base_TinyStem,
		Base_Bush,
		Base_SmallTree,
		Base_ComplexTree,

		//Leaf Types
		___LEAF_TYPES___ = 1000,
		Leaves_None,
		Leaves_MushroomCap,
		Leaves_Triangle,
	}
	public enum PartTypeRangeIndexer
	{
		Bases = 0,
		Leaves = 1000,
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red - new Color(0, 0, 0, .5f);
		Gizmos.DrawSphere(transform.position, .1f);
		Gizmos.DrawSphere(transform.position, .02f);
	}

}
