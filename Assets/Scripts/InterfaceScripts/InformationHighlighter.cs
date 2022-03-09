using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A data holder to make using custom info highlighters a bit easier
/// </summary>
public class InformationHighlighter : MonoBehaviour
{
	public TextMeshProUGUI highlightTitle, highlightSubtitle, highlightTextarea;
	public GameObject highlightBox;
	public Image highlightIcon;
	public int? currentDispHash;

	/// <summary>
	/// Closes the information highlight box
	/// </summary>
	public void CloseHighlight()
	{
		currentDispHash = null;
		if (highlightBox)
			highlightBox.SetActive(false);
	}

	/// <summary>
	/// Set the highlight box's state and values
	/// </summary>
	/// <param name="_hash">The hash code (or uuid) of the current object</param>
	/// <param name="_title">The text to be displayed in the title section</param>
	/// <param name="_subtitle">The text to be displayed in the subtitle section</param>
	/// <param name="_description">The text to be displayed in the description section</param>
	/// <param name="_icon">The sprite to be displayed in the icon section</param>
	/// <param name="_col">The colour of the icon to be displayed in the icon section</param>
	public void OpenHighlight(int? _hash, string _title, string _subtitle, string _description, Sprite _icon, Color _col)
	{
		if (highlightBox)
			highlightBox.SetActive(true);
		currentDispHash = _hash;
		if(highlightTitle)
			highlightTitle.text = _title;
		if(highlightSubtitle)
			highlightSubtitle.text = _subtitle;
		if(highlightTextarea)
			highlightTextarea.text = _description;
		if(highlightIcon)
			highlightIcon.sprite = _icon;
		if(highlightIcon)
			highlightIcon.color = _col;
	}
}
