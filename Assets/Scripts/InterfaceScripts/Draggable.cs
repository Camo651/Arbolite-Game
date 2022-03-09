using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public Vector3 dragTransformOriginOffset;
	public Vector3 draggableOrigin;
	public float zoomSpeed, zoomMin, zoomMax, zoomReal;
	public bool isDragging;
	public UnityEngine.UI.Image recentreButton;
	public float recentreHighlightDistance;

	/// <summary>
	/// Start dragging the transform
	/// </summary>
	public void BeginDrag()
	{
		dragTransformOriginOffset = transform.position - Input.mousePosition;
		isDragging = true;
	}

	/// <summary>
	/// Finish dragging the transform
	/// </summary>
	public void EndDrag()
	{
		dragTransformOriginOffset = Vector3.zero;
		isDragging = false;
	}

	/// <summary>
	/// Standard unity update cycle. Handles the actual dragging of the transform
	/// </summary>
	private void Update()
	{
		if (isDragging)
			transform.position = Input.mousePosition + dragTransformOriginOffset;

		zoomReal = Mathf.Clamp(zoomReal + (Input.mouseScrollDelta.y * zoomSpeed * (globalRefManager.settingsManager.invertScrollDirection ? -1 : 1)), zoomMin, zoomMax);
		if (Input.mouseScrollDelta.y != 0)
		{
			transform.localScale = new Vector3(zoomReal, zoomReal, 1f);
		}

		if(Vector3.Distance(draggableOrigin,transform.position) > recentreHighlightDistance)
		{
			recentreButton.color = Color.Lerp(Color.white, Color.gray, Mathf.Abs(Mathf.Sin(Time.time)));
		}
		else
		{
			recentreButton.color = Color.white;
		}
	}

	/// <summary>
	/// Recentres the transform to the centre point
	/// </summary>
	public void RecentreDraggable()
	{
		transform.position = draggableOrigin;
		zoomReal = Mathf.Lerp(zoomMax, zoomMin, .5f);
		transform.localScale = new Vector3(zoomReal, zoomReal, 1f);

	}
}
