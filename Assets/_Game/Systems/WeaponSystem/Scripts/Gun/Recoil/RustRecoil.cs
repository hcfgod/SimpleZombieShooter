using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;

public class RustRecoil : BaseRecoil
{
	[SerializeField] Transform cameraTransform;
	
	[SerializeField] Transform weaponTransform;
	
	[SerializeField] private float minWeaponKickbackAmount;
	[SerializeField] private float maxWeaponKickbackAmount;
	
	[SerializeField] private float recoilSmoothAmount;
	
	[SerializeField] private bool isSemiAuto = false;
	
	private Vector2 currentRecoilOffset; // The current offset applied to the weapon's aim or camera
	private int currentRecoilIndex; // The current index in the recoil pattern array
	
	private Vector3 originalWeaponPosition;
	private Vector3 targetWeaponPosition;
	private bool canResetWeapon = false;
	
	private Vector2 desiredRecoilOffset;
	
	private bool canApplyRecoil = false;
	
	// Initialize the recoil system
	private void Start()
	{
		originalWeaponPosition = weaponTransform.localPosition;
		
		currentRecoilOffset = Vector2.zero;
		currentRecoilIndex = 0;
	
		targetWeaponPosition = new Vector3(weaponTransform.localPosition.x, weaponTransform.localPosition.y, -Random.Range(minWeaponKickbackAmount, maxWeaponKickbackAmount));
	}

	private void Update()
	{
		if (canApplyRecoil && !isSemiAuto)
		{
			// Custom fast interpolation towards the desired recoil offset
			currentRecoilOffset += (desiredRecoilOffset - currentRecoilOffset) * recoilSmoothAmount * Time.deltaTime;
			ApplyOffsetToAim();
		}

		if(!isSemiAuto)
		{
			// Gradually reduce the desired recoil offset to allow player control
			desiredRecoilOffset = Vector2.Lerp(desiredRecoilOffset, Vector2.zero, Time.deltaTime * recoilData.recoveryRate);

			// Check if the recoil has been fully applied
			if (Vector2.Distance(currentRecoilOffset, desiredRecoilOffset) < 0.01f)
			{
				canApplyRecoil = false;
			}
		}

		ResetWeaponKickBack();
	}
	
	public override void ApplyRecoil()
	{
		Vector2 recoilStep = recoilData.recoilPattern[currentRecoilIndex];
		recoilStep *= (1 + Random.Range(-recoilData.randomnessFactor, recoilData.randomnessFactor));
		recoilStep *= recoilData.intensity;

		// Update the desired recoil offset
		desiredRecoilOffset += recoilStep;
		canApplyRecoil = true;  // Enable the recoil application

		currentRecoilIndex = (currentRecoilIndex + 1) % recoilData.recoilPattern.Length;
	}

	public void ApplySemiAutoCameraRecoil()
	{
		// Get the next step in the recoil pattern
		Vector2 recoilStep = recoilData.recoilPattern[currentRecoilIndex];

		// Apply randomness and intensity to the recoil step
		recoilStep *= (1 + Random.Range(-recoilData.randomnessFactor, recoilData.randomnessFactor));
		recoilStep *= recoilData.intensity;

		// Update the current recoil offset
		currentRecoilOffset += recoilStep;

		// Apply the recoil offset to the weapon's aim or camera
		ApplyOffsetToAimSemiAuto();

		// Update the current index for the next shot, looping back to the start if necessary
		currentRecoilIndex = (currentRecoilIndex + 1) % recoilData.recoilPattern.Length;
	}
	
	// Reset the recoil over time
	public override void ResetRecoil()
	{
		currentRecoilOffset = Vector2.zero;
		
		// Apply the updated recoil offset to reset the weapon's aim or camera
		ApplyOffsetToAim();
	}
	
	public void ApplyWeaponRecoil()
	{
		weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, targetWeaponPosition, Time.deltaTime * recoilData.recoveryRate);
	}
	
	public void SetCanApplyRecoil(bool value)
	{
		canApplyRecoil = value;
	}
	
	private void ResetWeaponKickBack()
	{
		if(weaponTransform.localPosition != originalWeaponPosition)
			canResetWeapon = true;
		
		if(weaponTransform.localPosition == originalWeaponPosition)
			canResetWeapon = false;
		
		if(canResetWeapon)
			weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, originalWeaponPosition, Time.deltaTime * recoilData.recoveryRate);
	}
	
	private void ApplyOffsetToAim()
	{
		// Target rotation based on the recoil offset
		Vector3 targetRotation = cameraTransform.localEulerAngles;
		targetRotation.x -= currentRecoilOffset.y; // Pitch
		targetRotation.y += currentRecoilOffset.x; // Yaw

		// Current rotation
		Vector3 currentRotation = cameraTransform.localEulerAngles;

		// Smoothly interpolate between the current and target rotations
		Vector3 smoothRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * recoilData.recoveryRate);

		// Apply the smooth rotation to the camera
		cameraTransform.localEulerAngles = smoothRotation;
	}
	
	private void ApplyOffsetToAimSemiAuto()
	{
		// Target rotation based on the recoil offset
		Vector3 targetRotation = cameraTransform.localEulerAngles;
		targetRotation.x -= currentRecoilOffset.y; // Pitch
		targetRotation.y += currentRecoilOffset.x; // Yaw

		// Current rotation
		Vector3 currentRotation = cameraTransform.localEulerAngles;

		// Apply the smooth rotation to the camera
		cameraTransform.localEulerAngles = targetRotation;
	}
}
