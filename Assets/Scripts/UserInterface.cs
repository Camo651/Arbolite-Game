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
	[HideInInspector] public bool interfaceLocked; //to be set on init
	public string interfaceCallbackID;
	public TextMeshProUGUI interfaceName;
	public TextMeshProUGUI interfaceDescription;
	public Image mainInterfaceIcon;
	public bool saveNotification;


	//transfer method for closing all the currently open interfaces
	public void CloseAllInterfaces()
	{
		interfaceManager.CloseAllInterfaces();
	}
	//transfer method for sending a notification
	public void SendNotification(string ID)
	{
		interfaceManager.EnqueueNotification(interfaceManager.GetNotificationType(ID), "");
	}
	//transfer method for opening a menu
	public void SetInterface(string ID)
	{
		interfaceManager.SetMajorInterface(interfaceManager.GetUserInterface(ID));
	}

	//wait the designated time, then close the notification if the game is not currently frozen for whatever reason
	public IEnumerator DelayToClose(int seconds)
	{
		yield return new WaitForSeconds(seconds);
		while (interfaceManager.globalRefManager.baseManager.gameIsActivelyFrozen)
			yield return null;
		interfaceManager.DequeueNotification(this);
	}


	public void OnPointerEnter(PointerEventData eventData)
	{
		interfaceManager.SetInterfaceHoverState(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		interfaceManager.SetInterfaceHoverState(false);
	}

	public enum InterfaceType
	{
		FullScreen, //blur bkg, takes up whole screen
		HUD, // dont blur, persitent overlay when in game view
		Modal, //blur bkg, smaller interface
		WorldSpace, // dont blur bkg, not persistent, floats around in world space
		Notification // dont blur, not persistent, queses in the notification bar
	}
}
