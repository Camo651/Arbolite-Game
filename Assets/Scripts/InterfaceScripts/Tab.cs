using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tab : MonoBehaviour
{
	public Image tabMenuBkg;
	public TextMeshProUGUI tabMenuText;
	public int tabIndex;
	[Space(10)]
	public TextMeshProUGUI tabTitle, tabSubtitle, tabDescrition;
	public Toggle tabToggle;

	/// <summary>
	/// Enables and disabled the current tab, handles tab menu button pressing as well
	/// </summary>
	/// <param name="state">The state to be set to</param>
	public void SetTabState(bool state)
	{
		gameObject.SetActive(state);
		tabMenuBkg.color = state ? Color.white : new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f);
		tabMenuText.color = !state ? Color.white : new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f);

		if (GetComponent<ItemDisplayer>())
			GetComponent<ItemDisplayer>().hoverInfoHighlight.LockToCursor(false);
	}
}
