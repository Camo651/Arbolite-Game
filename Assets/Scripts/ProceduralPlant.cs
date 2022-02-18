using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class running in game that defines a whole plant
/// </summary>
public class ProceduralPlant : MonoBehaviour
{
	//generated data
	public string plantFullName;
	public List<PlantPart> plantParts;

	//Genetics
	public PlantType plantType;
	public PlantPart.BaseType basePartType;
	public SO_BiomeType plantBiome;
	public List<SO_ResourceType> plantResourceComposition;
	public List<int> resourceCompositionDistribution;
	public PlantPart.LeafType[] leafTypes;


	public enum PlantType
	{
		SmallTree, //like a lemon tree
		LargeTree, //like a big oak
		Shrubbery, //like a berry bush
		HaningVine, //like the things on my desk
		CrawlingVine, //like a tomato plant
		Mushroom, //like a toadstool
		SingleFlower, //like a dandelion
		Flowerbush, //like a rosebush
	}
}