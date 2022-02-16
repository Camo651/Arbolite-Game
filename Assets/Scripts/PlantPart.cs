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
	public List<Node> nodes;

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
		Gizmos.color = Color.red - new Color(0, 0, 0, .5f);
		Gizmos.DrawSphere(transform.position, .1f);
		Gizmos.DrawSphere(transform.position, .02f);
	}

}
