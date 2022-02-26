using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public GlobalRefManager globalRefManager;
	public Rigidbody2D rb;
	public Vector3 playerPosition;
	public float playerLateralSpeed;
	public float playerJumpHeight;
	public bool physicalPlayerIsActive;
	public Vector3 inputMovement;
	public float lateralMovementSmoother;

	private void FixedUpdate()
	{
		if (physicalPlayerIsActive && !globalRefManager.baseManager.gameIsActivelyFrozen)
		{
			rb.AddForce(Vector2.right * inputMovement.x * playerLateralSpeed);
			rb.AddForce(Vector2.up * inputMovement.y * playerJumpHeight, ForceMode2D.Impulse);
			inputMovement.y = 0f;

		}
	}
	private void Update()
	{
		if (physicalPlayerIsActive && !globalRefManager.baseManager.gameIsActivelyFrozen)
		{
			inputMovement.x = Mathf.SmoothDamp(inputMovement.x, Input.GetKey(globalRefManager.settingsManager.GetKeyCode("player_right")) ? 1f : Input.GetKey(globalRefManager.settingsManager.GetKeyCode("player_left")) ? -1f : 0f, ref lateralMovementSmoother, .0001f);
		}
		//if (Input.GetKeyDown(globalRefManager.settingsManager.playerUp) || Input.GetKeyDown(globalRefManager.settingsManager.playerJump))
		//{
		//	inputMovement.y = 1f;
		//}
	}

	public Vector2Int GetPlayerTilePos()
	{
		return new Vector2Int(Mathf.RoundToInt(playerPosition.x), Mathf.RoundToInt(playerPosition.y));
	}
	public void SetPhysicalPlayerState(bool state)
	{
		physicalPlayerIsActive = state;
	}
}
