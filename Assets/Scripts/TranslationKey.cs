using UnityEngine;
using TMPro;

public class TranslationKey : MonoBehaviour
{
	public TextMeshProUGUI textBox;
	public string callBackID;

	private void Awake()
	{
		if (textBox == null)
			textBox = GetComponent<TextMeshProUGUI>();
	}
}
