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

	public void GeneratePlant()
	{
		//figure out whats what
		//make the base
		//add leaves
		//fukin idk

		biomeType = GetPropertyFromType(PropertyManager.PropertyType.Biome);
		speciesType = GetPropertyFromType(PropertyManager.PropertyType.Species);
		styleType = GetPropertyFromType(PropertyManager.PropertyType.Style);
		rarityType = GetPropertyFromType(PropertyManager.PropertyType.Rarity);

		PlantPart basePart = Instantiate(roomTile.roomContainer.globalRefManager.plantManager.GetRandomPlantPartPrefab(styleType), transform).GetComponent<PlantPart>();
		basePart.parentPlant = this;
		basePart.SetPartValues();

		List<Node> baseNodes = new List<Node>(basePart.transform.GetComponentsInChildren<Node>());
		List<PlantPart> leafParts = new List<PlantPart>();
		foreach (Node node in baseNodes)
		{
			if (node.needsToBeFulfilled)
			{
				PlantPart leaf = Instantiate(roomTile.roomContainer.globalRefManager.plantManager.GetRandomPlantPartPrefab(speciesType), node.transform).GetComponent<PlantPart>();
				leaf.parentPlant = this;
				leaf.SetPartValues();
			}
		}


	}

	public string GetPlantFullName()
	{
		string a = "";
		a += roomTile.roomContainer.globalRefManager.langManager.GetTranslation("name_" + ("prop_" + rarityType.propertyType + "_" + rarityType.callbackID).ToLower())+" ";
		a += roomTile.roomContainer.globalRefManager.langManager.GetTranslation("name_" + ("prop_" + speciesType.propertyType + "_" + speciesType.callbackID).ToLower())+" ";
		a += roomTile.roomContainer.globalRefManager.langManager.GetTranslation("name_" + ("prop_" + styleType.propertyType + "_" + styleType.callbackID).ToLower());
		return a;
	}

	public SO_Property GetPropertyFromType(PropertyManager.PropertyType type)
	{
		foreach(SO_Property p in plantProperties)
		{
			if(p.propertyType == type)
			{
				return p;
			}
		}
		return null;
	}
}
