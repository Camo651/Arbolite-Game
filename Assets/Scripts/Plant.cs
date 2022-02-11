using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
	public PlantType plantType;
	public List<PlantFeature> allChildedFeatures;
	public GameObject plantFeatureDefaultPrefab;
	public PlantFeature baseAnchorFeature;
	public float branchVerticalStretch;
	public float branchHorizontalStretch;
	public float branchSeperationAngle;
	public float branchNodeDensity;
	public int branchLayerCount;
	public float leafDensity;
	public float leafDistributionMin;
	public float leafDistributionMax;

	public Color branchColour;
	public Color leafColour;

	private void Start()
	{
		GeneratePlant();
	}
	public void Update()
	{
			ResetPlant();
			GeneratePlant();
	}
	public void GeneratePlant()
	{
		allChildedFeatures = new List<PlantFeature>();
		baseAnchorFeature = Instantiate(plantFeatureDefaultPrefab).GetComponent<PlantFeature>();
		baseAnchorFeature.transform.SetParent(transform);
		allChildedFeatures.Add(baseAnchorFeature);
		baseAnchorFeature.SetFeatureType(PlantFeature.PlantFeatureType.Branch);
		baseAnchorFeature.GenerateBranches(this, branchLayerCount-1);
	}

	public void ResetPlant()
	{
		Destroy(baseAnchorFeature.gameObject);
		allChildedFeatures.Clear();
	}

	public enum PlantType
	{
		Grassy,
		Foliage,
		SmallTree,
		LargeTree,
		Climbing
	}
}
