using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UserInterface : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	// any UI panel in game
	public InterfaceManager interfaceManager;
	public Image interfaceBackground;
	public InterfaceType interfaceType;
	public string interfaceCallbackID;
	public TextMeshProUGUI interfaceName;
	public TextMeshProUGUI interfaceDescription;
	public Image mainInterfaceIcon;
	public bool saveNotification;
	public Tab[] tabs;
	public TabMenuButton[] tabMenuButtons;
	public int selectedTabIndex;
	public bool isHovered;
	public List<TranslationKey> interfaceKeys;

	private void Awake()
	{
		if (!interfaceManager)
			interfaceManager = FindObjectOfType<InterfaceManager>(false);
		//indexes all the child keys
		interfaceKeys = new List<TranslationKey>();
		TranslationKey[] tryGetComp = transform.GetComponentsInChildren<TranslationKey>(true);
		if(tryGetComp != null && tryGetComp.Length > 0)
			foreach (TranslationKey key in tryGetComp)
			{
				key.Init();
				interfaceKeys.Add(key);
			}

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
		interfaceManager.CloseInspectorHighlights();
		if(index == -1)
		{
			interfaceManager.CloseAllInterfaces();
			return;
		}
		if(GetTab(index) != null)
		{
			GetTab(selectedTabIndex).SetTabState(false);
			GetTab(index).SetTabState(true);
			selectedTabIndex = index;
		}
		else
		{
			interfaceManager.ThrowErrorMessage("interface_not_found_error_message");
		}
	}

	/// <summary>
	/// Gets the tab fromt the children
	/// </summary>
	/// <param name="index"></param>
	/// <returns>The tab, given it exists</returns>
	public Tab GetTab(int index)
	{
		foreach (Tab tab in tabs)
		{
			if (tab.tabIndex == index)
				return tab;
		}
		return null;
	}

	/// <summary>
	/// Gets the tab button from the children
	/// </summary>
	/// <param name="index"></param>
	/// <returns>The tab button, given it exists</returns>
	public TabMenuButton GetTabButton(int index)
	{
		foreach (TabMenuButton tab in tabMenuButtons)
		{
			if (tab.index == index)
				return tab;
		}
		return null;
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

	/// <summary>
	/// transfer method for closing all the currently open interfaces
	/// </summary>
	public void CloseAllInterfaces()
	{
		interfaceManager.CloseAllInterfaces();
	}

	/// <summary>
	/// transfer method for sending a notification to the interface manager
	/// </summary>
	/// <param name="ID"></param>
	public void SendNotification(string ID)
	{
		interfaceManager.EnqueueNotification(ID, "", null);
	}

	/// <summary>
	/// Transfer method for setting the current major interface
	/// </summary>
	/// <param name="ID"></param>
	public void SetInterface(string ID)
	{
		interfaceManager.SetMajorInterface(ID);
	}

	/// <summary>
	/// wait the designated time, then close the notification if the game is not currently frozen for whatever reason
	/// </summary>
	/// <param name="seconds">The delay, in seconds</param>
	/// <returns>Nothing</returns>
	public IEnumerator DelayToClose(int seconds)
	{
		yield return new WaitForSeconds(seconds);
		if (isHovered)
			interfaceManager.SetInterfaceHoverState(false);
		interfaceManager.DequeueNotification(this);
	}

	/// <summary>
	/// Called when the pointer enters the GUI
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		interfaceManager.SetInterfaceHoverState(true);
		isHovered = true;
	}

	/// <summary>
	/// Called when the pointer exits the GUI
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerExit(PointerEventData eventData)
	{
		interfaceManager.SetInterfaceHoverState(false);
		isHovered = false;
	}

	/// <summary>
	/// Recieves a button pulse
	/// </summary>
	public void OnButtonPress()
	{
		if(interfaceType == InterfaceType.Notification)
		{
			interfaceManager.SetInterfaceHoverState(false);
			interfaceManager.DequeueNotification(this);
		}
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
