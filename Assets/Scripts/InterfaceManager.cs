using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InterfaceManager : MonoBehaviour
{
	//controls all the UI in the game
	[HideInInspector] public GlobalRefManager globalRefManager;
	public Image backgroundBlur;
	public UserInterface activeUserInterface;
	public GameObject notificationInterfacePrefab, notificationHolder;
	public int notificationPersistUptimeSeconds;
	public int timeOfLastNotificationUpdate;
	public List<SO_NotificationType> notificationTypes;
	[HideInInspector] public Queue<UserInterface> activeNotificationQueue;
	//[HideInInspector] public Stack<UserInterface> pastNotificationsStack;

	private void Start()
	{
		InitializeUserInterface();
	}

	// sets all the default or player pref values for what all the interfaces should look like on startup of the main game
	private void InitializeUserInterface()
	{
		SetBackgroundBlur(false);
		activeNotificationQueue = new Queue<UserInterface>();
		timeOfLastNotificationUpdate = (int)Time.unscaledTime;
	}

	private void Update()
	{
		if(activeNotificationQueue.Count > 0 && Time.unscaledTime - timeOfLastNotificationUpdate > notificationPersistUptimeSeconds)
		{
			DequeueNotification();

			timeOfLastNotificationUpdate = (int)Time.unscaledTime;
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			EnqueueNotification(GetNotificationType("Default"),"");
		}
	}


	public SO_NotificationType GetNotificationType(string ID)
	{
		foreach (SO_NotificationType type in notificationTypes)
		{
			if (type.callbackID.ToLower() == ID.ToLower())
				return type;
		}
		return null;
	}

	//adds a notification to the end of the stream of notifications to be seen
	public void EnqueueNotification(SO_NotificationType type, string customDescription)
	{
		GameObject note = Instantiate(notificationInterfacePrefab);
		note.transform.SetParent(notificationHolder.transform);
		UserInterface ui = note.GetComponent<UserInterface>();
		ui.interfaceName.text = type.notificationName;
		ui.interfaceDescription.text = (customDescription == "" ? type.notificationDescription : customDescription);
		ui.mainInterfaceIcon.sprite = type.notificationIcon;
		ui.saveNotification = type.shouldBeSaved;
		ui.interfaceManager = this;
		timeOfLastNotificationUpdate = (int)Time.unscaledTime;
		activeNotificationQueue.Enqueue(ui);
	}
	public void DequeueNotification()
	{
		UserInterface notification = activeNotificationQueue.Dequeue();
		Destroy(notification.gameObject);

		//NOTE notification UI gets destroyed cause useless.. idk what to put here. figure it out when making old notes UI ig
		//if (notification.saveNotification)
		//	pastNotificationsStack.Push(notification);
	}
	public void SetBackgroundBlur(bool state)
	{
		backgroundBlur.enabled = state;
	}
}
