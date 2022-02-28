using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TranslationKey : MonoBehaviour
{
	public TextMeshProUGUI textBox;
	public string callBackID;

	/// <summary>
	/// Initializes the UGUI on the translation key object
	/// </summary>
	public void Init()
	{
		if (textBox == null)
			textBox = GetComponent<TextMeshProUGUI>();
	}
}
