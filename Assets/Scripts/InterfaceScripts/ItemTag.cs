using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTag : MonoBehaviour
{
	public Item itemInfo;
	public ItemDisplayer displayer;
	public TMPro.TextMeshProUGUI nameBox;
	public UnityEngine.UI.Image icon;

	public int index;

	public void OnButtonPress()
	{
		if(itemInfo)
			displayer.HighlightItem(itemInfo);
	}
}
