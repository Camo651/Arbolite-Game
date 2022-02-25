using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;


	//visual
	public bool rainFX;
	public Toggle _rainFX;
	public bool vSync;
	public Toggle _vSync;
	public bool fullscreenMode;
	public Toggle _fullscreenMode;
	public bool parallaxBackground;
	public Toggle _parallaxBackground;
	public bool doDaynightCycle;
	public Toggle _doDaynightCycle;
	public bool doPostProcessing;
	public Toggle _doPostProcessing;
	public bool antiAliasing;
	public Toggle _antiAliasing;
	public bool colourBlindMode;
	public Toggle _colourBlindMode;
	public bool showClouds;
	public Toggle _showClouds;

	//audio
	public float musicVolume;
	public Slider _musicVolume;
	public float interfaceVolume;
	public Slider _interfaceVolume;
	public float ambientVolume;
	public Slider _ambientVolume;
	public bool fadeAudioInUI;
	public Toggle _fadeAudioInUI;

	//controls
	public bool zoomTowardsMouse;
	public Toggle _zoomTowardsMouse;
	public bool invertScrollDirection;
	public Toggle _invertScrollDirection;
	public bool smoothCameraMovement;
	public Toggle _smoothCameraMovement;

	//text
	public bool showSubtitles;
	public Toggle _showSubtitles;
	public float fontSizeModifier;
	public Slider _fontSizeModifier;
	public string languageOption;
	public Slider languageOptionSlider;

	//keybinds
	public Dictionary<string, KeyCode> keyCodes;

	//colours
	public Color buildModeAllow;
	public Color buildModeDeny;

	public TextMeshProUGUI player_left;
	public TextMeshProUGUI player_right;
	public TextMeshProUGUI player_up;
	public TextMeshProUGUI player_down;
	public TextMeshProUGUI close_UI;
	public TextMeshProUGUI pause_menu;
	public TextMeshProUGUI home_menu;
	public TextMeshProUGUI LanguageOptionDisplay;

	private readonly List<KeyCode> ReservedKeys = new List<KeyCode>()
	{
		KeyCode.Mouse0,
		KeyCode.Mouse1,
		KeyCode.Mouse2,
		KeyCode.Mouse3,
		KeyCode.Mouse4,
		KeyCode.Mouse5,
		KeyCode.Mouse6,
		KeyCode.None,
		KeyCode.Numlock,
		KeyCode.SysReq,
		KeyCode.LeftApple,
		KeyCode.RightApple,
		KeyCode.LeftWindows,
		KeyCode.RightWindows,
		KeyCode.Pipe,
		KeyCode.Pause,
	};

	public bool keyBindIsBeingSet;

	public void Start()
	{
		ResetToDefaultSettings();
		SetSettingsDisplays();
		UpdateSettings();
	}

	public KeyCode GetKeyCode(string callbackID)
	{
		return keyCodes.ContainsKey(callbackID) ? keyCodes[callbackID] : KeyCode.None;
	}

	public void UpdateSettings()
	{
		rainFX = _rainFX.isOn;
		vSync = _vSync.isOn;
		fullscreenMode = _fullscreenMode.isOn;
		parallaxBackground = _parallaxBackground.isOn;
		doDaynightCycle = _doDaynightCycle.isOn;
		doPostProcessing = _doPostProcessing.isOn;
		antiAliasing = _antiAliasing.isOn;
		colourBlindMode = _colourBlindMode.isOn;
		showClouds = _showClouds.isOn;

		musicVolume = _musicVolume.value;
		interfaceVolume = _interfaceVolume.value;
		ambientVolume = _ambientVolume.value;
		fadeAudioInUI = _fadeAudioInUI.isOn;

		zoomTowardsMouse = _zoomTowardsMouse.isOn;
		invertScrollDirection = _invertScrollDirection.isOn;
		smoothCameraMovement = _smoothCameraMovement.isOn;

		showSubtitles = _showSubtitles.isOn;
		fontSizeModifier = _fontSizeModifier.value;


		QualitySettings.vSyncCount = vSync ? 2 : 0;
		Screen.fullScreenMode = fullscreenMode ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
		QualitySettings.antiAliasing = antiAliasing ? 0 : 3;
		for (int i = 0; i < globalRefManager.terrainManager.clouds.Count; i++)
		{
			CloudObj c = globalRefManager.terrainManager.clouds[i];
			c.active = UnityEngine.Random.value < globalRefManager.terrainManager.cloudiness && showClouds;
			c.rend.color = new Color(c.layer == 0 ? .85f : c.layer == 1 ? .9f : .95f, c.layer == 0 ? .85f : c.layer == 1 ? .9f : .95f, c.layer == 0 ? .95f : c.layer == 1 ? .98f : 1f, c.active ? .5f : 0f);
		}

		globalRefManager.audioManager.StopAllAudioSources();
		globalRefManager.audioManager.StopAllCoroutines();
		globalRefManager.audioManager.PlayMusic();

		globalRefManager.langManager.SetLanguage(languageOption);
	}

	public void SetSettingsDisplays()
	{
		_rainFX.isOn = rainFX;
		_vSync.isOn = vSync;
		_fullscreenMode.isOn = fullscreenMode;
		_parallaxBackground.isOn = parallaxBackground;
		_doDaynightCycle.isOn = doDaynightCycle;
		_doPostProcessing.isOn = doPostProcessing;
		_antiAliasing.isOn = antiAliasing;
		_colourBlindMode.isOn = colourBlindMode;
		_showClouds.isOn = showClouds;

		_musicVolume.value = musicVolume;
		_interfaceVolume.value = interfaceVolume;
		_ambientVolume.value = ambientVolume;
		_fadeAudioInUI.isOn = fadeAudioInUI;

		_zoomTowardsMouse.isOn = zoomTowardsMouse;
		_invertScrollDirection.isOn = invertScrollDirection;
		_smoothCameraMovement.isOn = smoothCameraMovement;

		_showSubtitles.isOn = showSubtitles;
		_fontSizeModifier.value = fontSizeModifier;

		player_left.text = FormatKeyCodeString(GetKeyCode("player_left"));
		player_right.text = FormatKeyCodeString(GetKeyCode("player_right"));
		player_up.text = FormatKeyCodeString(GetKeyCode("player_up"));
		player_down.text = FormatKeyCodeString(GetKeyCode("player_down"));
		close_UI.text = FormatKeyCodeString(GetKeyCode("close_UI"));
		pause_menu.text = FormatKeyCodeString(GetKeyCode("pause_menu"));
		home_menu.text = FormatKeyCodeString(GetKeyCode("home_menu"));

		SetLanguage();
	}

	public void ResetToDefaultSettings()
	{
		rainFX = true;
		vSync = false;
		fullscreenMode = false;
		parallaxBackground = true;
		doDaynightCycle = true;
		doPostProcessing = true;
		antiAliasing = false;
		colourBlindMode = false;
		showClouds = true;

		musicVolume = .25f;
		interfaceVolume = .5f;
		ambientVolume = .75f;
		fadeAudioInUI = true;

		zoomTowardsMouse = true;
		invertScrollDirection = false;
		smoothCameraMovement = true;

		showSubtitles = false;
		fontSizeModifier = 1f;
		languageOption = "en_us";

		keyCodes = new Dictionary<string, KeyCode>();
		keyCodes.Clear();
		keyCodes.Add("player_left", KeyCode.A);
		keyCodes.Add("player_right", KeyCode.D);
		keyCodes.Add("player_up", KeyCode.W);
		keyCodes.Add("player_down", KeyCode.S);
		keyCodes.Add("close_UI", KeyCode.Escape);
		keyCodes.Add("pause_menu", KeyCode.Escape);
		keyCodes.Add("home_menu", KeyCode.E);
		keyCodes.Add("mode_build", KeyCode.Alpha1);
		keyCodes.Add("mode_edit", KeyCode.Alpha2);
	}

	public void SetKeybind(TextMeshProUGUI box)
	{
		box.text = "";
		StartCoroutine(WaitForKeystrokeSet(box));
	}
	private string editCode;
	public void SetKeyEditCode(string callbackID)
	{
		editCode = callbackID;
	}

	IEnumerator WaitForKeystrokeSet(TextMeshProUGUI box)
	{
		yield return null;
		keyBindIsBeingSet = true;
		while (!Input.anyKeyDown)
		{
			yield return null;
		}
		foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKey(kcode))
			{
				if (keyCodes.ContainsKey(editCode) && !ReservedKeys.Contains(kcode))
				{
					keyCodes[editCode] = kcode;
					box.text = FormatKeyCodeString(kcode);
				}
			}
		}
		keyBindIsBeingSet = false;
	}

	private string FormatKeyCodeString(KeyCode k)
	{
		switch (k)
		{
			case KeyCode.Escape:return "Esc";
			case KeyCode.Return:return "Rtrn";
			case KeyCode.Tab:return "Tab";
			case KeyCode.LeftShift:return "LShf";
			case KeyCode.RightShift:return "RShf";
			case KeyCode.LeftAlt:return "LAlt";
			case KeyCode.RightAlt:return "RAlt";
			case KeyCode.Delete:return "Del";
			case KeyCode.Backspace:return "Bksp";
			case KeyCode.LeftControl:return "LCtr";
			case KeyCode.RightControl:return "RCtr";

			default: return k + "";
		}
	}

	public void SetLanguage()
	{
		LanguageOptionDisplay.text = globalRefManager.langManager.allLanguageOptions[(int)languageOptionSlider.value].langNativeName;
		languageOption = globalRefManager.langManager.allLanguageOptions[(int)languageOptionSlider.value].langID;
	}
}