using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PropertyDisplayer : MonoBehaviour
{
	public PropertyManager propertyManager;
	public string propertyDisplayCallbackID;
	public GameObject gridLayoutTransform;
	public Tag highlightedPropertyTag;
	public TextMeshProUGUI highlightTitle, highlightSubtitle, highlightTextarea;
	public GameObject highlightBox;
	public Image highlightIcon;
	public GameObject tabSelectorButton;

	/// <summary>
	/// Displays the properties of rt
	/// </summary>
	/// <param name="rt">The RoomTile to display</param>
	public void DisplayProperties(RoomTile rt)
	{
		DisplayProperties(rt.properties);
	}

	/// <summary>
	/// Displays a list of properties
	/// </summary>
	/// <param name="properties">The list of properties to display</param>
	public void DisplayProperties(List<SO_Property> properties)
	{
		ClearProperties();
		highlightBox.SetActive(false);
		if (properties != null)
		{
			foreach (SO_Property property in properties)
			{
				Tag t = Instantiate(propertyManager.propertyTagPrefab, gridLayoutTransform.transform).GetComponent<Tag>();
				t.SetValues(this, property);
			}
		}
	}

	/// <summary>
	/// Clear all the currently displayed properties from this displayer
	/// </summary>
	public void ClearProperties()
	{
		for (int i = 0; i < gridLayoutTransform.transform.childCount; i++)
		{
			Destroy(gridLayoutTransform.transform.GetChild(i).gameObject);
		}
	}

	/// <summary>
	/// Set the highlight viewer GUI to show info
	/// </summary>
	/// <param name="a">The tag to be highlighted</param>
	public void SetHighlightedPropertyTag(Tag a)
	{
		if(highlightedPropertyTag == a)
		{
			highlightBox.SetActive(false);
			highlightedPropertyTag = null;
			propertyManager.globalRefManager.audioManager.Play(AudioManager.AudioClipType.Interface, "toggle_ui");

		}
		else
		{
			highlightedPropertyTag = a;
			highlightBox.SetActive(true);
			highlightTitle.text = a.GetTagName();
			highlightSubtitle.text = a.GetPropertyType();
			highlightTextarea.text = a.GetPropertyDesciption();
			highlightIcon.sprite = propertyManager.GetIcon(a.property.propertyType);
			highlightIcon.color = propertyManager.GetColour(a.property.propertyType);
			propertyManager.globalRefManager.audioManager.Play(AudioManager.AudioClipType.Interface, "property_hover");
		}
	}

	/// <summary>
	/// Closes the highlight viewer
	/// </summary>
	public void CloseHighlightPropertyDisplay()
	{
		highlightBox.SetActive(false);
		highlightedPropertyTag = null;
		propertyManager.globalRefManager.audioManager.Play(AudioManager.AudioClipType.Interface, "toggle_ui");

	}
}
