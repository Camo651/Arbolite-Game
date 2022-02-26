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

	public void OnButtonClick()
	{
		propertyDisplayer.SetHighlightedPropertyTag(this);
	}

	public string GetTagName()
	{
		return propertyManager.globalRefManager.langManager.GetTranslation("name_"+fullTranslationID);
	}
	public string GetPropertyType()
	{
		return propertyManager.globalRefManager.langManager.GetTranslation("proptype_"+(""+property.propertyType).ToLower());
	}
	public string GetPropertyDesciption()
	{
		return propertyManager.globalRefManager.langManager.GetTranslation("info_" +fullTranslationID.ToLower());
	}
}
