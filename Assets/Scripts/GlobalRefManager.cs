using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalRefManager : MonoBehaviour
{
	public BaseManager baseManager;
	public TerrainManager terrainManager;
	public ContentManager contentManager;
	public InterfaceManager interfaceManager;
	public CameraController cameraController;
	public FlowManager flowManager;

	private void Awake()
	{
		baseManager.globalRefManager = this;
		terrainManager.globalRefManager = this;
		contentManager.globalRefManager = this;
		interfaceManager.globalRefManager = this;
		cameraController.globalRefManager = this;
		flowManager.globalRefManager = this;
	}
}
