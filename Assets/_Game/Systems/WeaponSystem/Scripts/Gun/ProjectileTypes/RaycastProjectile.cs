using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaycastProjectile : MonoBehaviour, IProjectileType
{
	[SerializeField] private GameObject playerRoot;
	[SerializeField] private PlayerData playerData;
	[SerializeField] private WeaponData weaponData;
	[SerializeField] private GunData gunData;
	
	public UnityEvent2 OnTargetHitUnityEvent;
	public delegate void TargetHitEventHandler(GameObject target);
	
	public event TargetHitEventHandler OnTargetHit;
	
	[SerializeField] private Camera _camera;
	
	[SerializeField] private LayerMask _hitLayers; // Layers that the raycast can hit

	[SerializeField] private List<ParticleSystem> muzzleFlashEffects;
	
	[SerializeField] private GameObject impactEffectPrefab;
	[SerializeField] private GameObject fleshImpactEffectPrefab;
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
		RaycastHit hitInfo;
		Vector3 start = _camera.transform.position;
		Vector3 direction = _camera.transform.forward;

		float spread = _weaponSpread.GetCurrentSpread();
		
		direction.x += Random.Range(-spread, spread);
		direction.y += Random.Range(-spread, spread);
		
		if (Physics.Raycast(start, direction, out hitInfo, weaponData.Range, _hitLayers))
		{
			// Check if the hit object is in the ignore list
			if (_ignoreTransforms.Contains(hitInfo.transform))
			{
				return; // We hit ourselves or a child object, so return
			}
			
			foreach(ParticleSystem particalSystem in muzzleFlashEffects)
			{
				particalSystem.Play();
			}
			
			IDamagable iDamageable = hitInfo.transform.GetComponent<IDamagable>();
		
			if(iDamageable != null)
			{
				iDamageable.DamagableHit(hitInfo, weaponData);
			
				if(impactAudioClip != null)
				{
					AudioManager.instance.PlaySFX(impactAudioClip, 0.25f, true);
				}
				
				// Instantiate the impact effect at the collision point and align it with the surface normal
				if (impactEffectPrefab != null)
				{
					GameObject impactEffect = Instantiate(fleshImpactEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
					Destroy(impactEffect, 0.2f);  // Destroy the effect after 1 seconds
				}
			}
			else
			{
				// Instantiate the impact effect at the collision point and align it with the surface normal
				if (impactEffectPrefab != null)
				{
					GameObject impactEffect = Instantiate(impactEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
					Destroy(impactEffect, 2f);  // Destroy the effect after 2 seconds
				}
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
