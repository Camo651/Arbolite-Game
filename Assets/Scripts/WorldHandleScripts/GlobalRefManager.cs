using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GlobalRefManager : MonoBehaviour
{
	//a script to give every manager in the game a way to reference any other manager with ease
	public BaseManager baseManager;
	public TerrainManager terrainManager;
	public InterfaceManager interfaceManager;
	public CameraController cameraController;
	public FlowManager flowManager;
	public LanguangeManager langManager;
	public SettingsManager settingsManager;
	public AudioManager audioManager;
	public ParticleManager particleManager;
	public PlantManager plantManager;
	public PlayerManager playerManager;
	public DeveloperManager developerManager;
	public PropertyManager propertyManager;
	public BlueprintManager blueprintManager;
	public ItemManager itemManager;
	private void Awake()
	{
		baseManager.globalRefManager = this;
		terrainManager.globalRefManager = this;
		interfaceManager.globalRefManager = this;
		cameraController.globalRefManager = this;
		flowManager.globalRefManager = this;
		langManager.globalRefManager = this;
		settingsManager.globalRefManager = this;
		audioManager.globalRefManager = this;
		particleManager.globalRefManager = this;
		plantManager.globalRefManager = this;
		playerManager.globalRefManager = this;
		developerManager.globalRefManager = this;
		propertyManager.globalRefManager = this;
	}
}
