using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationHighlighter : MonoBehaviour
{
	public TextMeshProUGUI highlightTitle, highlightSubtitle, highlightTextarea;
	public GameObject highlightBox;
	public Image highlightIcon;
	public int? currentDispHash;

	public void CloseHighlight()
	{
		currentDispHash = null;
		if (highlightBox)
			highlightBox.SetActive(false);
	}
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
