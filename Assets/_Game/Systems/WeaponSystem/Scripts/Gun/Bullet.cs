using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
	public delegate void TargetHitEventHandler(RaycastHit hitInfo);
	public event TargetHitEventHandler OnTargetHit;
	
	private Rigidbody bulletRigidBody;
	
	public List<string> IgnoreTransformsNames = new List<string>();

	private void Awake()
	{
		bulletRigidBody = GetComponent<Rigidbody>();
	}

	protected void Update()
	{
		// Use raycasting to find the exact point of impact
		RaycastHit hitInfo;
		
		if (Physics.Raycast(transform.position - bulletRigidBody.velocity.normalized, bulletRigidBody.velocity.normalized, out hitInfo))
		{
			// Check if the hit object is in the ignore list
			if (IgnoreTransformsNames.Contains(hitInfo.transform.name))
			{
				return; // We hit ourselves or a child object, so return
			}

			bulletRigidBody.velocity = Vector3.zero;
			gameObject.SetActive(false);
		
			OnTargetHit?.DynamicInvoke(hitInfo);
		}
		
	}
}