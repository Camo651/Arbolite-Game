using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
	// any UI panel in game
	public Image interfaceBackground;
	public InterfaceType interfaceType;
	public enum InterfaceType
	{
		FullScreen, //blur bkg, takes up whole screen
		HUD, // dont blur, persitent overlay when in game view
		Modal, //blur bkg, smaller interface
		WorldSpace // dont blur bkg, not persistent, floats around in world space
	}
}
