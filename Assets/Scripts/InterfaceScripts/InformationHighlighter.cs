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
	public bool lockToCursor;
	public PropertyDisplayer propDisp;

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
	#pragma warning disable
	public void OpenHighlight(int? _hash, string? _title, string? _subtitle, string? _description, Sprite? _icon, Color? _col)
	{
		if (highlightBox)
			highlightBox.SetActive(true);
		currentDispHash = _hash;
		if(highlightTitle && _title!=null)
			highlightTitle.text = _title;
		if(highlightSubtitle && _subtitle!=null)
			highlightSubtitle.text = _subtitle;
		if(highlightTextarea && _description!=null)
			highlightTextarea.text = _description;
		if(highlightIcon)
			highlightIcon.sprite = _icon;
		if (highlightIcon && _col!=null)
			highlightIcon.color = _col ?? Color.white;
	}
#pragma warning enable

	public void LockToCursor(bool lockState)
	{
		lockToCursor = lockState;
		gameObject.SetActive(lockState);
	}

	private void LateUpdate()
	{
		if (lockToCursor)
		{
			transform.position = Input.mousePosition;
		}
	}
}
