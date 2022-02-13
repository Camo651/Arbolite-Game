using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantFeature : MonoBehaviour
{
	public PlantFeatureType featureType;
	public Plant parentPlant;
	public PlantFeature parentFeature;
	public LineRenderer branchRenderer;
	public SpriteRenderer mainSpriteRenderer;
	public List<PlantFeature> childBranches, childNonBranchFeatures;
	public Vector3 branchEndPoint;
	public float endPointWidth;
	public Vector4 featureInterpolationSeed;
	public int branchlayer;
	public int localBranchIndex;


	//update the outside look of the plant based on its age, does not affect the overall structure
	public void UpdatePlant()
	{
		endPointWidth = parentPlant.trunkBaseWidth + ((branchlayer / (float)parentPlant.trueBranchLayerCount) * (parentPlant.branchWidthModifier * branchlayer));
		float inverter =  featureInterpolationSeed.w > .5f ? -1f : 1f;
		float trunkStretch = branchlayer == parentPlant.trueBranchLayerCount ? parentPlant.baseTrunkAmplifier : 1f;
		float verticalStretch = trunkStretch * Mathf.Lerp(parentPlant.branchVerticalStretchMin, parentPlant.branchVerticalStretchMax, featureInterpolationSeed.x) * parentPlant.plantAge;
		float horizontalStretch = inverter * Mathf.Lerp(parentPlant.branchHorizontalStretchMin, parentPlant.branchHorizontalStretchMax, featureInterpolationSeed.y) * parentPlant.plantAge;
		float seperationAngle = inverter * Mathf.Lerp(parentPlant.branchSeperationAngleMin, parentPlant.branchSeperationAngleMax, featureInterpolationSeed.z);

		Vector3 parentEndpoint = (parentFeature ? parentFeature.branchEndPoint : (Vector3.back));
		float parentEndpointWidth = (parentFeature ? parentFeature.endPointWidth : endPointWidth);
		branchEndPoint = parentEndpoint + new Vector3(horizontalStretch * Mathf.Cos((90 + (seperationAngle * localBranchIndex)) * Mathf.Deg2Rad),
									                    verticalStretch * Mathf.Sin((90 + (seperationAngle * localBranchIndex)) * Mathf.Deg2Rad), -1f);
		branchEndPoint.z = -1;
		branchRenderer.SetPosition(0, parentEndpoint + parentPlant.transform.position);
		branchRenderer.SetPosition(1, branchEndPoint + parentPlant.transform.position);
		branchRenderer.startWidth = parentEndpointWidth;
		branchRenderer.endWidth = endPointWidth;

		for (int i = 0; i < childNonBranchFeatures.Count; i++)
		{
			Vector3 pos = Vector3.Lerp(parentEndpoint + parentPlant.transform.position, branchEndPoint + parentPlant.transform.position, (i/(float)childNonBranchFeatures.Count));
			pos.z = -5;
			childNonBranchFeatures[i].transform.position = pos;
			childNonBranchFeatures[i].transform.localScale = new Vector3(parentPlant.plantAge * .1f * (branchlayer+1), parentPlant.plantAge * .1f * (branchlayer+1), 1f);
		}

		//flooding call
		foreach (PlantFeature branch in childBranches)
		{
			branch.UpdatePlant();
		}
	}

	//Generates the childed branches off this main one (assumes it alr genned)
	public void GenerateChildBranches(int layerIndex, Plant anchor)
	{
		parentPlant = anchor;
		branchlayer = layerIndex;
		int childBranchCount = (int)(Mathf.Lerp(parentPlant.branchNodeDensityMin, parentPlant.branchNodeDensityMax, featureInterpolationSeed.z));
		for (int i = 0; i < childBranchCount; i++)
		{
			PlantFeature newBranch = Instantiate(parentPlant.plantFeatureDefaultPrefab).GetComponent<PlantFeature>();
			newBranch.transform.SetParent(transform);
			childBranches.Add(newBranch);
			parentPlant.allChildedFeatures.Add(newBranch);
			newBranch.localBranchIndex = i+1;
			newBranch.GenerateThisBranch(this);
		}
	}

	//Builds the base structure of the current branch instance
	public void GenerateThisBranch(PlantFeature parent)
	{
		parentPlant = parent.parentPlant;
		parentFeature = parent;
		//do some presetup
		featureInterpolationSeed = new Vector4(Random.value, Random.value, Random.value, Random.value);
		branchlayer = parent.branchlayer - 1;
		branchRenderer = gameObject.AddComponent<LineRenderer>();
		branchRenderer.material = parentPlant.lineRendererMaterial;
		branchRenderer.startColor = parentPlant.branchColour;
		branchRenderer.endColor = parentPlant.branchColour;
		branchRenderer.numCapVertices = 3;

		//leaf generation
		if (branchlayer >= parentPlant.leafGenerationMinLayer && branchlayer <= parentPlant.leafGenerationMaxLayer)
		{
			float leafCount = (int)(Mathf.Lerp(parentPlant.leafDensityMin, parentPlant.leafDensityMax, featureInterpolationSeed.x));
			for (int i = 0; i < leafCount; i++)
			{
				PlantFeature newLeaf = Instantiate(parentPlant.plantFeatureDefaultPrefab).GetComponent<PlantFeature>();
				newLeaf.parentPlant = parentPlant;
				newLeaf.parentFeature = this;
				childNonBranchFeatures.Add(newLeaf);
				parentPlant.allChildedFeatures.Add(newLeaf);
				newLeaf.transform.SetParent(transform);
				SpriteRenderer sr = newLeaf.gameObject.AddComponent<SpriteRenderer>();
				sr.sprite = parentPlant.defaultLeaf;
				sr.color = parentPlant.leafColour;
				newLeaf.featureType = PlantFeatureType.Leaves;
			}
		}

		if (branchlayer > 1)
			GenerateChildBranches(branchlayer, parentPlant);
	}


	public enum PlantFeatureType
	{
		Branch,
		Leaves,
		Flower,
		Bushes,
		Vines,
		MushroomCap,
		Fruit
	}
}
