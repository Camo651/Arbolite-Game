using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
	//controls all the UI in the game
	[HideInInspector] public GlobalRefManager globalRefManager;
	public Image backgroundBlur;
	public UserInterface activeUserInterface;
}
