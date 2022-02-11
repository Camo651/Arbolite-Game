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
	public List<SpriteRenderer> childSpriteRenderers;
	public List<PlantFeature> childPlantFeatures;
	public Vector3 branchEndPoint;

	public void GenerateBranches(Plant anchor, int branchLayerCount)
	{
		parentPlant = anchor;
		if(branchLayerCount > 0)
		{
			for(float i = parentPlant.branchNodeDensity / -2; i < parentPlant.branchNodeDensity/2; i++)
			{
				PlantFeature child = Instantiate(parentPlant.plantFeatureDefaultPrefab).GetComponent<PlantFeature>();
				child.SetFeatureType(PlantFeatureType.Branch);
				child.transform.SetParent(transform);
				anchor.allChildedFeatures.Add(child);
				child.branchEndPoint = new Vector3(branchEndPoint.x + anchor.branchVerticalStretch * Mathf.Cos(( 90 + anchor.branchSeperationAngle * i) * Mathf.Deg2Rad), branchEndPoint.y + anchor.branchVerticalStretch * Mathf.Sin((90 + anchor.branchSeperationAngle * i) * Mathf.Deg2Rad), -1f);
				child.branchRenderer.SetPosition(0, branchEndPoint);
				child.branchRenderer.SetPosition(1, child.branchEndPoint);
				child.branchRenderer.SetWidth(.1f, .1f);
				child.GenerateBranches(anchor, branchLayerCount - 1);
			}
		}
	}
	public void SetFeatureType(PlantFeatureType type)
	{
		switch (type)
		{
			case PlantFeatureType.Branch: branchRenderer = gameObject.AddComponent<LineRenderer>(); break;
		}
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
