using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Blueprint : MonoBehaviour
{
	public BlueprintManager blueprintManager;
	public bool isFilled;
	public List<SO_Property> properties;

	public TextMeshProUGUI titleBox;
	public Image icon;
	public Image background;
	public GameObject containedRoomPrefab;
	public int resourceCount;
	public bool isLocked;
	[HideInInspector] public bool hasBeenSeenUnlocked;

	/// <summary>
	/// Called from inspector when a button is clicked
	/// </summary>
	public void OnButtonClick()
	{
		blueprintManager.SelectBlueprint(this);
		hasBeenSeenUnlocked = true;
		background.color = blueprintManager.unlockedBluebrintColour;
	}

	/// <summary>
	/// Get the translated name of the blueprint
	/// </summary>
	/// <returns>The translated name of the blueprint</returns>
	public string GetBlueprintName()
	{
		return blueprintManager.globalRefManager.langManager.GetTranslation("name_" + GetContainedRoom().tileNameInfoID.ToLower());
	}

	/// <summary>
	/// Get the translated info of the blueprint
	/// </summary>
	/// <returns>The translated info of the blueprint</returns>
	public string GetBlueprintInfo()
	{
		return blueprintManager.globalRefManager.langManager.GetTranslation("info_" + GetContainedRoom().tileNameInfoID.ToLower());
	}

	/// <summary>
	/// Get the contained room linked to the blueprint's prefab
	/// </summary>
	/// <returns>A prefabbed contained room instance, given it exists</returns>
	public ContainedRoom GetContainedRoom()
	{
		return containedRoomPrefab.GetComponent<ContainedRoom>();
	}
}
