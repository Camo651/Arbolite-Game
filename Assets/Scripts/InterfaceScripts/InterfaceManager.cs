﻿using System.Collections;
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
	public InformationHighlighter inspectorPropertyHighlight, inspectorItemHighlight;
	public TreeDisplayer advancementsTreeDisplayer;
	public ItemDisplayer homepageInventoryView;
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
			else if (activeUserInterface == null && Input.GetKeyDown(globalRefManager.settingsManager.GetKeyCode("pause_menu")))
			{
				SetMajorInterface("Pause_Menu");
				return;
			}
			else if (Input.GetKeyDown(globalRefManager.settingsManager.GetKeyCode("close_UI")))
			{
				CloseAllInterfaces();
				return;
			}
			else if (Input.GetKeyDown(globalRefManager.settingsManager.GetKeyCode("confirm_option")))
			{
				CloseAllInterfaces();
				return;
			}
			else if(globalRefManager.settingsManager.developerMode && Input.GetKeyDown(KeyCode.Return))
			{
				if (activeUserInterface == null)
					SetMajorInterface("DevConsole");
				else
					CloseAllInterfaces();
			}
			else if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				TrySetTabFromKeyPress(0);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				TrySetTabFromKeyPress(1);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				TrySetTabFromKeyPress(2);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				TrySetTabFromKeyPress(3);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				TrySetTabFromKeyPress(4);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha6))
			{
				TrySetTabFromKeyPress(5);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				TrySetTabFromKeyPress(6);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha8))
			{
				TrySetTabFromKeyPress(7);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha9))
			{
				TrySetTabFromKeyPress(8);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				TrySetTabFromKeyPress(9);
			}
			else if (Input.GetKeyDown(KeyCode.BackQuote))
			{
				TrySetTabFromKeyPress(-1);
			}
		}
	}

	/// <summary>
	/// Sets the tab state of the current user interface, or open the homepage and do so
	/// </summary>
	/// <param name="index">The tab index</param>
	public void TrySetTabFromKeyPress(int index)
	{
		if(index == -1 && activeUserInterface)
		{
			activeUserInterface.SetInterfaceTab(-1);
		}
		else if (!activeUserInterface && index < 4)
		{
			SetMajorInterface("Home");
			TrySetTabFromKeyPress(index);
		}
		else if(activeUserInterface && activeUserInterface.tabs.Length > index && activeUserInterface.GetTabButton(index).gameObject.activeInHierarchy)
		{
			activeUserInterface.SetInterfaceTab(index);
			if(activeUserInterface.interfaceCallbackID == "Home")
			{
				SetHomepageTabValues(index);
			}
			else if(activeUserInterface.interfaceCallbackID == "Inspector")
			{
				SetValuesForInspector(index);
			}
		}
	}
	/// <summary>
	/// sets all the default or player pref values for what all the interfaces should look like on startup of the main game
	/// </summary>
	private void InitializeUserInterface()
	{
		SetBackgroundBlur(false);
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
		if (activeUserInterface && activeUserInterface.interfaceCallbackID == UiName)
			return;
		CloseAllInterfacesNoTween();
		UserInterface UI = GetUserInterface(UiName);
		if(UI == errorModal)
		{
			ThrowErrorMessage("interface_not_found_error_message");
			return;
		}
		activeUserInterface = UI;
		GetUserInterface("Home_Button").gameObject.SetActive(false);
		bool blur = UI.interfaceType == UserInterface.InterfaceType.FullScreen || UI.interfaceType == UserInterface.InterfaceType.Modal;
		StartCoroutine(TweenInterfaceAlpha(activeUserInterface, true, 25f, blur));
	}

	/// <summary>
	/// Fade the alpha of a menu
	/// </summary>
	/// <param name="group">The group to affect</param>
	/// <param name="state">The end state of the tween</param>
	/// <returns></returns>
	public IEnumerator TweenInterfaceAlpha(UserInterface u, bool state, float time, bool changeBkgBlur)
	{
		if (u)
		{
			if (state)
			{
				u.gameObject.SetActive(true);
				if (changeBkgBlur)
					SetBackgroundBlur(true);
				SetInterfaceLanguage(u);
				switch (u.interfaceCallbackID)
				{
					case "Pause_Menu":
						globalRefManager.audioManager.Play(AudioManager.AudioClipType.Interface, "toggle_pause");
						break;
					case "Home":
						activeUserInterface.SetInterfaceTab(0);
						SetHomepageTabValues(0);
						break;
					default:
						globalRefManager.audioManager.Play(AudioManager.AudioClipType.Interface, "toggle_ui");
						break;
				}
			}
			CanvasGroup group = u.GetComponent<CanvasGroup>();
			if (group)
			{
				WaitForSeconds waitForSeconds = new WaitForSeconds(.005f);
				group.alpha = state ? 0 : 1;
				for (int i = 0; i < time; i++)
				{
					yield return waitForSeconds;
					group.alpha = Mathf.Clamp01(state?(i / time):((time-i)/time));
				}
				group.alpha = state ? 1 : 0;
			}
			if (!state)
			{
				u.gameObject.SetActive(false);
				if(changeBkgBlur)
					SetBackgroundBlur(false);
			}
		}
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

		if (globalRefManager.baseManager.editModePermSelectedRoomTile.canHavePlant)
		{
			activeUserInterface.GetTabButton(1).gameObject.SetActive(true);
		}
		else
		{
			activeUserInterface.GetTabButton(1).gameObject.SetActive(false);
		}
		if (globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.rotorRoom)
		{
			activeUserInterface.GetTabButton(2).gameObject.SetActive(true);
		}
		else
		{
			activeUserInterface.GetTabButton(2).gameObject.SetActive(false);
		}
		if (globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.itemContainer)
		{
			activeUserInterface.GetTabButton(3).gameObject.SetActive(true);
		}
		else
		{
			activeUserInterface.GetTabButton(3).gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Sets the data to be displayed in the inspector modal menu. Used from the button class in scene
	/// </summary>
	/// <param name="index">The menu index</param>
	public void SetValuesForInspector(int index)
	{
		RoomTile tile = globalRefManager.baseManager.editModePermSelectedRoomTile;
		if (!tile)
			return;
		ContainedRoom sel = tile.roomContainer;
		switch (index)
		{
			case 0:
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabTitle.text = globalRefManager.langManager.GetTranslation("name_" + sel.tileNameInfoID.ToLower());
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabDescrition.text = globalRefManager.langManager.GetTranslation("info_" + sel.tileNameInfoID.ToLower());
				globalRefManager.propertyManager.GetPropertyDisplayer("TileInspector").DisplayProperties(globalRefManager.baseManager.editModePermSelectedRoomTile);
				break;
			case 1:
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabTitle.text = globalRefManager.baseManager.editModePermSelectedRoomTile.thisRoomsPlant != null ? globalRefManager.baseManager.editModePermSelectedRoomTile.thisRoomsPlant.GetPlantFullName() : globalRefManager.langManager.GetTranslation("no_plant_in_tile");
				globalRefManager.propertyManager.GetPropertyDisplayer("PlantInspector").DisplayProperties(globalRefManager.baseManager.editModePermSelectedRoomTile.thisRoomsPlant!=null? globalRefManager.baseManager.editModePermSelectedRoomTile.thisRoomsPlant.plantProperties:null);
				break;
			case 2:
				if (!sel.rotorRoom)
					return;
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
				sub += "\n - " + globalRefManager.langManager.GetTranslation("mechinfo_state") + " : " + globalRefManager.langManager.GetTranslation("name_prop_machinestate_" + ("" + sel.rotorRoom.thisRoomsRotor.rotorStateProperty.callbackID.ToLower()));
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabSubtitle.text = sub;
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabToggle.SetIsOnWithoutNotify(globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.rotorRoom.thisRoomsRotor.rotorIsEnabled);
				globalRefManager.propertyManager.GetPropertyDisplayer("MechInspector").DisplayProperties(globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.rotorRoom.thisRoomsRotor != null ? globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.rotorRoom.thisRoomsRotor.GetRotorProperties() : null);
				break;
			case 3:
				activeUserInterface.GetTab(activeUserInterface.selectedTabIndex).tabTitle.text = globalRefManager.langManager.GetTranslation("items_button");
				activeUserInterface.GetTab(3).transform.GetComponent<ItemDisplayer>().DisplayItems(globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.itemContainer.itemsInContainer, globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.itemContainer.GetCount());
				break;
		}
	}

	/// <summary>
	/// Closes the currently open interface menu and tweens it out
	/// </summary>
	public void CloseAllInterfaces()
	{
		StartCoroutine(TweenInterfaceAlpha(activeUserInterface, false, 25f, true));
		activeUserInterface = null;
		userIsHoveredOnInterfaceElement = false;
		GetUserInterface("Home_Button").gameObject.SetActive(true);
		CloseInspectorHighlights();
	}

	/// <summary>
	/// Immidiately closes the currently hovered interface with no tween or animation
	/// </summary>
	public void CloseAllInterfacesNoTween()
	{
		globalRefManager.baseManager.gameIsActivelyFrozen = false;
		if(activeUserInterface)
			activeUserInterface.gameObject.SetActive(false);
		activeUserInterface = null;
		userIsHoveredOnInterfaceElement = false;
		GetUserInterface("Home_Button").gameObject.SetActive(true);
		CloseInspectorHighlights();
	}

	/// <summary>
	/// Closes the inspector's highlight tabs
	/// </summary>
	public void CloseInspectorHighlights()
	{
		inspectorItemHighlight.CloseHighlight();
		inspectorPropertyHighlight.CloseHighlight();
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
		GameObject note = Instantiate(notificationInterfacePrefab, notificationHolder.transform);
		note.SetActive(true);
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
		StartCoroutine(TweenInterfaceAlpha(ui, false, 15f, false));
	}

	/// <summary>
	/// Sets the status of the background blur. Also handles the low pass filtering
	/// </summary>
	/// <param name="state">The state to be set to</param>
	public void SetBackgroundBlur(bool state)
	{
		globalRefManager.audioManager.SetBackgroundMusicLowPass(state);
		backgroundBlur.enabled = state;
		globalRefManager.baseManager.gameIsActivelyFrozen = state;
	}

	/// <summary>
	/// Called when a tab on the homepage UI is opened to initizlize its values
	/// </summary>
	/// <param name="index"></param>
	public void SetHomepageTabValues(int index)
	{
		switch (index)
		{
			case 0://home
				homepageInventoryView.DisplayItems(globalRefManager.itemManager.mainInventory.itemsInContainer, globalRefManager.itemManager.mainInventory.GetCount());
				break;
			case 1://building
				globalRefManager.blueprintManager.Initialize();
				break;
			case 2://advancements
				advancementsTreeDisplayer.DisplayTree();
				break;
			case 3://market
				break;
		}
	}

	/// <summary>
	/// Toggles the state of the currently selected rotor
	/// </summary>
	/// <param name="t">The toggle that is controlling the state value</param>
	public void ToggleSelectedRotorState(Toggle t)
	{
		if (globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.rotorRoom)
		{
			globalRefManager.baseManager.editModePermSelectedRoomTile.roomContainer.rotorRoom.thisRoomsRotor.ToggleRotorState(t);
		}
	}
}
