using UnityEngine;

public class Node : MonoBehaviour
{
	public enum NodeType
	{
		None,
		BranchNode,
		FeatureNode,
	}
	public NodeType nodeType;
	public bool needsToBeFulfilled;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(transform.position, .05f);
	}
}
