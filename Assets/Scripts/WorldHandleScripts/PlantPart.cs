using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a prefab container for holding individual parts of plants (braches, leaves, flowers etc)
/// </summary>
public class PlantPart : MonoBehaviour
{
	public string partName;
	public SO_Property propertyDependancy;
	public List<Node> nodes;
	public Vector2 rotatability;
	public bool flipable;
	public RelativePartDepth partDepth;
	public enum RelativePartDepth : int
	{
		Back = -1,
		Middle = 0,
		Front = 1,
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red - new Color(0, 0, 0, .5f);
		Gizmos.DrawSphere(transform.position, .1f);
		Gizmos.DrawSphere(transform.position, .02f);
	}
}
