using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
	[Header("Refs")]
	public bool regenPlant;
	public PlantType plantType;
	public GameObject plantFeatureDefaultPrefab;
	public Sprite defaultLeaf;
	public Material lineRendererMaterial;

	[Space(10), Header("Params")]
	public float branchVerticalStretchMin;
	public float branchVerticalStretchMax;
	public float branchHorizontalStretchMin, branchHorizontalStretchMax;
	public float branchSeperationAngleMin, branchSeperationAngleMax;
	public float branchNodeDensityMin, branchNodeDensityMax;
	public int minBranchLayerCount, maxBranchLayerCount;
	public float trunkBaseWidth, branchWidthModifier;
	public float baseTrunkAmplifier;
	public float leafGenerationMinLayer, leafGenerationMaxLayer;
	public float leafScaleMin, leafScaleMax;
	public float leafDensityMin, leafDensityMax;


	[Space(10), Header("Dynamics")]
	public int trueBranchLayerCount;
	public float plantAge;
	public List<PlantFeature> allChildedFeatures;
	public PlantFeature baseAnchorFeature;

	[Space(10), Header("Colours")]
	public Color branchColour;
	public Color leafColour;

	private void Start()
	{
		GeneratePlant();
	}
	public void Update()
	{
		if (regenPlant)
		{
			regenPlant = false;
			ResetPlant();
			GeneratePlant();
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			plantAge += .1f;
			baseAnchorFeature.UpdatePlant();
		}
	}
	public void GeneratePlant()
	{
		allChildedFeatures = new List<PlantFeature>();
		baseAnchorFeature = Instantiate(plantFeatureDefaultPrefab).GetComponent<PlantFeature>();
		baseAnchorFeature.transform.SetParent(transform);
		allChildedFeatures.Add(baseAnchorFeature);
		trueBranchLayerCount = Random.Range(minBranchLayerCount, maxBranchLayerCount);

		//presetup for base branch
		baseAnchorFeature.featureInterpolationSeed = new Vector4(Random.value, Random.value, Random.value, Random.value);
		baseAnchorFeature.branchRenderer = baseAnchorFeature.gameObject.AddComponent<LineRenderer>();
		baseAnchorFeature.branchRenderer.material = lineRendererMaterial;
		baseAnchorFeature.branchRenderer.SetColors(branchColour, branchColour);
		baseAnchorFeature.branchRenderer.numCapVertices = 3;
		baseAnchorFeature.branchRenderer.SetWidth(.1f, .1f);
		baseAnchorFeature.GenerateChildBranches(trueBranchLayerCount, this);
		baseAnchorFeature.UpdatePlant();
	}

	public void ResetPlant()
	{
		plantAge = 1f;
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
