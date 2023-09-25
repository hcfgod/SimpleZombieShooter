using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
	public delegate void TargetHitEventHandler(GameObject target);
	
	public event TargetHitEventHandler OnTargetHit;
	
	private Rigidbody bulletRigidBody;
	
	public List<Transform> IgnoreTransforms = new List<Transform>();
	
	[SerializeField] private GameObject impactEffectPrefab;
	[SerializeField] private AudioClip impactAudioClip;
	
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
			if (IgnoreTransforms.Contains(hitInfo.transform))
			{
				return; // We hit ourselves or a child object, so return
			}
			
			// Instantiate the impact effect at the collision point and align it with the surface normal
			if (impactEffectPrefab != null)
			{
				GameObject impactEffect = Instantiate(impactEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
				Destroy(impactEffect, 2f);  // Destroy the effect after 2 seconds
			}
			
			if(impactAudioClip != null)
			{
				AudioManager.instance.PlaySFX(impactAudioClip, 0.025f, true);
			}
			
			Debug.Log(hitInfo.transform.name);
			
			bulletRigidBody.velocity = Vector3.zero;
			gameObject.SetActive(false);
		
			OnTargetHit?.DynamicInvoke(hitInfo.transform.gameObject);
		}
		
	}
	
	private void OnTriggerEnter(Collider other)
	{

	}
}