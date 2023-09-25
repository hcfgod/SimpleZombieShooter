using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpread : MonoBehaviour
{
	[SerializeField] private float baseSpread = 1.0f;
	[SerializeField] private float movingSpread = 1.5f;
	[SerializeField] private float runningSpread = 2.5f;
	[SerializeField] private float jumpingSpread = 2.0f;
	[SerializeField] private float crouchingSpread = 0.5f;

	private float currentSpread;

	private void Start()
	{
		currentSpread = baseSpread;
	}

	public void UpdateSpread(PlayerData playerData)
	{
		if( playerData.isIdle)
			currentSpread = baseSpread;
			
		if( playerData.isWalking)
			currentSpread = movingSpread;
			
		if( playerData.isRunning)
			currentSpread = runningSpread;
			
		if( playerData.isCrouching)
			currentSpread = crouchingSpread;
			
		if(!playerData.isGrounded)
			currentSpread = jumpingSpread;
	}

	public float GetCurrentSpread()
	{
		return currentSpread;
	}
}
