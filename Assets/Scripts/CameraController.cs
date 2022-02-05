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
		//Camera Movements
		trueCameraPosition.x = Mathf.Clamp(trueCameraPosition.x + (Input.GetAxis("Horizontal") * cameraMoveSpeed.x * mainCamera.orthographicSize * Time.deltaTime),cameraBounds.x - mainCamera.orthographicSize, cameraBounds.y + mainCamera.orthographicSize);
		trueCameraPosition.y = Mathf.Clamp(trueCameraPosition.y + (Input.GetAxis("Vertical") * cameraMoveSpeed.y * mainCamera.orthographicSize * Time.deltaTime), mainCamera.orthographicSize + cameraBounds.z, mainCamera.orthographicSize +  cameraBounds.w);
		mainCamera.transform.position = trueCameraPosition;

		//Camera Zooming
		if (Input.mouseScrollDelta.y != 0)
			cameraZoomAccelereation = -Input.mouseScrollDelta.y;
		else
		{
			if (Mathf.Abs(cameraZoomAccelereation) > 0.1f)
				cameraZoomAccelereation -= (cameraAccelSpeed * Time.deltaTime * Mathf.Sign(cameraZoomAccelereation));
			else
				cameraZoomAccelereation = 0f;
		}
		mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize + (cameraZoomAccelereation * cameraZoomSpeed * Time.deltaTime), cameraZoomBounds.x, cameraZoomBounds.y);
	}
}