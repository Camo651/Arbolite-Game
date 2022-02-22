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
		if (state)
		{
			transform.Translate(Vector2.up);
		}
		else
		{
			transform.Translate(Vector2.down);
		}

		gameObject.SetActive(state);
	}
}
