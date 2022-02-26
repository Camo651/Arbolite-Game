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

	public void SetTabState(bool state)
	{
		gameObject.SetActive(state);
		tabMenuBkg.color = state ? new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f) : Color.white;
		tabMenuText.color = !state ? new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f) : Color.white;
	}
}
