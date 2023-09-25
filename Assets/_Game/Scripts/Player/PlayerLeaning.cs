using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeaning : MonoBehaviour
{
	[SerializeField] private Transform targetTransform;  // Reference to the target transform
	[SerializeField] private Transform cameraTransform;  // Reference to the camera transform
    
	[SerializeField] private float leanAngle = 15.0f;  // The angle to lean
	[SerializeField] private float leanSpeed = 2.0f;  // How fast to lean
    
	private float currentLean = 0.0f;  // Current lean angle

	private void Update()
	{
		HandleLeaning();
	}

	private void HandleLeaning()
	{
		float targetLean = 0.0f;

		if (Input.GetKey(KeyCode.Q))
		{
			targetLean = leanAngle;
		}
		else if (Input.GetKey(KeyCode.E))
		{
			targetLean = -leanAngle;
		}
		else
		{
			// Reset the lean angle when neither Q nor E is pressed
			targetLean = 0.0f;
		}

		currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);

		// Clamp the leaning angle to [-leanAngle, leanAngle]
		currentLean = Mathf.Clamp(currentLean, -leanAngle, leanAngle);

		// Apply the leaning to the camera's existing local rotation
		Vector3 currentEulerAngles = cameraTransform.localRotation.eulerAngles;
		cameraTransform.localRotation = Quaternion.Euler(currentEulerAngles.x, currentEulerAngles.y, currentLean);
	}
	
	private void HandleLeaning2()
	{
		float targetLean = 0.0f;

		if (Input.GetKey(KeyCode.Q))
		{
			targetLean = leanAngle;
		}
		else if (Input.GetKey(KeyCode.E))
		{
			targetLean = -leanAngle;
		}
        
		if(!Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Q))
		{
			targetTransform.rotation = cameraTransform.rotation;
		}

		currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);

		// Clamp the leaning angle to [-leanAngle, leanAngle]
		currentLean = Mathf.Clamp(currentLean, -leanAngle, leanAngle);

		// Apply the leaning to the camera's local rotation
		cameraTransform.localRotation = Quaternion.Euler(0, 0, currentLean);
	}
}
