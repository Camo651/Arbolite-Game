using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public Camera mainCamera;
	[HideInInspector] public GlobalRefManager globalRefManager;
	[Tooltip("Xmin Xmax Ymin Ymax")]public Vector4 cameraBounds;
	public Vector2 cameraMoveSpeed;
	public float cameraZoomSpeed;
	public float cameraAccelSpeed;
	[Tooltip("Min Max")] public Vector2 cameraZoomBounds;
	private Vector3 trueCameraPosition = Vector3.back * 10;
	public float cameraZoomAccelereation = 0;
	public Vector2 keyInput = Vector2.zero;
	public float cameraRawInputAccel, camRawDeccelMult;
	public float camZoomRawAccel;

	private void Update()
	{
		if (!globalRefManager.baseManager.gameIsActivelyFrozen)
		{
			if (globalRefManager.baseManager.currentPlayerState != BaseManager.PlayerState.PlayerMode)
			{
				if (globalRefManager.settingsManager.smoothCameraMovement)
				{
					if (Input.mouseScrollDelta.y != 0)
						cameraZoomAccelereation = Mathf.Clamp(cameraZoomAccelereation + (Input.mouseScrollDelta.y * (globalRefManager.settingsManager.invertScrollDirection ? 1f : -1f)) * Time.deltaTime * 50f,-1,1);
					else
					{
						//stops the camera from deccelerating when it reaches a low enough speed
						if (Mathf.Abs(cameraZoomAccelereation) > 0.05f)
							cameraZoomAccelereation -= (cameraAccelSpeed * Time.deltaTime * camZoomRawAccel * Mathf.Sign(cameraZoomAccelereation));
						else
							cameraZoomAccelereation = 0f;
					}
				}
				else
				{
					cameraZoomAccelereation = Input.mouseScrollDelta.y * (globalRefManager.settingsManager.invertScrollDirection ? 1f : -1f) * 20f;
				}
				mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize + (cameraZoomAccelereation * cameraZoomSpeed * Time.deltaTime), cameraZoomBounds.x, cameraZoomBounds.y);

				//moves the camera towards the cursor so it gives a better zooming effect
				Vector3 lateralZoomBuffer = Vector3.zero;
				if (mainCamera.orthographicSize > cameraZoomBounds.x && mainCamera.orthographicSize < cameraZoomBounds.y)
				{
					lateralZoomBuffer = new Vector3((Input.mousePosition.x / Screen.width - .5f) * 2.3f, (Input.mousePosition.y / Screen.height - .5f) * 2f, 0f);
					lateralZoomBuffer *= -cameraZoomAccelereation / 17f;
					if (globalRefManager.settingsManager.zoomTowardsMouse)
						trueCameraPosition += lateralZoomBuffer;
				}

				//Camera Movements
				if (Input.anyKey)
				{
					switch (
						Input.GetKey(globalRefManager.settingsManager.GetKeyCode("player_right")),
						Input.GetKey(globalRefManager.settingsManager.GetKeyCode("player_left")))
					{
						case (true, true):keyInput.x += 0; ;break;
						case (false, false):keyInput.x += 0; ;break;
						case (true, false):keyInput.x += cameraRawInputAccel;break;
						case (false, true):keyInput.x += -cameraRawInputAccel; break;
					}
					switch (
						Input.GetKey(globalRefManager.settingsManager.GetKeyCode("player_up")),
						Input.GetKey(globalRefManager.settingsManager.GetKeyCode("player_down")))
					{
						case (true, true): keyInput.y += 0; ; break;
						case (false, false): keyInput.y += 0; ; break;
						case (true, false): keyInput.y += cameraRawInputAccel; break;
						case (false, true): keyInput.y += -cameraRawInputAccel; break;
					}
					keyInput.x = Mathf.Clamp(keyInput.x, -1, 1);
					keyInput.y = Mathf.Clamp(keyInput.y, -1, 1);
				}

				trueCameraPosition.x = Mathf.Clamp(trueCameraPosition.x + (keyInput.x * cameraMoveSpeed.x * mainCamera.orthographicSize * Time.deltaTime), cameraBounds.x + (mainCamera.orthographicSize * mainCamera.aspect), cameraBounds.y - (mainCamera.orthographicSize * mainCamera.aspect));
				trueCameraPosition.y = Mathf.Clamp(trueCameraPosition.y + (keyInput.y * cameraMoveSpeed.y * mainCamera.orthographicSize * Time.deltaTime), cameraBounds.z + mainCamera.orthographicSize, cameraBounds.w - mainCamera.orthographicSize);

				if (Mathf.Abs(keyInput.x) > 0.1f)
					keyInput.x -= (cameraRawInputAccel * Time.deltaTime * camRawDeccelMult * Mathf.Sign(keyInput.x));
				else
					keyInput.x = 0f;
				if (Mathf.Abs(keyInput.y) > 0.1f)
					keyInput.y -= (cameraRawInputAccel * Time.deltaTime * camRawDeccelMult * Mathf.Sign(keyInput.y));
				else
					keyInput.y = 0f;

			}
			else
			{
				trueCameraPosition.x = globalRefManager.playerManager.transform.position.x;
				trueCameraPosition.y = globalRefManager.playerManager.transform.position.y;
				trueCameraPosition.z = -10f;
			}
			mainCamera.transform.position = trueCameraPosition;

		}
	}
}