using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

	//keybinds
	public KeyCode playerLeft, playerRight, playerUp, playerDown, playerJump;
	public KeyCode closeAllInterfaces;
	public KeyCode openPauseMenu, openHomeMenu;
	public KeyCode setModePlayer, setModeBuild, setModeEdit;

	//colours
	public Color buildModeAllow;
	public Color buildModeDeny;

	public void Start()
	{
		ResetToDefaultSettings();
		SetSettingsDisplays();
		UpdateSettings();
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
	}


}