using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlantObject : MonoBehaviour
{
	public RoomTile roomTile;
	public List<SO_Property> plantProperties;
	public List<PlantPart> plantParts;

	SO_Property biomeType;
	SO_Property speciesType;
	SO_Property styleType;
	SO_Property rarityType;

	/// <summary>
	/// Generates the plants parts
	/// </summary>
	public void GeneratePlant()
	{
		biomeType = roomTile.roomContainer.globalRefManager.plantManager.GetPropertyFromType(plantProperties, PropertyManager.PropertyType.Biome);
		speciesType = roomTile.roomContainer.globalRefManager.plantManager.GetPropertyFromType(plantProperties, PropertyManager.PropertyType.Species);
		styleType = roomTile.roomContainer.globalRefManager.plantManager.GetPropertyFromType(plantProperties, PropertyManager.PropertyType.Style);
		rarityType = roomTile.roomContainer.globalRefManager.plantManager.GetPropertyFromType(plantProperties, PropertyManager.PropertyType.Rarity);

		PlantPart basePart = Instantiate(roomTile.roomContainer.globalRefManager.plantManager.GetRandomPlantPartPrefab(styleType), transform).GetComponent<PlantPart>();
		basePart.parentPlant = this;
		print(speciesType);
		basePart.SetPartValues(speciesType.SPECIES_BaseColour);

		List<Node> baseNodes = new List<Node>(basePart.transform.GetComponentsInChildren<Node>());
		List<PlantPart> leafParts = new List<PlantPart>();
		foreach (Node node in baseNodes)
		{
			if (node.needsToBeFulfilled)
			{
				PlantPart leaf = Instantiate(roomTile.roomContainer.globalRefManager.plantManager.GetRandomPlantPartPrefab(speciesType), node.transform).GetComponent<PlantPart>();
				leaf.parentPlant = this;
				leaf.SetPartValues(speciesType.SPECIES_LeafColour);
			}
		}


	}

	/// <summary>
	/// Get the full translated name of the plant object
	/// </summary>
	/// <returns>The full translated name of the plant object</returns>
	public string GetPlantFullName()
	{
		string a = "";
		a += roomTile.roomContainer.globalRefManager.langManager.GetTranslation("name_" + ("prop_" + rarityType.propertyType + "_" + rarityType.callbackID).ToLower())+" ";
		a += roomTile.roomContainer.globalRefManager.langManager.GetTranslation("name_" + ("prop_" + speciesType.propertyType + "_" + speciesType.callbackID).ToLower())+" ";
		a += roomTile.roomContainer.globalRefManager.langManager.GetTranslation("name_" + ("prop_" + styleType.propertyType + "_" + styleType.callbackID).ToLower());
		return a;
	}
}
