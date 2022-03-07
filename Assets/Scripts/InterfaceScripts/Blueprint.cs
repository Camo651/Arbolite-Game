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
	public void OnButtonClick()
	{
		blueprintManager.SelectBlueprint(this);
		hasBeenSeenUnlocked = true;
		background.color = blueprintManager.unlockedBluebrintColour;
	}

	public string GetBlueprintName()
	{
		return blueprintManager.globalRefManager.langManager.GetTranslation("name_" + GetContainedRoom().tileNameInfoID.ToLower());
	}

	public string GetBlueprintInfo()
	{
		return blueprintManager.globalRefManager.langManager.GetTranslation("info_" + GetContainedRoom().tileNameInfoID.ToLower());
	}

	public ContainedRoom GetContainedRoom()
	{
		return containedRoomPrefab.GetComponent<ContainedRoom>();
	}
}
