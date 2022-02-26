using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UserInterface : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	// any UI panel in game
	[HideInInspector]public InterfaceManager interfaceManager;
	public Image interfaceBackground;
	public InterfaceType interfaceType;
	public string interfaceCallbackID;
	public TextMeshProUGUI interfaceName;
	public TextMeshProUGUI interfaceDescription;
	public Image mainInterfaceIcon;
	public bool saveNotification;
	public Tab[] tabs;
	public int selectedTabIndex;
	public bool isHovered;
	public List<TranslationKey> interfaceKeys;

	private void Awake()
	{
		//indexes all the child keys
		interfaceKeys = new List<TranslationKey>();
		var tryGetComp = transform.GetComponentsInChildren(typeof(TranslationKey), true);
		if(tryGetComp != null && tryGetComp.Length > 0)
			foreach (Component component in tryGetComp)
			{
				interfaceKeys.Add((TranslationKey)component);
			}

		tabs = transform.GetComponentsInChildren<Tab>();

		if(tabs.Length > 0)
		{
			SetInterfaceTab(0);
		}
	}

	/// <summary>
	/// Sets the tab of this interface
	/// </summary>
	/// <param name="index">An index that exists in the array. -1 to close it</param>
	public void SetInterfaceTab(int index)
	{
		if(index == -1)
		{
			interfaceManager.CloseAllInterfaces();
			return;
		}
		if(index >= 0 && index < tabs.Length)
		{
			tabs[selectedTabIndex].SetTabState(false);
			selectedTabIndex = index;
			tabs[selectedTabIndex].SetTabState(true);
		}
		else
		{
			interfaceManager.ThrowErrorMessage("interface_not_found_error_message");
		}
	}

	/// <summary>
	/// Get the translation key in this UI's childen texts
	/// </summary>
	/// <param name="callbackID">The callback ID of the UGUI</param>
	/// <returns>The key object, given it exists</returns>
	public TranslationKey GetTranslationKey(string callbackID)
	{
		foreach (TranslationKey key in interfaceKeys)
		{
			if(key.callBackID.ToLower() == callbackID.ToLower())
			{
				return key;
			}
		}
		return null;
	}

	//transfer method for closing all the currently open interfaces
	public void CloseAllInterfaces()
	{
		interfaceManager.CloseAllInterfaces();
	}
	//transfer method for sending a notification
	public void SendNotification(string ID)
	{
		interfaceManager.EnqueueNotification(ID, "", null);
	}
	//transfer method for opening a menu
	public void SetInterface(string ID)
	{
		interfaceManager.SetMajorInterface(ID);
	}

	//wait the designated time, then close the notification if the game is not currently frozen for whatever reason
	public IEnumerator DelayToClose(int seconds)
	{
		yield return new WaitForSeconds(seconds);
		while (interfaceManager.globalRefManager.baseManager.gameIsActivelyFrozen)
			yield return null;
		if (isHovered)
			interfaceManager.SetInterfaceHoverState(false);
		interfaceManager.DequeueNotification(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		interfaceManager.SetInterfaceHoverState(true);
		isHovered = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		interfaceManager.SetInterfaceHoverState(false);
		isHovered = false;
	}

	public void OnButtonPress()
	{
		interfaceManager.SetInterfaceHoverState(false);
		interfaceManager.DequeueNotification(this);
	}


	public enum InterfaceType
	{
		FullScreen, //blur bkg, takes up whole screen
		HUD, // dont blur, persitent overlay when in game view
		Modal, //blur bkg, smaller interface
		WorldSpace, // dont blur bkg, not persistent, floats around in world space
		Notification, // dont blur, not persistent, queses in the notification bar
		Subtitle
	}
}
