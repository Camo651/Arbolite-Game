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
	public UserInterface worldPosHoverHUD;
	public bool hoverHUDEnabled;
	public Vector3 hoverHudOffset;
	public Dictionary<string, UserInterface> allUserInterfaces;
	public Dictionary<string, SO_NotificationType> notificationTypes;
	[HideInInspector] public List<UserInterface> activeNotificationQueue;
	//[HideInInspector] public Stack<UserInterface> pastNotificationsStack;

	private void Start()
	{
		InitializeUserInterface();
	}
	private void Update()
	{
		HandlePlayerInputCycle();

	}

	/// <summary>
	/// Consolidates the player's input into one method to keep everything clean
	/// </summary>
	private void HandlePlayerInputCycle()
	{
		if (Input.anyKey && !globalRefManager.settingsManager.keyBindIsBeingSet)
		{
			if (Input.GetKeyDown(globalRefManager.settingsManager.GetKeyCode("home_menu")))
			{
				SetMajorInterface("Home");
				return;
			}
			if (activeUserInterface == null && Input.GetKeyDown(globalRefManager.settingsManager.GetKeyCode("pause_menu")))
			{
				SetMajorInterface("Pause_Menu");
				return;
			}
			if (Input.GetKeyDown(globalRefManager.settingsManager.GetKeyCode("close_UI")))
			{
				CloseAllInterfaces();
				return;
			}
			if (Input.GetKeyDown(globalRefManager.settingsManager.GetKeyCode("")))
			{
				globalRefManager.baseManager.SetPlayerState(BaseManager.PlayerState.PlayerMode);
			}
			if (Input.GetKeyDown(globalRefManager.settingsManager.GetKeyCode("mode_edit")))
			{
				globalRefManager.baseManager.SetPlayerState(BaseManager.PlayerState.EditMode);
			}
			if (Input.GetKeyDown(globalRefManager.settingsManager.GetKeyCode("mode_build")))
			{
				globalRefManager.baseManager.SetPlayerState(BaseManager.PlayerState.BuildMode);
			}
			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				globalRefManager.baseManager.selectedRoomName = "Big Room";
			}
			if(globalRefManager.settingsManager.developerMode && Input.GetKeyDown(KeyCode.Return))
			{
				if (activeUserInterface == null)
					SetMajorInterface("DevConsole");
				else
					CloseAllInterfaces();
			}
		}
	}
	/// <summary>
	/// sets all the default or player pref values for what all the interfaces should look like on startup of the main game
	/// </summary>
	private void InitializeUserInterface()
	{
		SetBackgroundBlur(false);
		globalRefManager.baseManager.gameIsActivelyFrozen = false;
		allUserInterfaces = new Dictionary<string, UserInterface>();
#pragma warning disable
		UserInterface[] _unsortedInterfaces = (UserInterface[])FindObjectsOfTypeAll(typeof(UserInterface));
#pragma warning enable
		foreach (UserInterface userInterface in _unsortedInterfaces)
		{
			if (!allUserInterfaces.ContainsKey(userInterface.interfaceCallbackID.ToLower()))
			{
				allUserInterfaces.Add(userInterface.interfaceCallbackID.ToLower(), userInterface);
				userInterface.interfaceManager = this;
				if(userInterface.interfaceType != UserInterface.InterfaceType.HUD)
				userInterface.gameObject.SetActive(false);
			}
		}
		notificationTypes = new Dictionary<string, SO_NotificationType>();
		SO_NotificationType[] _unsortedNotes = (SO_NotificationType[])Resources.LoadAll<SO_NotificationType>("");
		foreach (SO_NotificationType note in _unsortedNotes)
		{
			if (!notificationTypes.ContainsKey(note.callbackID.ToLower()))
			{
				notificationTypes.Add(note.callbackID.ToLower(), note);
			}
		}
		activeNotificationQueue = new List<UserInterface>();
	}

	/// <summary>
	/// Opens up a fullscreen or modal UI
	/// </summary>
	/// <param name="UiName">The callback ID of the UI element in the scene</param>
	public void SetMajorInterface(string UiName)
	{
		UserInterface UI = GetUserInterface(UiName);
		if(UI == errorModal)
		{
			ThrowErrorMessage("interface_not_found_error_message");
			return;
		}
		if(activeUserInterface!=null)
			activeUserInterface.gameObject.SetActive(false);
		activeUserInterface = UI;
		activeUserInterface.gameObject.SetActive(true);
		SetInterfaceLanguage(UI);
		SetBackgroundBlur(UI.interfaceType == UserInterface.InterfaceType.FullScreen || UI.interfaceType == UserInterface.InterfaceType.Modal);
		globalRefManager.baseManager.gameIsActivelyFrozen = UI.interfaceType == UserInterface.InterfaceType.FullScreen || UI.interfaceType == UserInterface.InterfaceType.Modal;
		GetUserInterface("Home_Button").gameObject.SetActive(false);
		if(activeUserInterface.interfaceCallbackID == "Pause_Menu")
			globalRefManager.audioManager.Play(AudioManager.AudioClipType.Interface, "toggle_pause");
		else
			globalRefManager.audioManager.Play(AudioManager.AudioClipType.Interface, "toggle_ui");
	}

	/// <summary>
	/// Opens a link in a browser
	/// </summary>
	/// <param name="url"></param>
	public void OpenLink(string url)
	{
		Application.OpenURL(url);
	}

	/// <summary>
	/// Closes all other UI and opens the error modal with the message
	/// </summary>
	/// <param name="errorMessageCallbackID"></param>
	public void ThrowErrorMessage(string errorMessageCallbackID)
	{
		CloseAllInterfaces();
		SetBackgroundBlur(true);
		globalRefManager.baseManager.gameIsActivelyFrozen = true;
		activeUserInterface = errorModal;
		activeUserInterface.gameObject.SetActive(true);
		SetInterfaceLanguage(errorModal);
		errorModal.interfaceDescription.text = globalRefManager.langManager.GetTranslation(errorMessageCallbackID != "" ? errorMessageCallbackID : "error_modal_info");
		GetUserInterface("Home_Button").gameObject.SetActive(false);
	}

	//set worldposition viewer state
	public void SetWorldPositionViewerState(bool enabled, RoomTile rt)
	{
		hoverHUDEnabled = enabled;
		worldPosHoverHUD.gameObject.SetActive(enabled);
		if (enabled)
		{
			worldPosHoverHUD.transform.position = hoverHudOffset + globalRefManager.baseManager.editModePermSelectedRoomTile.transform.position;
			worldPosHoverHUD.interfaceName.text = globalRefManager.langManager.GetTranslation("name_"+rt.roomContainer.tileNameInfoID.ToLower());
			worldPosHoverHUD.interfaceDescription.text = globalRefManager.langManager.GetTranslation("info_" + rt.roomContainer.tileNameInfoID.ToLower());
		}
		else
		{
			userIsHoveredOnInterfaceElement = false;
			globalRefManager.baseManager.clickedSelect.ClearSelection();
		}
	}

	//closes the currently hovered tile menu thing
	public void CloseWorldPositionViewer()
	{
		SetWorldPositionViewerState(false, null);
		if (globalRefManager.baseManager.editModePermSelectedRoomTile != null)
			globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.SetRoomTint(Color.white);
		globalRefManager.baseManager.editModePermSelectedRoomTile = null;
	}

	//recives the call to delete the currently seleced tile
	public void DeleteCurrentltySelecedTile()
	{
		globalRefManager.baseManager.TryDestroyCurrentlySelectedTile();
	}

	/// <summary>
	/// Opens the advanced inspector modal and defaults to the info tab (0).
	/// Also chooses which tabs are availble to press based on the rooms properties
	/// </summary>
	public void OpenSelectedTileInfoModal()
	{
		SetMajorInterface("Inspector");
		activeUserInterface.SetInterfaceTab(0);
		activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabTitle.text = globalRefManager.langManager.GetTranslation("name_" + globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.tileNameInfoID.ToLower());
		activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabDescrition.text = globalRefManager.langManager.GetTranslation("info_" + globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.tileNameInfoID.ToLower());
		globalRefManager.propertyManager.GetPropertyDisplayer("TileInspector").DisplayProperties(globalRefManager.baseManager.editModePermSelectedRoomTile);

		if (globalRefManager.baseManager.editModePermSelectedRoomTile.canHavePlant)
		{
			globalRefManager.propertyManager.GetPropertyDisplayer("PlantInspector").tabSelectorButton.SetActive(true);
			globalRefManager.propertyManager.GetPropertyDisplayer("PlantInspector").DisplayProperties(globalRefManager.baseManager.editModePermSelectedRoomTile.thisRoomsPlant!=null? globalRefManager.baseManager.editModePermSelectedRoomTile.thisRoomsPlant.plantProperties:null);
		}
		else
		{
			globalRefManager.propertyManager.GetPropertyDisplayer("PlantInspector").tabSelectorButton.SetActive(false);
		}
		if (globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.rotorRoom)
		{
			globalRefManager.propertyManager.GetPropertyDisplayer("MechInspector").tabSelectorButton.SetActive(true);
			globalRefManager.propertyManager.GetPropertyDisplayer("MechInspector").DisplayProperties(globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.rotorRoom.thisRoomsRotor != null ? globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.rotorRoom.thisRoomsRotor.rotorProperties : null);
		}
		else
		{
			globalRefManager.propertyManager.GetPropertyDisplayer("MechInspector").tabSelectorButton.SetActive(false);
		}

	}

	/// <summary>
	/// Sets the data to be displayed in the inspector modal menu. Used from the button class in scene
	/// </summary>
	/// <param name="index">The menu index</param>
	public void SetValuesForInspector(int index)
	{
		ContainedRoom sel = globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer;
		switch (index)
		{
			case 0:
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabTitle.text = globalRefManager.langManager.GetTranslation("name_" + sel.tileNameInfoID.ToLower());
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabDescrition.text = globalRefManager.langManager.GetTranslation("info_" + sel.tileNameInfoID.ToLower());
				break;
			case 1:
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabTitle.text = globalRefManager.baseManager.editModePermSelectedRoomTile.thisRoomsPlant != null ? globalRefManager.baseManager.editModePermSelectedRoomTile.thisRoomsPlant.GetPlantFullName() : globalRefManager.langManager.GetTranslation("no_plant_in_tile");
				break;
			case 2:
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabTitle.text = globalRefManager.langManager.GetTranslation("name_" + sel.tileNameInfoID.ToLower());
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabDescrition.text = globalRefManager.langManager.GetTranslation("info_" + sel.tileNameInfoID.ToLower());
				string sub = "";
				sub += " - " + globalRefManager.langManager.GetTranslation("mechinfo_producing") + " : " + sel.rotorRoom.thisRoomsRotor.GetRotorProductionItems();
				sub += "\n - " + globalRefManager.langManager.GetTranslation("mechinfo_energy_" + (sel.rotorRoom.thisRoomsRotor.rotorType == Rotor.RotorType.Machine ?
																							"use" :
																							sel.rotorRoom.thisRoomsRotor.rotorType == Rotor.RotorType.Generator ?
																							"produce" :
																							"carry")) + " : " +
																							(sel.rotorRoom.thisRoomsRotor.rotorType == Rotor.RotorType.Driveshaft ? sel.rotorRoom.thisRoomsRotor.totalSystemEnergy : Mathf.Abs(sel.rotorRoom.thisRoomsRotor.energyDelta));
				sub += "\n - " + globalRefManager.langManager.GetTranslation("mechinfo_state") + " : " + globalRefManager.langManager.GetTranslation("name_prop_machinestate_" + ("" + sel.rotorRoom.thisRoomsRotor.GetRotorState().callbackID.ToLower()));
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabSubtitle.text = sub;
					//state + state prop
				break;
		}
	}

	//close the currently open interface
	public void CloseAllInterfaces()
	{
		SetBackgroundBlur(false);
		globalRefManager.baseManager.gameIsActivelyFrozen = false;
		if(activeUserInterface)
			activeUserInterface.gameObject.SetActive(false);
		activeUserInterface = null;
		userIsHoveredOnInterfaceElement = false;
		GetUserInterface("Home_Button").gameObject.SetActive(true);
	}

	//finds all the text elements in an interface and translates them
	public void SetInterfaceLanguage(UserInterface ui)
	{
		foreach (TranslationKey key in ui.interfaceKeys)
		{
			key.textBox.text = ui.interfaceManager.globalRefManager.langManager.GetTranslation(key.callBackID);
		}
	}

	//finds all the text elements in an interface and translates them
	public void SetInterfaceLanguage(UserInterface ui, string[] customData, string customDataCallbackID)
	{
		foreach (TranslationKey key in ui.interfaceKeys)
		{
			if(key.callBackID == customDataCallbackID && customData != null)
				key.textBox.text = InsertCustomData(ui.interfaceManager.globalRefManager.langManager.GetTranslation(key.callBackID),customData);
			else
				key.textBox.text = ui.interfaceManager.globalRefManager.langManager.GetTranslation(key.callBackID);
		}
	}
	//finds all the text elements in an interface and translates them
	public void SetInterfaceLanguage(UserInterface ui, SO_NotificationType type, string customDataCallbackID, string[] customData)
	{
		if (customData != null && customDataCallbackID == type.notificationName)
			ui.GetTranslationKey("notification_title").textBox.text = InsertCustomData(globalRefManager.langManager.GetTranslation("name_" + type.notificationName), customData);
		else
			ui.GetTranslationKey("notification_title").textBox.text = globalRefManager.langManager.GetTranslation("name_" + type.notificationName);

		if (customData != null && customDataCallbackID == type.notificationDescription)
			ui.GetTranslationKey("notification_info").textBox.text = InsertCustomData(globalRefManager.langManager.GetTranslation("info_" + type.notificationDescription),customData);
		else
			ui.GetTranslationKey("notification_info").textBox.text = globalRefManager.langManager.GetTranslation("info_" + type.notificationDescription);
	}

	/// <summary>
	/// Inserts the custom data into the string based on its tags
	/// </summary>
	/// <param name="original">The string with the tags</param>
	/// <param name="data">A list of enumerated data that can be interted</param>
	/// <returns>The string with the data inserted into it</returns>
	public string InsertCustomData(string original, string[] data)
	{
		for(int i = 0; i < data.Length; i++)
		{
			if (original.Contains("#" + i + "#"))
			{
				original = original.Replace("#" + i + "#", data[i]);
			}
		}
		return original;
	}
	/// <summary>
	/// Toggles the state of the hover on UI elements
	/// </summary>
	/// <param name="state"></param>
	public void SetInterfaceHoverState(bool state)
	{
		userIsHoveredOnInterfaceElement = state;
		globalRefManager.baseManager.hoverSelect.ClearSelection();
	}

	/// <summary>
	/// Gets the UI based on its ID
	/// </summary>
	/// <param name="callbackID">The callback ID</param>
	/// <returns>The UserInterface, given it existst</returns>
	public UserInterface GetUserInterface(string callbackID)
	{
		if (allUserInterfaces.ContainsKey(callbackID.ToLower()))
			return allUserInterfaces[callbackID.ToLower()];
		else
		{
			return errorModal;
		}
	}

	/// <summary>
	/// Get the notification type from its ID
	/// </summary>
	/// <param name="ID">The callback ID</param>
	/// <returns>The notification type, given it exists</returns>
	public SO_NotificationType GetNotificationType(string ID)
	{
		return notificationTypes.ContainsKey(ID.ToLower()) ? notificationTypes[ID.ToLower()] : notificationTypes["error"];
	}

	/// <summary>
	/// Adds a custom notification to the rendered notifications queues
	/// </summary>
	/// <param name="_type"> The callback id for the type of notification</param>
	/// <param name="customCallbackIDForData"> The callbackID for the text for a custom descriptor </param>
	/// <param name="customData"> A list of data that can be inserted into the custom data hashes in the key list</param>
	public void EnqueueNotification(string _type, string customCallbackIDForData, string[] customData)
	{
		SO_NotificationType type = GetNotificationType(_type);
		GameObject note = Instantiate(notificationInterfacePrefab);
		note.SetActive(true);
		note.transform.SetParent(notificationHolder.transform);
		UserInterface ui = note.GetComponent<UserInterface>();
		SetInterfaceLanguage(ui, type, customCallbackIDForData, customData);
		ui.mainInterfaceIcon.sprite = type.notificationIcon;
		ui.saveNotification = type.shouldBeSaved;
		ui.interfaceManager = this;
		activeNotificationQueue.Add(ui);
		StartCoroutine(ui.DelayToClose(notificationPersistUptimeSeconds));
	}

	/// <summary>
	/// Removes the last notification from the notification queue
	/// </summary>
	/// <param name="ui">The UI to be removed</param>
	public void DequeueNotification(UserInterface ui)
	{
		activeNotificationQueue.Remove(ui);
		if(ui)
			Destroy(ui.gameObject);
	}

	/// <summary>
	/// Sets the status of the background blur. Also handles the low pass filtering
	/// </summary>
	/// <param name="state">The state to be set to</param>
	public void SetBackgroundBlur(bool state)
	{
		globalRefManager.audioManager.SetBackgroundMusicLowPass(state);
		backgroundBlur.enabled = state;
	}
}
