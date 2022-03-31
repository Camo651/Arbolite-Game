using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlantObject : MonoBehaviour
{
	public RoomTile roomTile;

	public List<PlantPart> plantParts;
	public PlantPart basePart;
	public List<PlantPart> leafParts;

	public List<SO_Property> plantProperties;
	SO_Property biomeType;
	SO_Property speciesType;
	SO_Property styleType;
	SO_Property rarityType;
	SO_Property ageType;
	public bool canBeHarvestedWithoutDestroy;

	/// <summary>
	/// Generates the plants parts
	/// </summary>
	public void GeneratePlant()
	{
		biomeType = roomTile.roomContainer.globalRefManager.propertyManager.GetPropertyFromType(plantProperties, PropertyManager.PropertyType.Biome);
		speciesType = roomTile.roomContainer.globalRefManager.propertyManager.GetPropertyFromType(plantProperties, PropertyManager.PropertyType.Species);
		styleType = roomTile.roomContainer.globalRefManager.propertyManager.GetPropertyFromType(plantProperties, PropertyManager.PropertyType.Style);
		rarityType = roomTile.roomContainer.globalRefManager.propertyManager.GetPropertyFromType(plantProperties, PropertyManager.PropertyType.Rarity);
		ageType = roomTile.roomContainer.globalRefManager.propertyManager.GetPropertyFromType(plantProperties, PropertyManager.PropertyType.Age);

		basePart = Instantiate(styleType.GENERAL_PlantParts[Random.Range(0,styleType.GENERAL_PlantParts.Count)], transform).GetComponent<PlantPart>();
		basePart.parentPlant = this;
		basePart.SetPartValues(speciesType.SPECIES_BaseColour);
		plantParts.Add(basePart);

		List<Node> baseNodes = new List<Node>(basePart.transform.GetComponentsInChildren<Node>());
		leafParts = new List<PlantPart>();
		foreach (Node node in baseNodes)
		{
			if (node.needsToBeFulfilled)
			{
				PlantPart leaf = Instantiate(speciesType.GENERAL_PlantParts[Random.Range(0, speciesType.GENERAL_PlantParts.Count)], node.transform).GetComponent<PlantPart>();
				leaf.parentPlant = this;
				leaf.SetPartValues(speciesType.SPECIES_LeafColour);
				leafParts.Add(leaf);
			}
		}
		plantParts.AddRange(leafParts);

		//Make sure all the values have been set before aging the plant
		SetGrowthStage(ageType);
	}

	/// <summary>
	/// Get the full translated name of the plant object
	/// </summary>
	/// <returns>The full translated name of the plant object</returns>
	public string GetPlantFullName()
	{
		string a = "";
		a += roomTile.roomContainer.globalRefManager.langManager.GetTranslation("name_" + ("prop_" + ageType.propertyType + "_" + ageType.callbackID).ToLower()) + " ";
		a += roomTile.roomContainer.globalRefManager.langManager.GetTranslation("name_" + ("prop_" + rarityType.propertyType + "_" + rarityType.callbackID).ToLower())+" ";
		a += roomTile.roomContainer.globalRefManager.langManager.GetTranslation("name_" + ("prop_" + speciesType.propertyType + "_" + speciesType.callbackID).ToLower())+" ";
		a += roomTile.roomContainer.globalRefManager.langManager.GetTranslation("name_" + ("prop_" + styleType.propertyType + "_" + styleType.callbackID).ToLower());
		return a;
	}

	/// <summary>
	/// Attempts to increment the plant objects growth stage
	/// </summary>
	public void TryGrowPlant()
	{
		if((ageType.AGE_GrowthStageModifier*speciesType.SPECIES_GrowthStageChance*styleType.STYLE_GrowthStageModifier) > Random.value)
		{
			if(ageType.AGE_Value < 8)
			{
				SetGrowthStage(roomTile.roomContainer.globalRefManager.propertyManager.GetAge(ageType.AGE_Value+1));
			}
		}
	}

	public void HarvestWithoutDestroy()
	{
		SetGrowthStage(roomTile.roomContainer.globalRefManager.propertyManager.GetAge(5));
	}

	/// <summary>
	/// Sets the growth stage to a certain stage
	/// </summary>
	/// <param name="stage"></param>
	public void SetGrowthStage(SO_Property stage)
	{
		plantProperties.Remove(ageType);
		ageType = stage;
		plantProperties.Add(stage);
		transform.localScale = Vector3.one * stage.AGE_GrowthScale;
		basePart.SetPartValues(speciesType.SPECIES_BaseColour + new Color(stage.AGE_ColorTint, stage.AGE_ColorTint, stage.AGE_ColorTint,1f));
		foreach (PlantPart leaf in leafParts)
		{
			leaf.gameObject.SetActive(stage.AGE_HasLeaves);
			leaf.SetPartValues(speciesType.SPECIES_LeafColour + new Color(stage.AGE_ColorTint, stage.AGE_ColorTint, stage.AGE_ColorTint, 1f));

		}
	}

	/// <summary>
	/// Get the resources from the plant object
	/// </summary>
	/// <param name="onlyRenewable">Should it return all the resources or just renewable ones?</param>
	/// <returns>A list of items</returns>
	public List<Item> GetPlantResources(bool onlyRenewable)
	{
		PropertyManager pman = roomTile.roomContainer.globalRefManager.propertyManager;
		List<Item> items = new List<Item>();
		float multiplier = ageType.GENERAL_ItemDropMultiplier * speciesType.GENERAL_ItemDropMultiplier * styleType.GENERAL_ItemDropMultiplier;
		float baseCount = multiplier;
		float leafCount = leafParts.Count * multiplier;
		float fruitCount = 0;

		if (onlyRenewable)
		{
			for (int i = 0; i < leafCount / 2f; i++)
			{
				Item item = new Item(null);
				List<SO_Property> itemProps = new List<SO_Property>() { pman.GetProperty(PropertyManager.PropertyType.Resource, "leaves"), speciesType, ageType.GENERAL_Properties[0] };
				item.SetItemProperties(itemProps, roomTile.roomContainer.globalRefManager);
				items.Add(item);
			}
			for (int i = 0; i < fruitCount; i++)
			{
				Item item = new Item(null);
				item.SetItemProperties(null, roomTile.roomContainer.globalRefManager);
				items.Add(item);
			}
		}
		else
		{
			for (int i = 0; i < baseCount; i++)
			{
				Item item = new Item(null);
				List<SO_Property> itemProps = new List<SO_Property>() { pman.GetProperty(PropertyManager.PropertyType.Resource, "log"),speciesType,ageType.GENERAL_Properties[0] };
				item.SetItemProperties(itemProps, roomTile.roomContainer.globalRefManager);
				item.isValidBuildingMaterial = true;
				items.Add(item);
			}
			for (int i = 0; i < leafCount; i++)
			{
				Item item = new Item(null);
				List<SO_Property> itemProps = new List<SO_Property>() { pman.GetProperty(PropertyManager.PropertyType.Resource, "leaves"), speciesType, ageType.GENERAL_Properties[0] };
				item.SetItemProperties(itemProps, roomTile.roomContainer.globalRefManager);
				items.Add(item);
			}
			for (int i = 0; i < fruitCount; i++)
			{
				Item item = new Item(null);
				item.SetItemProperties(null, roomTile.roomContainer.globalRefManager);
				items.Add(item);
			}
		}
		return items;
	}
}
