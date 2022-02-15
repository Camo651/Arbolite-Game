using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// a prefab container for holding individual parts of plants (braches, leaves, flowers etc)
/// </summary>
public class PlantPart : MonoBehaviour
{
	public string partName;
	public PartType partType;

	public enum PartType
	{
		Unused,
		Base,
		Branches,
		Leaves,
		MushroomCap,
		Fruit,
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, .05f);

		for(int i=0;i<transform.childCount;i++)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere(transform.GetChild(i).position, .05f);
		}
	}

}
