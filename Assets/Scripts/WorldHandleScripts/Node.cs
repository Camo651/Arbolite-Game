using UnityEngine;

public class Node : MonoBehaviour
{
	public enum NodeType
	{
		None,
		BranchNode,
		FeatureNode,
		PlantSpot,
	}
	public NodeType nodeType;
	public bool needsToBeFulfilled;

	private void OnDrawGizmos()
	{
		Gizmos.color = nodeToColour[(int)nodeType] - new Color(0, 0, 0, .5f);
		Gizmos.DrawSphere(transform.position, .05f);
	}
	private Color[] nodeToColour = { Color.black, Color.cyan, Color.yellow, Color.green, Color.red};
}
