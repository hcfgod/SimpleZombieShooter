using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaycastProjectile : MonoBehaviour, IProjectileType
{
	[SerializeField] private GameObject playerRoot;
	[SerializeField] private PlayerData playerData;
	[SerializeField] private GunData gunData;
	
	public UnityEvent2 OnTargetHitUnityEvent;
	public delegate void TargetHitEventHandler(GameObject target);
	
	public event TargetHitEventHandler OnTargetHit;
	
	[SerializeField] private Camera _camera;
	
	[SerializeField] private LayerMask _hitLayers; // Layers that the raycast can hit

	[SerializeField] private List<ParticleSystem> muzzleFlashEffects;
	
	[SerializeField] private GameObject impactEffectPrefab;
	[SerializeField] private AudioClip impactAudioClip;
	
	private List<Transform> _ignoreTransforms;
	
	private WeaponSpread _weaponSpread;
	
	private void Awake()
	{
		_weaponSpread = GetComponent<WeaponSpread>();
	}
	
	private void Start()
	{
		// Initialize the list of transforms to ignore
		_ignoreTransforms = ComponentUtils.CollectAllTransforms(playerRoot.transform);
	}
	
	private void Update()
	{
		_weaponSpread.UpdateSpread(playerData);
	}
	
	public void Fire(WeaponData weaponData)
	{
		RaycastHit hit;
		Vector3 start = _camera.transform.position;
		Vector3 direction = _camera.transform.forward;

		float spread = _weaponSpread.GetCurrentSpread();
		
		direction.x += Random.Range(-spread, spread);
		direction.y += Random.Range(-spread, spread);
		
		if (Physics.Raycast(start, direction, out hit, weaponData.Range, _hitLayers))
		{
			// Check if the hit object is in the ignore list
			if (_ignoreTransforms.Contains(hit.transform))
			{
				return; // We hit ourselves or a child object, so return
			}
			
			// Instantiate the impact effect at the collision point and align it with the surface normal
			if (impactEffectPrefab != null)
			{
				GameObject impactEffect = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
				Destroy(impactEffect, 2f);  // Destroy the effect after 2 seconds
			}
			
			if(impactAudioClip != null)
			{
				AudioManager.instance.PlaySFX(impactAudioClip, 0.5f, true);
			}
			
			foreach(ParticleSystem particalSystem in muzzleFlashEffects)
			{
				particalSystem.Play();
			}
			
			OnTargetHit?.DynamicInvoke();
			OnTargetHitUnityEvent?.Invoke();
		} 
	}
	
	public void StopFiring()
	{
		gunData.isShooting = false;
	}
}
