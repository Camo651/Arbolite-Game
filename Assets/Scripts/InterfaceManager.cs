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
	public UserInterface activeUserInterface, errorModal;
	public GameObject notificationInterfacePrefab, notificationHolder;
	public int notificationPersistUptimeSeconds;
	public bool userIsHoveredOnInterfaceElement;
	public List<UserInterface> allUserInterfaces;
	public List<SO_NotificationType> notificationTypes;
	[HideInInspector] public Queue<UserInterface> activeNotificationQueue;
	//[HideInInspector] public Stack<UserInterface> pastNotificationsStack;

	private void Start()
	{
		InitializeUserInterface();
	}
	private void Update()
	{
		HandlePlayerInputCycle();
	}

	//should be a mess and take care of all the keyboard input in one place so it doesnt get spread around the other managers
	private void HandlePlayerInputCycle()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			SetMajorInterface("Home");
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SetMajorInterface("Pause");
		}
	}

	// sets all the default or player pref values for what all the interfaces should look like on startup of the main game
	private void InitializeUserInterface()
	{
		SetBackgroundBlur(false);
		globalRefManager.baseManager.gameIsActivelyFrozen = false;
		allUserInterfaces = new List<UserInterface>();
		allUserInterfaces.AddRange((UserInterface[])Resources.FindObjectsOfTypeAll(typeof(UserInterface)));
		foreach (UserInterface userInterface in allUserInterfaces)
		{
			userInterface.interfaceManager = this;
			if(userInterface.interfaceType != UserInterface.InterfaceType.HUD && 
				userInterface.interfaceType != UserInterface.InterfaceType.WorldSpace && 
				userInterface.interfaceType != UserInterface.InterfaceType.Notification)
			userInterface.gameObject.SetActive(false);
		}
		activeNotificationQueue = new Queue<UserInterface>();
	}
	
	//opens up a fullscreen interface / menu
	public void SetMajorInterface(string UiName)
	{
		UserInterface UI = GetUserInterface(UiName);
		if(activeUserInterface!=null)
			activeUserInterface.gameObject.SetActive(false);
		activeUserInterface = UI;
		activeUserInterface.gameObject.SetActive(true);
		SetInterfaceLanguage(UI);
		SetBackgroundBlur(UI.interfaceType == UserInterface.InterfaceType.FullScreen || UI.interfaceType == UserInterface.InterfaceType.Modal);
		globalRefManager.baseManager.gameIsActivelyFrozen = UI.interfaceType == UserInterface.InterfaceType.FullScreen || UI.interfaceType == UserInterface.InterfaceType.Modal;
	}

	//close the currently open interface
	public void CloseAllInterfaces()
	{
		SetBackgroundBlur(false);
		globalRefManager.baseManager.gameIsActivelyFrozen = false;
		activeUserInterface.gameObject.SetActive(false);
		userIsHoveredOnInterfaceElement = false;
	}

	//finds all the text elements in an interface and translates them
	public void SetInterfaceLanguage(UserInterface ui)
	{
		ui.SetChildrenKeys();
	}

	//toggles the state of the player hovering over a UI element
	public void SetInterfaceHoverState(bool state)
	{
		userIsHoveredOnInterfaceElement = state;
	}

	//returns the interface with the given ID, or defaults to the error modal
	public UserInterface GetUserInterface(string callbackID)
	{
		foreach (UserInterface userInterface in allUserInterfaces)
		{
			if (userInterface.interfaceCallbackID.ToLower() == callbackID.ToLower())
			{
				return userInterface;
			}
		}
		return errorModal; //defaults to the error modal
	}

	//returns a notification type if it exits
	public SO_NotificationType GetNotificationType(string ID)
	{
		foreach (SO_NotificationType type in notificationTypes)
		{
			if (type.callbackID.ToLower() == ID.ToLower())
				return type;
		}
		return notificationTypes[0]; //defaults to the not found notification, does not show error modal and interrupt tho
	}

	//adds a notification to the end of the stream of notifications to be seen
	public void EnqueueNotification(SO_NotificationType type, string customDescription)
	{
		GameObject note = Instantiate(notificationInterfacePrefab);
		note.SetActive(true);
		note.transform.SetParent(notificationHolder.transform);
		UserInterface ui = note.GetComponent<UserInterface>();
		SetInterfaceLanguage(ui);
		ui.mainInterfaceIcon.sprite = type.notificationIcon;
		ui.saveNotification = type.shouldBeSaved;
		ui.interfaceManager = this;
		activeNotificationQueue.Enqueue(ui);
		StartCoroutine(ui.DelayToClose(notificationPersistUptimeSeconds));
	}

	//removes the oldest notification
	public void DequeueNotification(UserInterface ui)
	{
		activeNotificationQueue.Dequeue();
		Destroy(ui.gameObject);

		//NOTE notification UI gets destroyed cause useless.. idk what to put here. figure it out when making old notes UI ig
		//if (notification.saveNotification)
		//	pastNotificationsStack.Push(notification);
	}

	//sets the state of the background blur
	public void SetBackgroundBlur(bool state)
	{
		backgroundBlur.enabled = state;
	}
}
