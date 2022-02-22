using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tab : MonoBehaviour
{
	public Image tabMenuBkg;
	public TextMeshProUGUI tabMenuText;

	public void SetTabState(bool state)
	{
		gameObject.SetActive(state);
	}
}
