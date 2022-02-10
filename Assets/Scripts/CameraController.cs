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
	private float cameraZoomAccelereation = 0;

	private void Update()
	{
		if (!globalRefManager.baseManager.gameIsActivelyFrozen)
		{
			//Camera Zooming
			if (Input.mouseScrollDelta.y != 0)
				cameraZoomAccelereation = -Input.mouseScrollDelta.y;
			else
			{
				//stops the camera from deccelerating when it reaches a low enough speed
				if (Mathf.Abs(cameraZoomAccelereation) > 0.1f)
					cameraZoomAccelereation -= (cameraAccelSpeed * Time.deltaTime * Mathf.Sign(cameraZoomAccelereation));
				else
					cameraZoomAccelereation = 0f;
			}
			mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize + (cameraZoomAccelereation * cameraZoomSpeed * Time.deltaTime), cameraZoomBounds.x, cameraZoomBounds.y);

			//moves the camera towards the cursor so it gives a better zooming effect
			Vector3 lateralZoomBuffer = Vector3.zero;
			if(mainCamera.orthographicSize > cameraZoomBounds.x && mainCamera.orthographicSize < cameraZoomBounds.y)
			{
				lateralZoomBuffer = new Vector3((Input.mousePosition.x / Screen.width - .5f) * 2.3f, (Input.mousePosition.y / Screen.height - .5f) * 2f, 0f);
				lateralZoomBuffer *= -cameraZoomAccelereation / 17f;
				if(globalRefManager.settingsManager.zoomTowardsMouse)
					trueCameraPosition += lateralZoomBuffer;
			}

			//Camera Movements
			trueCameraPosition.x = Mathf.Clamp(trueCameraPosition.x + (Input.GetAxis("Horizontal") * cameraMoveSpeed.x * mainCamera.orthographicSize * Time.deltaTime), cameraBounds.x + (mainCamera.orthographicSize * mainCamera.aspect), cameraBounds.y - (mainCamera.orthographicSize * mainCamera.aspect));
			trueCameraPosition.y = Mathf.Clamp(trueCameraPosition.y + (Input.GetAxis("Vertical") * cameraMoveSpeed.y * mainCamera.orthographicSize * Time.deltaTime), cameraBounds.z + mainCamera.orthographicSize, cameraBounds.w - mainCamera.orthographicSize);
			mainCamera.transform.position = trueCameraPosition;

		}
	}
}