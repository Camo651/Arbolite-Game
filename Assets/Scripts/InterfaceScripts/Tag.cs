using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tag : MonoBehaviour
{
	public PropertyManager propertyManager;
	public PropertyDisplayer propertyDisplayer;
	public SO_Property property;
	public Image bkgImage;
	public TextMeshProUGUI textBox;
	public string fullTranslationID;

	/// <summary>
	/// Set the values of the tag
	/// </summary>
	/// <param name="disp">The property displayer to be shown in</param>
	/// <param name="prop">The tag data to be displayed</param>
	public void SetValues(PropertyDisplayer disp, SO_Property prop)
	{
		propertyManager = disp.propertyManager;
		propertyDisplayer = disp;
		property = prop;
		bkgImage.color = propertyManager.GetColour(prop.propertyType);
		fullTranslationID = ("prop_" + prop.propertyType + "_" + prop.callbackID).ToLower();
		textBox.text = propertyManager.globalRefManager.langManager.GetTranslation("name_"+fullTranslationID);
		transform.name = fullTranslationID;
	}

	/// <summary>
	/// Intakes a button press on the tag object
	/// </summary>
	public void OnButtonClick()
	{
		propertyDisplayer.SetHighlightedPropertyTag(this);
	}

	/// <summary>
	/// Get the translated tag name
	/// </summary>
	/// <returns>The translated tag name</returns>
	public string GetTagName()
	{
		return propertyManager.globalRefManager.langManager.GetTranslation("name_"+fullTranslationID);
	}

	/// <summary>
	/// Get the translated property type
	/// </summary>
	/// <returns>The translated property type</returns>
	public string GetPropertyType()
	{
		return propertyManager.globalRefManager.langManager.GetTranslation("proptype_"+(""+property.propertyType).ToLower());
	}

	/// <summary>
	/// Get the translated property description
	/// </summary>
	/// <returns>The translated property description</returns>
	public string GetPropertyDesciption()
	{
		return propertyManager.globalRefManager.langManager.GetTranslation("info_" +fullTranslationID.ToLower());
	}
}
