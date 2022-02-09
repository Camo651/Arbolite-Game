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


	//audio
	public float musicVolume;
	public float interfaceVolume;
	public float ambientVolume;
}
