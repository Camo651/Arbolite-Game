using UnityEngine;

public class Spinner : MonoBehaviour
{
	public float spinningSpeed;

	private void FixedUpdate()
	{
		transform.Rotate(Vector3.forward*spinningSpeed);
	}
}
