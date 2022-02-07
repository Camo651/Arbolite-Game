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
	public Queue<UserInterface> activeNotificationQueue, pastNotificationsQueue;


	private void Start()
	{
		InitializeUserInterface();
	}

	// sets all the default or player pref values for what all the interfaces should look like on startup of the main game
	private void InitializeUserInterface()
	{
		SetBackgroundBlur(false);
	}


	//adds a notification to the end of the stream of notifications to be seen
	public void EnqueueNotification(string title, string descript, int upTime, bool toBeSaved)
	{

	}
	public void DequeueNotification()
	{

	}
	public void SetBackgroundBlur(bool state)
	{
		backgroundBlur.enabled = state;
	}
}
