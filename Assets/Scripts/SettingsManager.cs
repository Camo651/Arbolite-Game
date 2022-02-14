using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;


	//visual
	public bool rainFX;
	public bool vSync;
	public FullScreenMode windowMode;
	public bool parallaxBackground;
	public bool doDaynightCycle;
	public bool doPostProcessing;
	public bool antiAliasing;
	public bool colourBlindMode;

	//audio
	public float musicVolume;
	public float interfaceVolume;
	public float ambientVolume;

	//controls
	public bool zoomTowardsMouse;
	public bool invertScrollDirection;

	//text
	public bool showSubtitles;
	public float fontSizeModifier;
	public string languageOption;

	//keybinds
	public KeyCode playerLeft, playerRight, playerUp, playerDown;
	public KeyCode closeAllInterfaces;
	public KeyCode openPauseMenu, openHomeMenu;
	public KeyCode setModePlayer, setModeBuild, setModeEdit;

	public void UpdateSettings()
	{
		musicVolume = 1f;
		ambientVolume = 1f;
		interfaceVolume = 1f;

		globalRefManager.langManager.SetLanguage(languageOption);
	}


}