using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
	// any UI panel in game
	public InterfaceManager interfaceManager;
	public Image interfaceBackground;
	public InterfaceType interfaceType;
	[HideInInspector] public bool interfaceLocked; //to be set on init
	public string interfaceName;










	public enum InterfaceType
	{
		FullScreen, //blur bkg, takes up whole screen
		HUD, // dont blur, persitent overlay when in game view
		Modal, //blur bkg, smaller interface
		WorldSpace, // dont blur bkg, not persistent, floats around in world space
		Notification // dont blur, not persistent, queses in the notification bar
	}
}
