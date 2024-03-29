using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// a prefab container for holding individual parts of plants (braches, leaves, flowers etc)
/// </summary>
public class PlantPart : MonoBehaviour
{
	public string partName;
	public List<Node> nodes;
	public Vector2 rotatability;
	public bool flipable;
	public RelativePartDepth partDepth;
	public PlantObject parentPlant;
	public SpriteRenderer Srenderer;
	public enum RelativePartDepth : int
	{
		Back2 = -2,
		Back = -1,
		Middle = 0,
		Front = 1,
		Front2 = 2,
	}

	/// <summary>
	/// Sets the rotation and scale of the part
	/// </summary>
	public void SetPartValues(Color partColour)
	{
		if(Srenderer == null)
		{
			Srenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
		}
		transform.localEulerAngles = Vector3.forward * Random.Range(rotatability.x, rotatability.y);
		transform.GetChild(0).localScale = new Vector3(flipable ? Random.value > .5f ? -transform.GetChild(0).localScale.x : transform.GetChild(0).localScale.x : transform.GetChild(0).localScale.x, transform.GetChild(0).localScale.y, transform.GetChild(0).localScale.z);
		Srenderer.color = partColour;
		Srenderer.sortingOrder = (int)partDepth;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red - new Color(0, 0, 0, .5f);
		Gizmos.DrawSphere(transform.position, .1f);
		Gizmos.DrawSphere(transform.position, .02f);
	}
}
