using UnityEngine;
using TMPro;

public class TranslationKey : MonoBehaviour
{
	public TextMeshProUGUI textBox;
	public string callBackID;

	public void Init()
	{
		if (textBox == null)
			textBox = GetComponent<TextMeshProUGUI>();
	}
}
