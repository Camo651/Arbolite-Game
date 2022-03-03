using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Handles the animating of components in the scene that just always play and never need to be changed.
/// These animations do not affect any aspect of the game play
///
/// </summary>
public class AestheticAnimator : MonoBehaviour
{
	public enum AestAnimType
	{
		None,
		Spinning,
		Sliding,
	}

	[HideInInspector] public GlobalRefManager globalRefManager;
	public AestAnimType animType;
	public GameObject animatedComponent;
	[Space(10)]
	public float spinningSpeed;
	public float slidingSpeed, slidingDistance;
	public bool slideXAxis, spinAffectedByWind;
	private float slidingDirection = 1;

	private void Awake()
	{
		globalRefManager = GameObject.FindGameObjectWithTag("Base").GetComponent<GlobalRefManager>();
	}

	private void FixedUpdate()
	{
		if (!animatedComponent || globalRefManager.baseManager.gameIsActivelyFrozen)
			return;

		switch (animType)
		{
			case AestAnimType.None:
				break;
			case AestAnimType.Spinning:
				animatedComponent.transform.Rotate(Vector3.forward * spinningSpeed * (spinAffectedByWind?globalRefManager.terrainManager.windspeed:1f));
				break;
			case AestAnimType.Sliding:
				if (Mathf.Abs(animatedComponent.transform.localPosition.x) > slidingDistance || Mathf.Abs(animatedComponent.transform.localPosition.y) > slidingDistance)
					slidingDirection *= -1;
				animatedComponent.transform.Translate((slideXAxis?Vector2.right:Vector2.up) * slidingSpeed * slidingDirection, Space.Self);
				break;
		}
	}
}

//[CustomEditor(typeof(AestheticAnimator))]
//public class AestAnimEditor : Editor
//{
//	public override void OnInspectorGUI()
//	{
//		base.OnInspectorGUI();
//		AestheticAnimator a = (AestheticAnimator)target;
//		switch (a.animType)
//		{
//			case AestheticAnimator.AestAnimType.None:
//				break;
//			case AestheticAnimator.AestAnimType.Spinning:
//				a.spinningSpeed = EditorGUILayout.FloatField("Spinning Speed", a.spinningSpeed);
//				a.spinAffectedByWind = EditorGUILayout.Toggle("Spin by wind", a.spinAffectedByWind);
//				break;
//			case AestheticAnimator.AestAnimType.Sliding:
//				a.slidingSpeed = EditorGUILayout.FloatField("Sliding Speed", a.slidingSpeed);
//				a.slidingDistance = EditorGUILayout.FloatField("Sliding Distance", a.slidingDistance);
//				a.slideXAxis = EditorGUILayout.Toggle("Sliding on the " + (a.slideXAxis ? "X" : "Y") + " axis", a.slideXAxis);
//				break;
//		}
//	}
//}
